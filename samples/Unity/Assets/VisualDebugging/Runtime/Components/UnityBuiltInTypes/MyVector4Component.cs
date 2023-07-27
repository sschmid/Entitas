using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyVector4Component : IComponent
{
    public Vector4 Value;
}
