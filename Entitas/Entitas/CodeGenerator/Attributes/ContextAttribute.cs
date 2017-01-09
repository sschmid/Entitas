using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class ContextAttribute : Attribute {

        public readonly string contextName;

        // TODO Try to use this again as soon Unity updates to newer mono
        //public ContextAttribute(string contextName = CodeGenerator.DEFAULT_CONTEXT_NAME) {
        public ContextAttribute(string contextName = "Context") {
            this.contextName = contextName;
        }
    }
}
