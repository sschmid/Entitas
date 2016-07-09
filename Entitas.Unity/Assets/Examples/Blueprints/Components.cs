using Entitas;
using Entitas.Serialization.Blueprints;

[Blueprints]
public class NameComponent : IComponent {
    public string value;
}

[Blueprints]
public class AgeComponent : IComponent {
    public int value;
}

[Blueprints, HideInBlueprintInspector]
public class HideInBlueprintInspectorComponent : IComponent {
}

