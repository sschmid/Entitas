using System;

namespace Entitas.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class ComponentNamesAttribute : Attribute
    {
        public readonly string[] ComponentNames;

        public ComponentNamesAttribute(params string[] componentNames)
        {
            ComponentNames = componentNames;
        }
    }
}
