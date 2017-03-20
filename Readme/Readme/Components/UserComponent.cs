using Entitas;
using Entitas.CodeGenerator.Api;

[Game, Unique]
public sealed class UserComponent : IComponent {

    public string name;
    public int age;
}
