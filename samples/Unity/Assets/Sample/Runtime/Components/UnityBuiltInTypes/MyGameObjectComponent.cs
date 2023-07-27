using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyGameObjectComponent : IComponent
{
    public GameObject Value;
}
