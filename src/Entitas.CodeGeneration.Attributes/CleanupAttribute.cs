using System;

namespace Entitas.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class CleanupAttribute : Attribute
    {
        public readonly CleanupMode Mode;

        public CleanupAttribute(CleanupMode mode)
        {
            Mode = mode;
        }
    }

    public enum CleanupMode
    {
        RemoveComponent,
        DestroyEntity
    }
}
