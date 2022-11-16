using System;

namespace Entitas.CodeGeneration.Attributes
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
