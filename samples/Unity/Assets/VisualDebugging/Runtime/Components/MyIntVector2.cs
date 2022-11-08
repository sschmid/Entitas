using System;
using Entitas.CodeGeneration.Attributes;

[Serializable, Game, ComponentName("PositionComponent", "VelocityComponent")]
public struct MyIntVector2
{
    public int X;
    public int Y;
}
