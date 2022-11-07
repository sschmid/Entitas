using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test"), Context("Test2")]
public sealed class Test2ContextComponent : IComponent
{
    public string value;
}
