using System.Collections.Generic;
using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyHashSetComponent : IComponent
{
    public HashSet<string> Value;
}
