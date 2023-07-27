using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyUnityObjectComponent : IComponent
{
    public Object Value;
}
