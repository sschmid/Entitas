using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Unique]
public sealed class UserComponent : IComponent {

    public string name;
    public int age;
}
