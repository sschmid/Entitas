using Entitas;
using Entitas.CodeGeneration.Attributes;

[Game, Unique]
public class MyUniqueComponent : IComponent
{
    public string Value;
}
