using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyAnimationCurveComponent : IComponent
{
    public AnimationCurve Value;
}
