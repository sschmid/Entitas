using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyRectComponent : IComponent
{
    public Rect Value;
}
