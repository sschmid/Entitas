using System;

namespace Entitas.CodeGeneration.Attributes
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
