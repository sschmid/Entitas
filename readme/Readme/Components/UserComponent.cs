using Entitas;
using Entitas.Plugins.Attributes;

[Game, Input, Unique]
public sealed class UserComponent : IComponent {

    public string Name;
    public int Age;
}
