using System.Collections.Generic;
using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyDictArrayComponent : IComponent
{
    public Dictionary<int, string[]> Dict;
    public Dictionary<int, string[]>[] DictArray;
}
