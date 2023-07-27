using System.Collections.Generic;
using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyListArrayComponent : IComponent
{
    public List<string>[] Value;
}
