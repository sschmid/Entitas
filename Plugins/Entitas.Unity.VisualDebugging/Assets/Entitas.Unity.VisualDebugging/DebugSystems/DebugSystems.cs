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
                foreach(var system in _initializeSystems) {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalInitializeSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalExecuteSystemsCount {
            get {
                var total = 0;
                foreach(var system in _executeSystems) {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalExecuteSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalCleanupSystemsCount {
            get {
                var total = 0;
                foreach(var system in _cleanupSystems) {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalCleanupSystemsCount : 1;
                }
                return total;
            }
        }

        public int totalTearDownSystemsCount {
            get {
                var total = 0;
                foreach(var system in _tearDownSystems) {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalTearDownSystemsCount : 1;
                }
                return total;
            }
        }

		public int totalSystemsCount {
            get {
                var total = 0;
                foreach(var system in _systems) {
                    var debugSystems = system as DebugSystems;
                    total += debugSystems != null ? debugSystems.totalSystemsCount : 1;
                }
                return total;
            }
        }

        public int initializeSystemsCount { get { return _initializeSystems.Count; } }
        public int executeSystemsCount { get { return _executeSystems.Count; } }
        public int cleanupSystemsCount { get { return _cleanupSystems.Count; } }
        public int tearDownSystemsCount { get { return _tearDownSystems.Count; } }

        public string name { get { return _name; } }
        public GameObject gameObject { get { return _gameObject; } }

        public double executeDuration { get { return _executeDuration; } }

        public SystemInfo[] initializeSystemInfos { get { return _initializeSystemInfos.ToArray(); } }
        public SystemInfo[] executeSystemInfos { get { return _executeSystemInfos.ToArray(); } }
        public SystemInfo[] cleanupSystemInfos { get { return _cleanupSystemInfos.ToArray(); } }
        public SystemInfo[] tearDownSystemInfos { get { return _tearDownSystemInfos.ToArray(); } }

        public bool paused;

        readonly string _name;

        readonly List<ISystem> _systems;
        readonly GameObject _gameObject;
        readonly List<SystemInfo> _initializeSystemInfos;
        readonly List<SystemInfo> _executeSystemInfos;
        readonly List<SystemInfo> _cleanupSystemInfos;
        readonly List<SystemInfo> _tearDownSystemInfos;

        readonly Stopwatch _stopwatch;

        double _executeDuration;

        public DebugSystems(string name = "Systems") {
            _name = name;
            _gameObject = new GameObject(name);
            _gameObject.AddComponent<DebugSystemsBehaviour>().Init(this);

			_systems = new List<ISystem>();
            _initializeSystemInfos = new List<SystemInfo>();
            _executeSystemInfos = new List<SystemInfo>();
            _cleanupSystemInfos = new List<SystemInfo>();
            _tearDownSystemInfos = new List<SystemInfo>();

            _stopwatch = new Stopwatch();
        }

        public override Systems Add(ISystem system) {
            _systems.Add(system);
            var debugSystems = system as DebugSystems;
            if(debugSystems != null) {
                debugSystems.gameObject.transform.SetParent(_gameObject.transform, false);
            }
            var systemInfo = new SystemInfo(system);
            if(systemInfo.isInitializeSystems) {
                _initializeSystemInfos.Add(systemInfo);
            }
            if(systemInfo.isExecuteSystems || systemInfo.isReactiveSystems) {
                _executeSystemInfos.Add(systemInfo);
            }
            if(systemInfo.isCleanupSystems) {
                _cleanupSystemInfos.Add(systemInfo);
            }
            if(systemInfo.isTearDownSystems) {
                _tearDownSystemInfos.Add(systemInfo);
            }

            return base.Add(system);
        }

        public void ResetDurations() {
            foreach(var systemInfo in _executeSystemInfos) {
                systemInfo.ResetDurations();
            }

            foreach(var system in _systems) {
                var debugSystems = system as DebugSystems;
                if(debugSystems != null) {
                    debugSystems.ResetDurations();
                }
            }
        }

        public override void Initialize() {
            for (int i = 0; i < _initializeSystems.Count; i++) {
                if(_initializeSystemInfos[i].isActive) {
                    _initializeSystems[i].Initialize();
                }
            }
        }

        public override void Execute() {
            if(!paused) {
                StepExecute();
            }
        }

        public override void Cleanup() {
            if(!paused) {
                StepCleanup();
            }
        }

        public void StepExecute() {
            _executeDuration = 0;
            if(Time.frameCount % (int)avgResetInterval == 0) {
                ResetDurations();
            }
            for (int i = 0; i < _executeSystems.Count; i++) {
                var systemInfo = _executeSystemInfos[i];
                if(systemInfo.isActive) {
                    var duration = monitorSystemExecutionDuration(_executeSystems[i]);
                    _executeDuration += duration;
                    systemInfo.AddExecutionDuration(duration);
                }
            }
        }

        public void StepCleanup() {
            for (int i = 0; i < _cleanupSystems.Count; i++) {
                if(_cleanupSystemInfos[i].isActive) {
                    _cleanupSystems[i].Cleanup();
                }
            }
        }

        public override void TearDown() {
            for (int i = 0; i < _tearDownSystems.Count; i++) {
                if(_tearDownSystemInfos[i].isActive) {
                    _tearDownSystems[i].TearDown();
                }
            }
        }

        double monitorSystemExecutionDuration(IExecuteSystem system) {
            _stopwatch.Reset();
            _stopwatch.Start();
            system.Execute();
            _stopwatch.Stop();
            return _stopwatch.Elapsed.TotalMilliseconds;
        }
    }
}
