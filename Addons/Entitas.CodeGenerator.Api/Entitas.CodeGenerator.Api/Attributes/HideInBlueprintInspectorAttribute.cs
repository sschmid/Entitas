using System;

namespace Entitas.CodeGenerator.Api {

    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class | AttributeTargets.Struct)]
    public class HideInBlueprintInspectorAttribute : Attribute {
    }
}
