using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    public class SystemInfo {
        public string systemName { get { return _systemName; } }
        public double totalExecutionDuration { get { return _totalExecutionDuration; } }
        public double minExecutionDuration { get { return _minExecutionDuration; } }
        public double maxExecutionDuration { get { return _maxExecutionDuration; } }
        public double averageExecutionDuration {
            get { return _durationsCount == 0 ? 0 : _totalExecutionDuration / _durationsCount; }
        }

        public bool isActive;

        string _systemName;
        double _totalExecutionDuration = -1;
        double _minExecutionDuration;
        double _maxExecutionDuration;
        int _durationsCount;

        public SystemInfo(string systemName) {
            _systemName = systemName;
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
    }

    public enum AvgResetInterval {
        EveryFrame = 1,
        Every30Frames = 30,
        Every60Frames = 60,
        Every120Frames = 120,
        Every300Frames = 300,
        Never = int.MaxValue
    }

    public class DebugSystems : Systems {
        public string name { get { return _name; } }
        public GameObject container { get { return _container.gameObject; } }
        public double totalDuration { get { return _totalDuration; } }
        public SystemInfo[] systemInfos {
            get { return _systemInfos.Values
                    .Where(systemInfo => systemInfo.averageExecutionDuration >= threshold)
                    .ToArray(); }
        }

        public bool paused;

        public float threshold;
        public AvgResetInterval avgResetInterval = AvgResetInterval.Never;

        readonly string _name;
        readonly Transform _container;
        double _totalDuration;
        Dictionary<Type, SystemInfo> _systemInfos;
        Stopwatch _stopwatch;

        public DebugSystems(string name = "Debug Systems") {
            _name = name;
            _container = new GameObject().transform;
            _container.gameObject.AddComponent<SystemsDebugBehaviour>().Init(this);
            _systemInfos = new Dictionary<Type, SystemInfo>();
            _stopwatch = new Stopwatch();
            updateName();
        }

        public void Reset() {
            foreach (var systemInfo in _systemInfos.Values) {
                systemInfo.Reset();
            }
        }

        public override void Start() {
            _totalDuration = 0;
            for (int i = 0, _startSystemsCount = _startSystems.Count; i < _startSystemsCount; i++) {
                var system = _startSystems[i];
                var systemInfo = getSystemInfo(system);
                if (systemInfo.isActive) {
                    var duration = monitorSystemStartDuration(system);
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
                var systemInfo = getSystemInfo(system);
                if (systemInfo.isActive) {
                    var duration = monitorSystemExecutionDuration(system);
                    _totalDuration += duration;
                    systemInfo.AddExecutionDuration(duration);
                }
            }

            updateName();
        }

        double monitorSystemStartDuration(IStartSystem system) {
            _stopwatch.Reset();
            _stopwatch.Start();
            system.Start();
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

        SystemInfo getSystemInfo(ISystem system) {
            var reactiveSystem = system as ReactiveSystem;
            var systemType = reactiveSystem != null
                                ? reactiveSystem.subsystem.GetType()
                                : system.GetType();

            SystemInfo systemInfo;
            if (!_systemInfos.TryGetValue(systemType, out systemInfo)) {
                const string systemSuffix = "System";
                var systemName = systemType.Name.EndsWith(systemSuffix, StringComparison.Ordinal)
                                    ? systemType.Name.Substring(0, systemType.Name.Length - systemSuffix.Length)
                                    : systemType.Name;

                systemInfo = new SystemInfo(systemName);
                _systemInfos.Add(systemType, systemInfo);
            }

            return systemInfo;
        }

        void updateName() {
            if (_container != null) {
                _container.name = string.Format("{0} ({1} start, {2} exe, {3:0.###} ms)",
                    _name, startSystemsCount, executeSystemsCount, _totalDuration);
            }
        }
    }
}