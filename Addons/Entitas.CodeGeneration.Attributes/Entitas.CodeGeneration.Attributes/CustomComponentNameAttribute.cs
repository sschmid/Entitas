using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class CustomComponentNameAttribute : Attribute {

        public readonly string[] componentNames;

        public CustomComponentNameAttribute(params string[] componentNames) {
            this.componentNames = componentNames;
        }
    }
}
