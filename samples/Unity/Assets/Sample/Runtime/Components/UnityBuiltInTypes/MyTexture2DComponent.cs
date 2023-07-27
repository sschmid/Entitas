using Entitas;
using Entitas.Generators.Attributes;
using UnityEngine;

[Context(typeof(GameContext))]
public sealed class MyTexture2DComponent : IComponent
{
    public Texture2D Value;
}
