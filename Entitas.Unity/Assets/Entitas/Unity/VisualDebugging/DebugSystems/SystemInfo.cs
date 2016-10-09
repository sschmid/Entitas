using System;

namespace Entitas.Unity.VisualDebugging {

    [Flags]
    public enum SystemInterfaceFlags {
        None              = 0,
        IInitializeSystem = 1 << 1,
        IExecuteSystem    = 1 << 2,
        ICleanupSystem    = 1 << 3,
        ITearDownSystem   = 1 << 4,
        IReactiveSystem   = 1 << 5
    }

    public class SystemInfo {

        public ISystem system { get { return _system; } }
        public string systemName { get { return _systemName; } }

        public bool isInitializeSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.IInitializeSystem) == SystemInterfaceFlags.IInitializeSystem; }
        }

        public bool isExecuteSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.IExecuteSystem) == SystemInterfaceFlags.IExecuteSystem; }
        }

        public bool isCleanupSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.ICleanupSystem) == SystemInterfaceFlags.ICleanupSystem; }
        }

        public bool isTearDownSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.ITearDownSystem) == SystemInterfaceFlags.ITearDownSystem; }
        }

        public bool isReactiveSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.IReactiveSystem) == SystemInterfaceFlags.IReactiveSystem; }
        }

        public double accumulatedDuration { get { return _accumulatedExecutionDuration; } }
        public double minDuration { get { return _minDuration; } }
        public double maxDuration { get { return _maxDuration; } }
        public double averageExecutionDuration {
            get { return _durationsCount == 0 ? 0 : _accumulatedExecutionDuration / _durationsCount; }
        }

        public bool isActive;

        readonly ISystem _system;
        readonly SystemInterfaceFlags _interfaceFlags;
        readonly string _systemName;

        double _accumulatedExecutionDuration;
        double _minDuration;
        double _maxDuration;
        int _durationsCount;

        const string SYSTEM_SUFFIX = "System";

        public SystemInfo(ISystem system) {
            _system = system;

            var reactiveSystem = system as ReactiveSystem;
            var isReactive = reactiveSystem != null;
            Type systemType;
            if(isReactive) {
                _interfaceFlags = getInterfaceFlags(reactiveSystem.subsystem, isReactive);
                systemType = reactiveSystem.subsystem.GetType();
            } else {
                _interfaceFlags = getInterfaceFlags(system, isReactive);
                systemType = system.GetType();
            }

            var debugSystem = system as DebugSystems;
            if(debugSystem != null) {
                _systemName = debugSystem.name;
            } else {
                _systemName = systemType.Name.EndsWith(SYSTEM_SUFFIX, StringComparison.Ordinal)
                    ? systemType.Name.Substring(0, systemType.Name.Length - SYSTEM_SUFFIX.Length)
                    : systemType.Name;
            }

            isActive = true;
        }

        public void AddExecutionDuration(double executionDuration) {
            if(executionDuration < _minDuration || _minDuration == 0) {
                _minDuration = executionDuration;
            }
            if(executionDuration > _maxDuration) {
                _maxDuration = executionDuration;
            }

            _accumulatedExecutionDuration += executionDuration;
            _durationsCount += 1;
        }

        public void ResetDurations() {
            _accumulatedExecutionDuration = 0;
            _durationsCount = 0;
        }

        static SystemInterfaceFlags getInterfaceFlags(ISystem system, bool isReactive) {
            var flags = SystemInterfaceFlags.None;
            if(system is IInitializeSystem) {
                flags |= SystemInterfaceFlags.IInitializeSystem;
            }
            if(system is IExecuteSystem) {
                flags |= SystemInterfaceFlags.IExecuteSystem;
            }
            if(system is ICleanupSystem) {
                flags |= SystemInterfaceFlags.ICleanupSystem;
            }
            if(system is ITearDownSystem) {
                flags |= SystemInterfaceFlags.ITearDownSystem;
            }
            if(isReactive) {
                flags |= SystemInterfaceFlags.IReactiveSystem;
            }

            return flags;
        }
    }
}
