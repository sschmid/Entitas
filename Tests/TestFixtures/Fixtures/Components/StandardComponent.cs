using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test")]
public sealed class StandardComponent : IComponent
{
    public string value;
}
