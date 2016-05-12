using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = true)]
    public class PoolAttribute : Attribute {
        public readonly string poolName;

        public PoolAttribute(string poolName = "") {
            this.poolName = poolName;
        }
    }
}