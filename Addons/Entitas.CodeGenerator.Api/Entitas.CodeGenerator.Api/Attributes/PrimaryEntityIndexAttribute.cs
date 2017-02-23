using System;

namespace Entitas.CodeGenerator.Api {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class PrimaryEntityIndexAttribute : AbstractEntityIndexAttribute {

        public PrimaryEntityIndexAttribute() : base("PrimaryEntityIndex", true) {
        }
    }
}
