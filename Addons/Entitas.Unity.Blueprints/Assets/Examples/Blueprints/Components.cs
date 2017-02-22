using Entitas;
using Entitas.CodeGenerator.Api;

[Game]
public class NameComponent : IComponent {
    public string value;
}

[Game]
public class AgeComponent : IComponent {
    public int value;
}
