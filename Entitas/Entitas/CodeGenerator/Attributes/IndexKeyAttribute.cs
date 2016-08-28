using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class IndexKeyAttribute : Attribute {

        public readonly string key;

        public IndexKeyAttribute(string key) {
            this.key = key;
        }
    }
}
