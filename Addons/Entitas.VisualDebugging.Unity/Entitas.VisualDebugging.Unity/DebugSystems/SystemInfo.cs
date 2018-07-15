using System;

namespace Entitas.VisualDebugging.Unity {

    [Flags]
    public enum SystemInterfaceFlags {
        None = 0,
        IInitializeSystem = 1 << 1,
        IExecuteSystem    = 1 << 2,
        ICleanupSystem    = 1 << 3,
        ITearDownSystem   = 1 << 4,
        IReactiveSystem   = 1 << 5
    }

    public class SystemInfo {

        public ISystem system { get { return _system; } }
        public string systemName { get { return _systemName; } }

        public bool isInitializeSystems { get { return (_interfaceFlags & SystemInterfaceFlags.IInitializeSystem) == SystemInterfaceFlags.IInitializeSystem; } }
        public bool isExecuteSystems { get { return (_interfaceFlags & SystemInterfaceFlags.IExecuteSystem) == SystemInterfaceFlags.IExecuteSystem; } }
        public bool isCleanupSystems { get { return (_interfaceFlags & SystemInterfaceFlags.ICleanupSystem) == SystemInterfaceFlags.ICleanupSystem; } }
        public bool isTearDownSystems { get { return (_interfaceFlags & SystemInterfaceFlags.ITearDownSystem) == SystemInterfaceFlags.ITearDownSystem; } }
        public bool isReactiveSystems { get { return (_interfaceFlags & SystemInterfaceFlags.IReactiveSystem) == SystemInterfaceFlags.IReactiveSystem; } }

        public double initializationDuration { get; set; }

        public double accumulatedExecutionDuration { get { return _accumulatedExecutionDuration; } }
        public double minExecutionDuration { get { return _minExecutionDuration; } }
        public double maxExecutionDuration { get { return _maxExecutionDuration; } }
        public double averageExecutionDuration { get { return _executionDurationsCount == 0 ? 0 : _accumulatedExecutionDuration / _executionDurationsCount; } }

        public double accumulatedCleanupDuration { get { return _accumulatedCleanupDuration; } }
        public double minCleanupDuration { get { return _minCleanupDuration; } }
        public double maxCleanupDuration { get { return _maxCleanupDuration; } }
        public double averageCleanupDuration { get { return _cleanupDurationsCount == 0 ? 0 : _accumulatedCleanupDuration / _cleanupDurationsCount; } }

        public double cleanupDuration { get; set; }
        public double teardownDuration { get; set; }

        public bool areAllParentsActive { get { return parentSystemInfo == null || (parentSystemInfo.isActive && parentSystemInfo.areAllParentsActive); } }

        public SystemInfo parentSystemInfo;
        public bool isActive;

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

        public SystemInfo(ISystem system) {
            _system = system;
            _interfaceFlags = getInterfaceFlags(system);

            var debugSystem = system as DebugSystems;
            _systemName = debugSystem != null
                ? debugSystem.name
                : system.GetType().Name.RemoveSystemSuffix();

            isActive = true;
        }

        public void AddExecutionDuration(double executionDuration) {
            if (executionDuration < _minExecutionDuration || _minExecutionDuration == 0) {
                _minExecutionDuration = executionDuration;
            }
            if (executionDuration > _maxExecutionDuration) {
                _maxExecutionDuration = executionDuration;
            }

            _accumulatedExecutionDuration += executionDuration;
            _executionDurationsCount += 1;
        }

        public void AddCleanupDuration(double cleanupDuration) {
            if (cleanupDuration < _minCleanupDuration || _minCleanupDuration == 0) {
                _minCleanupDuration = cleanupDuration;
            }
            if (cleanupDuration > _maxCleanupDuration) {
                _maxCleanupDuration = cleanupDuration;
            }

            _accumulatedCleanupDuration += cleanupDuration;
            _cleanupDurationsCount += 1;
        }

        public void ResetDurations() {
            _accumulatedExecutionDuration = 0;
            _executionDurationsCount = 0;

            _accumulatedCleanupDuration = 0;
            _cleanupDurationsCount = 0;
        }

        static SystemInterfaceFlags getInterfaceFlags(ISystem system) {
            var flags = SystemInterfaceFlags.None;
            if (system is IInitializeSystem) {
                flags |= SystemInterfaceFlags.IInitializeSystem;
            }
            if (system is IReactiveSystem) {
                flags |= SystemInterfaceFlags.IReactiveSystem;
            } else if (system is IExecuteSystem) {
                flags |= SystemInterfaceFlags.IExecuteSystem;
            }
            if (system is ICleanupSystem) {
                flags |= SystemInterfaceFlags.ICleanupSystem;
            }
            if (system is ITearDownSystem) {
                flags |= SystemInterfaceFlags.ITearDownSystem;
            }

            return flags;
        }
    }
}
