using System;

namespace Entitas.CodeGenerator.Attributes {

    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class PrimaryEntityIndexAttribute : AbstractEntityIndexAttribute {

        public PrimaryEntityIndexAttribute() : base(EntityIndexType.PrimaryEntityIndex) {
        }
    }
}
