using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class CustomEntityIndexAttribute : Attribute {

        public readonly Type contextType;

        public CustomEntityIndexAttribute(Type contextType) {
            this.contextType = contextType;
        }
    }
}
