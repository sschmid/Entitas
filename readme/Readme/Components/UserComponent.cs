using Entitas;
using Entitas.Plugins.Attributes;

[Game, Unique]
public sealed class UserComponent : IComponent {

    public string Name;
    public int Age;
}
