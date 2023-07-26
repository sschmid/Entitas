using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using UnityEngine;

namespace Entitas.Unity
{
    public class DebugSystems : Systems
    {
        public static AvgResetInterval AvgResetInterval = AvgResetInterval.Never;

        public int TotalInitializeSystemsCount => _initializeSystems.Sum(system => system is DebugSystems debugSystems ? debugSystems.TotalInitializeSystemsCount : 1);
        public int TotalExecuteSystemsCount => _executeSystems.Sum(system => system is DebugSystems debugSystems ? debugSystems.TotalExecuteSystemsCount : 1);
        public int TotalCleanupSystemsCount => _cleanupSystems.Sum(system => system is DebugSystems debugSystems ? debugSystems.TotalCleanupSystemsCount : 1);
        public int TotalTearDownSystemsCount => _tearDownSystems.Sum(system => system is DebugSystems debugSystems ? debugSystems.TotalTearDownSystemsCount : 1);
        public int TotalSystemsCount => AllSystems.Sum(system => system is DebugSystems debugSystems ? debugSystems.TotalSystemsCount : 1);

        public int InitializeSystemsCount => _initializeSystems.Count;
        public int ExecuteSystemsCount => _executeSystems.Count;
        public int CleanupSystemsCount => _cleanupSystems.Count;
        public int TearDownSystemsCount => _tearDownSystems.Count;

        public string Name => _name;
        public GameObject GameObject => _gameObject;

        public SystemInfo SystemInfo => _systemInfo;
        public double ExecuteDuration => _executeDuration;
        public double CleanupDuration => _cleanupDuration;

        public readonly List<SystemInfo> InitializeSystemInfos = new List<SystemInfo>();
        public readonly List<SystemInfo> ExecuteSystemInfos = new List<SystemInfo>();
        public readonly List<SystemInfo> CleanupSystemInfos = new List<SystemInfo>();
        public readonly List<SystemInfo> TearDownSystemInfos = new List<SystemInfo>();
        public readonly List<ISystem> AllSystems = new List<ISystem>();

        public bool IsPaused;

        string _name;
        GameObject _gameObject;
        SystemInfo _systemInfo;
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
            _gameObject.AddComponent<DebugSystemsBehaviour>().Initialize(this);
            _systemInfo = new SystemInfo(this);
            _stopwatch = new Stopwatch();
        }

        public override Systems Add(ISystem system)
        {
            AllSystems.Add(system);

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

            childSystemInfo.ParentSystemInfo = _systemInfo;

            if (childSystemInfo.IsInitializeSystems) InitializeSystemInfos.Add(childSystemInfo);
            if (childSystemInfo.IsExecuteSystems || childSystemInfo.IsReactiveSystems) ExecuteSystemInfos.Add(childSystemInfo);
            if (childSystemInfo.IsCleanupSystems) CleanupSystemInfos.Add(childSystemInfo);
            if (childSystemInfo.IsTearDownSystems) TearDownSystemInfos.Add(childSystemInfo);

            return base.Add(system);
        }

        public void ResetDurations()
        {
            foreach (var systemInfo in ExecuteSystemInfos)
                systemInfo.ResetDurations();

            foreach (var system in AllSystems)
                if (system is DebugSystems debugSystems)
                    debugSystems.ResetDurations();
        }

        public override void Initialize()
        {
            for (var i = 0; i < _initializeSystems.Count; i++)
            {
                var systemInfo = InitializeSystemInfos[i];
                if (systemInfo.IsActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _initializeSystems[i].Initialize();
                    _stopwatch.Stop();
                    systemInfo.InitializationDuration = _stopwatch.Elapsed.TotalMilliseconds;
                }
            }
        }

        public override void Execute()
        {
            if (!IsPaused)
                StepExecute();
        }

        public override void Cleanup()
        {
            if (!IsPaused)
                StepCleanup();
        }

        public void StepExecute()
        {
            var executeDuration = 0d;
            if (Time.frameCount % (int)AvgResetInterval == 0)
                ResetDurations();

            for (var i = 0; i < _executeSystems.Count; i++)
            {
                var systemInfo = ExecuteSystemInfos[i];
                if (systemInfo.IsActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _executeSystems[i].Execute();
                    _stopwatch.Stop();
                    var duration = _stopwatch.Elapsed.TotalMilliseconds;
                    executeDuration += duration;
                    systemInfo.AddExecutionDuration(duration);
                }
            }

            _executeDuration = executeDuration;
        }

        public void StepCleanup()
        {
            var cleanupDuration = 0d;
            for (var i = 0; i < _cleanupSystems.Count; i++)
            {
                var systemInfo = CleanupSystemInfos[i];
                if (systemInfo.IsActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _cleanupSystems[i].Cleanup();
                    _stopwatch.Stop();
                    var duration = _stopwatch.Elapsed.TotalMilliseconds;
                    cleanupDuration += duration;
                    systemInfo.AddCleanupDuration(duration);
                }
            }

            _cleanupDuration = cleanupDuration;
        }

        public override void TearDown()
        {
            for (var i = 0; i < _tearDownSystems.Count; i++)
            {
                var systemInfo = TearDownSystemInfos[i];
                if (systemInfo.IsActive)
                {
                    _stopwatch.Reset();
                    _stopwatch.Start();
                    _tearDownSystems[i].TearDown();
                    _stopwatch.Stop();
                    systemInfo.TeardownDuration = _stopwatch.Elapsed.TotalMilliseconds;
                }
            }
        }
    }
}
