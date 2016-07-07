using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class RuntimeOnlyAttribute :Attribute {
    }
}

