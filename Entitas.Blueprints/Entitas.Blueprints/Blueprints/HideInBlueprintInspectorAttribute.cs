using System;

namespace Entitas.Blueprints {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HideInBlueprintInspectorAttribute : Attribute {
    }
}
