using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class PoolAttribute : Attribute {
        public string poolName;

        public PoolAttribute(string poolName) {
            this.poolName = poolName;
        }
    }
}