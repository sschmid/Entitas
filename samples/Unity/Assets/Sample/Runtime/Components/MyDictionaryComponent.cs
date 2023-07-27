using System.Collections.Generic;
using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyDictionaryComponent : IComponent
{
    public Dictionary<string, string> Value;
}
