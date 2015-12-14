using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Class)]
    public class DontGenerateAttribute : Attribute {
        public bool generateIndex { get { return _generateIndex; } }

        readonly bool _generateIndex;

        public DontGenerateAttribute(bool generateIndex = true) {
            _generateIndex = generateIndex;
        }
    }
}