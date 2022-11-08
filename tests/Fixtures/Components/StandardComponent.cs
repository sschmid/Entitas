using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test1")]
public sealed class StandardComponent : IComponent
{
    public string value;
}
