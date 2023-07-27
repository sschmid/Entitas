using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyColorComponent : IComponent
{
    public Color Value;
}
