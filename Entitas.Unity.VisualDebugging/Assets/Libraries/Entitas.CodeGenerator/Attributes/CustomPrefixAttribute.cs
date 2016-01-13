using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Class)]
    public class CustomPrefixAttribute : Attribute {
        public string prefix;

        public CustomPrefixAttribute(string prefix) {
            this.prefix = prefix;
        }
    }
}