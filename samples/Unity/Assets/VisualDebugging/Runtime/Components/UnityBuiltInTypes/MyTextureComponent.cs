using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyTextureComponent : IComponent
{
    public Texture Value;
}
