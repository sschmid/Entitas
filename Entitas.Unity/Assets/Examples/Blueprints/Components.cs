using Entitas;
using Entitas.CodeGenerator;

[Blueprints]
public class NameComponent : IComponent {
    public string value;
}

[Blueprints]
public class AgeComponent : IComponent {
    public int value;
}

[Blueprints]
[RuntimeOnly]
public class RuntimeOnlyComponent : IComponent {
}

