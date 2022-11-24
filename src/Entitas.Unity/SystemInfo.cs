using System;

namespace Entitas.Unity
{
    [Flags]
    public enum SystemInterfaceFlags
    {
        None = 0,
        IInitializeSystem = 1 << 1,
        IExecuteSystem = 1 << 2,
        ICleanupSystem = 1 << 3,
        ITearDownSystem = 1 << 4,
        IReactiveSystem = 1 << 5
    }

    public class SystemInfo
    {
        public ISystem System => _system;
        public string SystemName => _systemName;

        public bool IsInitializeSystems => (_interfaceFlags & SystemInterfaceFlags.IInitializeSystem) == SystemInterfaceFlags.IInitializeSystem;
        public bool IsExecuteSystems => (_interfaceFlags & SystemInterfaceFlags.IExecuteSystem) == SystemInterfaceFlags.IExecuteSystem;
        public bool IsCleanupSystems => (_interfaceFlags & SystemInterfaceFlags.ICleanupSystem) == SystemInterfaceFlags.ICleanupSystem;
        public bool IsTearDownSystems => (_interfaceFlags & SystemInterfaceFlags.ITearDownSystem) == SystemInterfaceFlags.ITearDownSystem;
        public bool IsReactiveSystems => (_interfaceFlags & SystemInterfaceFlags.IReactiveSystem) == SystemInterfaceFlags.IReactiveSystem;

        public double InitializationDuration { get; set; }
        public double AccumulatedExecutionDuration => _accumulatedExecutionDuration;
        public double MinExecutionDuration => _minExecutionDuration;
        public double MaxExecutionDuration => _maxExecutionDuration;
        public double AverageExecutionDuration => _executionDurationsCount == 0 ? 0 : _accumulatedExecutionDuration / _executionDurationsCount;
        public double AccumulatedCleanupDuration => _accumulatedCleanupDuration;
        public double MinCleanupDuration => _minCleanupDuration;
        public double MaxCleanupDuration => _maxCleanupDuration;
        public double AverageCleanupDuration => _cleanupDurationsCount == 0 ? 0 : _accumulatedCleanupDuration / _cleanupDurationsCount;
        public double CleanupDuration { get; set; }
        public double TeardownDuration { get; set; }

        public bool AreAllParentsActive => ParentSystemInfo == null || (ParentSystemInfo.IsActive && ParentSystemInfo.AreAllParentsActive);

        public SystemInfo ParentSystemInfo;
        public bool IsActive;

        readonly ISystem _system;
        readonly SystemInterfaceFlags _interfaceFlags;
        readonly string _systemName;

        double _accumulatedExecutionDuration;
        double _minExecutionDuration;
        double _maxExecutionDuration;
        int _executionDurationsCount;

        double _accumulatedCleanupDuration;
        double _minCleanupDuration;
        double _maxCleanupDuration;
        int _cleanupDurationsCount;

        public SystemInfo(ISystem system)
        {
            _system = system;
            _interfaceFlags = GetInterfaceFlags(system);

            _systemName = system is DebugSystems debugSystem
                ? debugSystem.Name
                : system.GetType().Name.RemoveSystemSuffix();

            IsActive = true;
        }

        public void AddExecutionDuration(double executionDuration)
        {
            if (executionDuration < _minExecutionDuration || _minExecutionDuration == 0)
                _minExecutionDuration = executionDuration;

            if (executionDuration > _maxExecutionDuration)
                _maxExecutionDuration = executionDuration;

            _accumulatedExecutionDuration += executionDuration;
            _executionDurationsCount += 1;
        }

        public void AddCleanupDuration(double cleanupDuration)
        {
            if (cleanupDuration < _minCleanupDuration || _minCleanupDuration == 0)
                _minCleanupDuration = cleanupDuration;

            if (cleanupDuration > _maxCleanupDuration)
                _maxCleanupDuration = cleanupDuration;

            _accumulatedCleanupDuration += cleanupDuration;
            _cleanupDurationsCount += 1;
        }

        public void ResetDurations()
        {
            _accumulatedExecutionDuration = 0;
            _executionDurationsCount = 0;

            _accumulatedCleanupDuration = 0;
            _cleanupDurationsCount = 0;
        }

        static SystemInterfaceFlags GetInterfaceFlags(ISystem system)
        {
            var flags = SystemInterfaceFlags.None;
            if (system is IInitializeSystem) flags |= SystemInterfaceFlags.IInitializeSystem;
            if (system is IReactiveSystem) flags |= SystemInterfaceFlags.IReactiveSystem;
            else if (system is IExecuteSystem) flags |= SystemInterfaceFlags.IExecuteSystem;
            if (system is ICleanupSystem) flags |= SystemInterfaceFlags.ICleanupSystem;
            if (system is ITearDownSystem) flags |= SystemInterfaceFlags.ITearDownSystem;
            return flags;
        }
    }
}
