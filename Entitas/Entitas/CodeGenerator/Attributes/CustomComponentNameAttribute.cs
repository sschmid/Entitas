using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class CustomComponentNameAttribute : Attribute {
        public readonly string componentName;

        public CustomComponentNameAttribute(string componentName) {
            this.componentName = componentName;
        }
    }
}
