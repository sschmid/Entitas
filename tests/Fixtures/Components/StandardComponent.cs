using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1")]
public sealed class StandardComponent : IComponent
{
    public string value;
}
