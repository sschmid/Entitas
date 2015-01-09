using System;

namespace Entitas.CodeGenerator {
    [AttributeUsage(AttributeTargets.Class)]
    public class ContextAttribute : Attribute {
        public string tag;

        public ContextAttribute(string tag) {
            this.tag = tag;
        }
    }
}