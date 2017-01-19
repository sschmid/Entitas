using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class ContextAttribute : Attribute {

        public readonly string contextName;

        public ContextAttribute(string contextName) {
            this.contextName = contextName;
        }
    }
}
