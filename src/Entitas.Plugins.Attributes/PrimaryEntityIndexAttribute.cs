using System;

namespace Entitas.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Class)]
    public class PrimaryEntityIndexAttribute : AbstractEntityIndexAttribute
    {
        public PrimaryEntityIndexAttribute() : base(EntityIndexType.PrimaryEntityIndex) { }
    }
}
