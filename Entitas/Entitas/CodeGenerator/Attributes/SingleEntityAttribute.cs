using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
    public class SingleEntityAttribute : Attribute {
    }
}