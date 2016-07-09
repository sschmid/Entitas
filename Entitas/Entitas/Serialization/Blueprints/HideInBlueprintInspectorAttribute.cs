using System;

namespace Entitas.Serialization.Blueprints {

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
    public class HideInBlueprintInspectorAttribute : Attribute {
    }
}
