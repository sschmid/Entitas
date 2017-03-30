using System;

namespace Entitas.CodeGenerator.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = true)]
    public class ContextAttribute : Attribute {

        public readonly string contextName;

        public ContextAttribute(string contextName) {
            this.contextName = contextName;
        }
    }
}
