using System;

namespace Entitas.CodeGenerator {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
    public class HideInBlueprintInspectorAttribute : Attribute {
    }
}
