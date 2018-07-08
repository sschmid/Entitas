using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class CleanupAttribute : Attribute {

        public readonly CleanupMode cleanupMode;

        public CleanupAttribute(CleanupMode cleanupMode) {
            this.cleanupMode = cleanupMode;
        }
    }

    public enum CleanupMode {
        RemoveComponent,
        DestroyEntity
    }
}
