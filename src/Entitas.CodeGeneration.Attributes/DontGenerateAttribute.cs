using System;

namespace Entitas.Plugins.Attributes
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class DontGenerateAttribute : Attribute
    {
        public readonly bool GenerateIndex;

        public DontGenerateAttribute(bool generateIndex = true)
        {
            GenerateIndex = generateIndex;
        }
    }
}
