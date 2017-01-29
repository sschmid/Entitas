using Entitas.Api;
using Entitas.CodeGenerator.Api;

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

[Blueprints, HideInBlueprintInspector]
public class HideInBlueprintInspectorClass {
}
