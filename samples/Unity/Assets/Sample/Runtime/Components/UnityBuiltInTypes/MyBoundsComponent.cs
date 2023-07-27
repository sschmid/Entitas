using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyBoundsComponent : IComponent
{
    public Bounds Value;
}
