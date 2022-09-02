using System;

namespace Entitas.CodeGeneration.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum)]
    public class DontGenerateAttribute : Attribute {

        public readonly bool generateIndex;

        public DontGenerateAttribute(bool generateIndex = true) {
            this.generateIndex = generateIndex;
        }
    }
}
