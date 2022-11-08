using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test1"), Unique]
public sealed class UniqueStandardComponent : IComponent
{
    public string value;
}
