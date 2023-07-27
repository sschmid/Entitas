using System;
using Entitas;
using Entitas.Generators.Attributes;

[Context(typeof(GameContext))]
public sealed class MyDateTimeComponent : IComponent
{
    public DateTime Value;
}
