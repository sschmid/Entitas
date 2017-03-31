using System;

namespace Entitas.CodeGenerator.Attributes {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
    public class UniqueAttribute : Attribute {
    }
}
