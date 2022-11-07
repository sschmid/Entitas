using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Unique]
public sealed class UniqueStandardComponent : IComponent
{
    public string value;
}
