using System;

namespace Entitas.Generators.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CleanupAttribute : Attribute
    {
        public readonly CleanupMode CleanupMode;

        public CleanupAttribute(CleanupMode cleanupMode)
        {
            CleanupMode = cleanupMode;
        }
    }

    public enum CleanupMode
    {
        RemoveComponent,
        DestroyEntity
    }
}
