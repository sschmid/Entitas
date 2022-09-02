using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class ComponentNameAttribute : Attribute {

        public readonly string[] componentNames;

        public ComponentNameAttribute(params string[] componentNames) {
            this.componentNames = componentNames;
        }
    }
}

namespace Entitas.CodeGeneration.Attributes {

    [Obsolete("Use [ComponentName] instead", false)]
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class CustomComponentNameAttribute : Attribute {

        public readonly string[] componentNames;

        public CustomComponentNameAttribute(params string[] componentNames) {
            this.componentNames = componentNames;
        }
    }
}
