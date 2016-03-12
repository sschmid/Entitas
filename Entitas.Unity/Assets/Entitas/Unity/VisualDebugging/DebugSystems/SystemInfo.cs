using System;
using Entitas;
using Entitas.Unity.VisualDebugging;

namespace Entitas.Unity.VisualDebugging {
    public class SystemInfo {
        public ISystem system { get { return _system; } }
        public string systemName { get { return _systemName; } }

        public bool isInitializeSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.IInitializeSystem) == SystemInterfaceFlags.IInitializeSystem; }
        }

        public bool isExecuteSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.IExecuteSystem) == SystemInterfaceFlags.IExecuteSystem; }
        }

        public bool isReactiveSystems {
            get { return (_interfaceFlags & SystemInterfaceFlags.IReactiveSystem) == SystemInterfaceFlags.IReactiveSystem; }
        }

        public double accumulatedExecutionDuration { get { return _accumulatedExecutionDuration; } }
        public double minExecutionDuration { get { return _minExecutionDuration; } }
        public double maxExecutionDuration { get { return _maxExecutionDuration; } }
        public double averageExecutionDuration {
            get { return _durationsCount == 0 ? 0 : _accumulatedExecutionDuration / _durationsCount; }
        }

        public bool isActive;

        readonly ISystem _system;
        readonly SystemInterfaceFlags _interfaceFlags;
        readonly string _systemName;

        double _accumulatedExecutionDuration;
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
            if (executionDuration < _minExecutionDuration || _minExecutionDuration == 0) {
                _minExecutionDuration = executionDuration;
            }
            if (executionDuration > _maxExecutionDuration) {
                _maxExecutionDuration = executionDuration;
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

        [Flags]
        enum SystemInterfaceFlags {
            None = 0,
            IInitializeSystem = 1,
            IExecuteSystem = 2,
            IReactiveSystem = 4
        }
    }
}