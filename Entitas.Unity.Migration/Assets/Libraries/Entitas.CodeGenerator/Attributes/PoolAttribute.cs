using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PoolAttribute : Attribute {
        public string tag;

        public PoolAttribute(string tag) {
            this.tag = tag;
        }
    }
}