using System;

namespace Entitas.CodeGeneration {

    [AttributeUsage(AttributeTargets.Class)]
    public class DontGenerateAttribute : Attribute {
        public readonly bool generateIndex;

        public DontGenerateAttribute(bool generateIndex = true) {
            this.generateIndex = generateIndex;
        }
    }
}