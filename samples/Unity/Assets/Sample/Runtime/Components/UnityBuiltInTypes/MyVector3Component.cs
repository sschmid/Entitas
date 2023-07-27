using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyVector3Component : IComponent
{
    public Vector3 Value;
}
