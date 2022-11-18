using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1"), Unique]
public sealed class UniqueStandardComponent : IComponent
{
    public string value;
}
