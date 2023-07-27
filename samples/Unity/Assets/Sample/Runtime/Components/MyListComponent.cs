using System.Collections.Generic;
using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyListComponent : IComponent
{
    public List<string> Value;
}
