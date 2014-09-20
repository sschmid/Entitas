using System;

namespace Entitas.CodeGenerator {
    [AttributeUsage(AttributeTargets.Class)]
    public class DontGenerate : Attribute {
    }
}