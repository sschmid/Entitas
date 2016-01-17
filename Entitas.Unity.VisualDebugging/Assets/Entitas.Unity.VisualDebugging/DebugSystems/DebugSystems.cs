using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    public class SystemInfo {
        public ISystem system { get { return _system; } }
        public SystemInterfaceFlags interfaceFlags { get { return _interfaceFlags; } }

        public bool isInitializeSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.IInitializeSystem) == SystemInterfaceFlags.IInitializeSystem; }
        }

        public bool isExecuteSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.IExecuteSystem) == SystemInterfaceFlags.IExecuteSystem; }
        }

        public bool isReactiveSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.IReactiveSystem) == SystemInterfaceFlags.IReactiveSystem; }
        }

        public string systemName { get { return _systemName; } }
        public double totalExecutionDuration { get { return _totalExecutionDuration; } }
        public double minExecutionDuration { get { return _minExecutionDuration; } }
        public double maxExecutionDuration { get { return _maxExecutionDuration; } }
        public double averageExecutionDuration {
            get { return _durationsCount == 0 ? 0 : _totalExecutionDuration / _durationsCount; }
        }

        public bool isActive;

        readonly ISystem _system;
        readonly SystemInterfaceFlags _interfaceFlags;

        readonly string _systemName;

        double _totalExecutionDuration = -1;
        double _minExecutionDuration;
        double _maxExecutionDuration;
        int _durationsCount;

        const string SYSTEM_SUFFIX = "System";

        public SystemInfo(ISystem system) {
            _system = system;

            var reactiveSystem = system as ReactiveSystem;
            var isReactive = reactiveSystem != null;
            Type systemType;
            if (isReactive) {
                _interfaceFlags = getInterfaceFlags(reactiveSystem.subsystem, isReactive);
                systemType = reactiveSystem.subsystem.GetType();
            } else {
                _interfaceFlags = getInterfaceFlags(system, isReactive);
                systemType = system.GetType();
            }

            var debugSystem = system as DebugSystems;
            if (debugSystem != null) {
                _systemName = debugSystem.name;
            } else {
                _systemName = systemType.Name.EndsWith(SYSTEM_SUFFIX, StringComparison.Ordinal)
                    ? systemType.Name.Substring(0, systemType.Name.Length - SYSTEM_SUFFIX.Length)
                    : systemType.Name;
            }
            
            isActive = true;
        }

        public void AddExecutionDuration(double executionDuration) {
            if (executionDuration < _minExecutionDuration || _totalExecutionDuration == -1) {
                _minExecutionDuration = executionDuration;
                if (_totalExecutionDuration == -1) {
                    _totalExecutionDuration = 0;
                }
            }
            if (executionDuration > _maxExecutionDuration) {
                _maxExecutionDuration = executionDuration;
            }

            _totalExecutionDuration += executionDuration;
            _durationsCount += 1;
        }

        public void Reset() {
            _totalExecutionDuration = 0;
            _durationsCount = 0;
        }

        static SystemInterfaceFlags getInterfaceFlags(ISystem system, bool isReactive) {
            var flags = SystemInterfaceFlags.None;
            if (system is IInitializeSystem) {
                flags |= SystemInterfaceFlags.IInitializeSystem;
            }
            if (system is IExecuteSystem) {
                flags |= SystemInterfaceFlags.IExecuteSystem;
            }
            if (isReactive) {
                flags |= SystemInterfaceFlags.IReactiveSystem;
            }

            return flags;
        }
    }

    public enum AvgResetInterval {
        EveryFrame = 1,
        Every30Frames = 30,
        Every60Frames = 60,
        Every120Frames = 120,
        Every300Frames = 300,
        Never = int.MaxValue
    }

    [Flags]
    public enum SystemInterfaceFlags {
        None = 0,
        IInitializeSystem = 1,
        IExecuteSystem = 2,
        IReactiveSystem = 4
    }

    public class DebugSystems : Systems {
        public int initializeSystemsCount { get { return _initializeSystems.Count; } }
        public int executeSystemsCount { get { return _executeSystems.Count; } }
        public int totalSystemsCount { get { return _systems.Count; } }

        public string name { get { return _name; } }
        public GameObject container { get { return _container.gameObject; } }
        public double totalDuration { get { return _totalDuration; } }
        public SystemInfo[] initializeSystemInfos { get { return _initializeSystemInfos.ToArray(); } }
        public SystemInfo[] executeSystemInfos { get { return _executeSystemInfos.ToArray(); } }

        public bool paused;

        public AvgResetInterval avgResetInterval = AvgResetInterval.Never;

        readonly string _name;

        readonly List<ISystem> _systems;
        readonly Transform _container;
        readonly List<SystemInfo> _initializeSystemInfos;
        readonly List<SystemInfo> _executeSystemInfos;
        readonly Stopwatch _stopwatch;
        double _totalDuration;

        public DebugSystems(string name = "Systems") {
            _name = name;
            _systems = new List<ISystem>();
            _container = new GameObject().transform;
            _container.gameObject.AddComponent<DebugSystemsBehaviour>().Init(this);
            _initializeSystemInfos = new List<SystemInfo>();
            _executeSystemInfos = new List<SystemInfo>();
            _stopwatch = new Stopwatch();
            updateName();
        }

        public override Systems Add(ISystem system) {
            _systems.Add(system);
            var debugSystems = system as DebugSystems;
            if (debugSystems != null) {
                debugSystems.container.transform.SetParent(_container.transform, false);
            }

            var systemInfo = new SystemInfo(system);
            if (systemInfo.isInitializeSystems) {
                _initializeSystemInfos.Add(new SystemInfo(system));
            }
            if (systemInfo.isExecuteSystems || systemInfo.isReactiveSystems) {
                _executeSystemInfos.Add(new SystemInfo(system));
            }

            return base.Add(system);
        }

        public void Reset() {
            foreach (var systemInfo in _initializeSystemInfos) {
                systemInfo.Reset();
            }
            foreach (var systemInfo in _executeSystemInfos) {
                systemInfo.Reset();
            }
        }

        public override void Initialize() {
            _totalDuration = 0;
            for (int i = 0, _initializeSystemsCount = _initializeSystems.Count; i < _initializeSystemsCount; i++) {
                var system = _initializeSystems[i];
                var systemInfo = _initializeSystemInfos[i];
                if (systemInfo.isActive) {
                    var duration = monitorSystemInitializeDuration(system);
                    _totalDuration += duration;
                    systemInfo.AddExecutionDuration(duration);
                }
            }

            updateName();
        }

        public override void Execute() {
            if (!paused) {
                Step();
            }
        }

        public void Step() {
            _totalDuration = 0;
            if (Time.frameCount % (int)avgResetInterval == 0) {
                Reset();
            }
            for (int i = 0, exeSystemsCount = _executeSystems.Count; i < exeSystemsCount; i++) {
                var system = _executeSystems[i];
                var systemInfo = _executeSystemInfos[i];
                if (systemInfo.isActive) {
                    var duration = monitorSystemExecutionDuration(system);
                    _totalDuration += duration;
                    systemInfo.AddExecutionDuration(duration);
                }
            }

            updateName();
        }

        double monitorSystemInitializeDuration(IInitializeSystem system) {
            _stopwatch.Reset();
            _stopwatch.Start();
            system.Initialize();
            _stopwatch.Stop();
            return _stopwatch.Elapsed.TotalMilliseconds;
        }

        double monitorSystemExecutionDuration(IExecuteSystem system) {
            _stopwatch.Reset();
            _stopwatch.Start();
            system.Execute();
            _stopwatch.Stop();
            return _stopwatch.Elapsed.TotalMilliseconds;
        }

        void updateName() {
            if (_container != null) {
                _container.name = string.Format("{0} ({1} init, {2} exe, {3:0.###} ms)",
                    _name, _initializeSystems.Count, _executeSystems.Count, _totalDuration);
            }
        }
    }
}