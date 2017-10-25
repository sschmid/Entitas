using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
    public class UniquePrefixAttribute : Attribute {

        public readonly string prefix;

        public UniquePrefixAttribute(string prefix) {
            this.prefix = prefix;
        }
    }
}
