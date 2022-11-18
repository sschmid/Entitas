using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Context("Test2")]
public sealed class Test2ContextComponent : IComponent
{
    public string value;
}
