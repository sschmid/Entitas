using System;

namespace Entitas.CodeGeneration {

    [AttributeUsage(AttributeTargets.Class)]
    public class CustomPrefixAttribute : Attribute {
        public readonly string prefix;

        public CustomPrefixAttribute(string prefix) {
            this.prefix = prefix;
        }
    }
}