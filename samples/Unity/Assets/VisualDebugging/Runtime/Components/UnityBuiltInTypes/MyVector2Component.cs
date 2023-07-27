using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyVector2Component : IComponent
{
    public Vector2 Value;
}
