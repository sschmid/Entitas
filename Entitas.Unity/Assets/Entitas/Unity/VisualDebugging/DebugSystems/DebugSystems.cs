using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    public enum AvgResetInterval {
        Always = 1,
        VeryFast = 30,
        Fast = 60,
        Normal = 120,
        Slow = 300,
        Never = int.MaxValue
    }

    public class DebugSystems : Systems {
        public static AvgResetInterval avgResetInterval = AvgResetInterval.Never;

        public int totalInitializeSystemsCount { 
            get {
                var total = 0;
                foreach (var system in _initializeSystems) {
                    var debugSystems = system as DebugSystems;
                    if (debugSystems != null) {
                        total += debugSystems.totalInitializeSystemsCount;
                    } else {
                        total += 1;
                    }
                }
                return total;
            }
        }

        public int totalExecuteSystemsCount {
            get {
                var total = 0;
                foreach (var system in _executeSystems) {
                    var debugSystems = system as DebugSystems;
                    if (debugSystems != null) {
                        total += debugSystems.totalExecuteSystemsCount;
                    } else {
                        total += 1;
                    }
                }
                return total;
            }
        }

        public int initializeSystemsCount { get { return _initializeSystems.Count; } }
        public int executeSystemsCount { get { return _executeSystems.Count; } }
        public int totalSystemsCount { get { return _systems.Count; } }

        public string name { get { return _name; } }
        public GameObject container { get { return _container.gameObject; } }
        public double totalDuration { get { return _totalDuration; } }
        public SystemInfo[] initializeSystemInfos { get { return _initializeSystemInfos.ToArray(); } }
        public SystemInfo[] executeSystemInfos { get { return _executeSystemInfos.ToArray(); } }

        public bool paused;

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
                _initializeSystemInfos.Add(systemInfo);
            }
            if (systemInfo.isExecuteSystems || systemInfo.isReactiveSystems) {
                _executeSystemInfos.Add(systemInfo);
            }

            return base.Add(system);
        }

        public void ResetDurations() {
            foreach (var systemInfo in _initializeSystemInfos) {
                systemInfo.ResetDurations();
            }
            foreach (var systemInfo in _executeSystemInfos) {
                systemInfo.ResetDurations();
                var debugSystems = systemInfo.system as DebugSystems;
                if (debugSystems != null) {
                    debugSystems.ResetDurations();
                }
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
                ResetDurations();
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