using System;

namespace Entitas.Unity
{
    [Flags]
    public enum SystemInterfaceFlags
    {
        None = 0,
        InitializeSystem = 1 << 1,
        ExecuteSystem = 1 << 2,
        CleanupSystem = 1 << 3,
        TearDownSystem = 1 << 4,
        ReactiveSystem = 1 << 5
    }
}
