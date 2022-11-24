using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Entitas.Unity
{
    public enum AvgResetInterval
    {
        Always = 1,
        VeryFast = 30,
        Fast = 60,
        Normal = 120,
        Slow = 300,
        Never = int.MaxValue
    }

    public class DebugSystems : Systems
    {
        public static AvgResetInterval AvgResetInterval = AvgResetInterval.Never;

        public int TotalInitializeSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _initializeSystems)
                    total += system is DebugSystems debugSystems ? debugSystems.TotalInitializeSystemsCount : 1;

                return total;
            }
        }

        public int TotalExecuteSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _executeSystems)
                    total += system is DebugSystems debugSystems ? debugSystems.TotalExecuteSystemsCount : 1;

                return total;
            }
        }

        public int TotalCleanupSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _cleanupSystems)
                    total += system is DebugSystems debugSystems ? debugSystems.TotalCleanupSystemsCount : 1;

                return total;
            }
        }

        public int TotalTearDownSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _tearDownSystems)
                    total += system is DebugSystems debugSystems ? debugSystems.TotalTearDownSystemsCount : 1;

                return total;
            }
        }

        public int TotalSystemsCount
        {
            get
            {
                var total = 0;
                foreach (var system in _systems)
                    total += system is DebugSystems debugSystems ? debugSystems.TotalSystemsCount : 1;

                return total;
            }
        }

        public int InitializeSystemsCount => _initializeSystems.Count;
        public int ExecuteSystemsCount => _executeSystems.Count;
        public int CleanupSystemsCount => _cleanupSystems.Count;
        public int TearDownSystemsCount => _tearDownSystems.Count;

        public string Name => _name;
        public GameObject GameObject => _gameObject;
        public SystemInfo SystemInfo => _systemInfo;
        public double ExecuteDuration => _executeDuration;
        public double CleanupDuration => _cleanupDuration;

        public SystemInfo[] InitializeSystemInfos => _initializeSystemInfos.ToArray();
        public SystemInfo[] ExecuteSystemInfos => _executeSystemInfos.ToArray();
        public SystemInfo[] CleanupSystemInfos => _cleanupSystemInfos.ToArray();
        public SystemInfo[] TearDownSystemInfos => _tearDownSystemInfos.ToArray();

        public bool Paused;

        string _name;

        List<ISystem> _systems;
        GameObject _gameObject;
        SystemInfo _systemInfo;

        List<SystemInfo> _initializeSystemInfos;
        List<SystemInfo> _executeSystemInfos;
        List<SystemInfo> _cleanupSystemInfos;
        List<SystemInfo> _tearDownSystemInfos;

        Stopwatch _stopwatch;

        double _executeDuration;
        double _cleanupDuration;

        public DebugSystems(string name)
        {
            Initialize(name);
        }

        protected DebugSystems(bool noInit) { }

        protected void Initialize(string name)
        {
            _name = name;
            _gameObject = new GameObject(name);
            _gameObject.AddComponent<DebugSystemsBehaviour>().Init(this);

            _systemInfo = new SystemInfo(this);

            _systems = new List<ISystem>();
            _initializeSystemInfos = new List<SystemInfo>();
            _executeSystemInfos = new List<SystemInfo>();
            _cleanupSystemInfos = new List<SystemInfo>();
            _tearDownSystemInfos = new List<SystemInfo>();

            _stopwatch = new Stopwatch();
        }

        public override Systems Add(ISystem system)
        {
            _systems.Add(system);

            SystemInfo childSystemInfo;

            if (system is DebugSystems debugSystems)
            {
                childSystemInfo = debugSystems.SystemInfo;
                debugSystems.GameObject.transform.SetParent(_gameObject.transform, false);
            }
            else
            {
                childSystemInfo = new SystemInfo(system);
            }

            childSystemInfo.parentSystemInfo = _systemInfo;

            if (childSystemInfo.isInitializeSystems) _initializeSystemInfos.Add(childSystemInfo);
            if (childSystemInfo.isExecuteSystems || childSystemInfo.isReactiveSystems) _executeSystemInfos.Add(childSystemInfo);
            if (childSystemInfo.isCleanupSystems) _cleanupSystemInfos.Add(childSystemInfo);
            if (childSystemInfo.isTearDownSystems) _tearDownSystemInfos.Add(childSystemInfo);

            return base.Add(system);
        }

        public void ResetDurations()
        {
            foreach (var systemInfo in _executeSystemInfos)
                systemInfo.ResetDurations();

            foreach (var system in _systems)
                if (system is DebugSystems debugSystems)
                    debugSystems.ResetDurations();
        }

        public override void Initialize()
        {
            for (var i = 0; i < _initializeSystems.Count; i++)
            {
                var systemInfo = _initializeSystemInfos[i];
                if (systemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _initializeSystems[i].Initialize();
                    _stopwatch.Stop();
                    systemInfo.initializationDuration = _stopwatch.Elapsed.TotalMilliseconds;
                }
            }
        }

        public override void Execute()
        {
            if (!Paused)
                StepExecute();
        }

        public override void Cleanup()
        {
            if (!Paused)
                StepCleanup();
        }

        public void StepExecute()
        {
            _executeDuration = 0;
            if (Time.frameCount % (int)AvgResetInterval == 0)
                ResetDurations();

            for (var i = 0; i < _executeSystems.Count; i++)
            {
                var systemInfo = _executeSystemInfos[i];
                if (systemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _executeSystems[i].Execute();
                    _stopwatch.Stop();
                    var duration = _stopwatch.Elapsed.TotalMilliseconds;
                    _executeDuration += duration;
                    systemInfo.AddExecutionDuration(duration);
                }
            }
        }

        public void StepCleanup()
        {
            _cleanupDuration = 0;
            for (var i = 0; i < _cleanupSystems.Count; i++)
            {
                var systemInfo = _cleanupSystemInfos[i];
                if (systemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _cleanupSystems[i].Cleanup();
                    _stopwatch.Stop();
                    var duration = _stopwatch.Elapsed.TotalMilliseconds;
                    _cleanupDuration += duration;
                    systemInfo.AddCleanupDuration(duration);
                }
            }
        }

        public override void TearDown()
        {
            for (var i = 0; i < _tearDownSystems.Count; i++)
            {
                var systemInfo = _tearDownSystemInfos[i];
                if (systemInfo.isActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _tearDownSystems[i].TearDown();
                    _stopwatch.Stop();
                    systemInfo.teardownDuration = _stopwatch.Elapsed.TotalMilliseconds;
                }
            }
        }
    }
}
