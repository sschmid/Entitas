using System;

namespace Entitas.CodeGenerator {
    [AttributeUsage(AttributeTargets.Class)]
    public class DontGenerateAttribute : Attribute {
    }
}