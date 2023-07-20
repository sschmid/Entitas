#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

[Context(typeof(MainContext))]
public sealed class EntityIndexComponent : IComponent
{
    [EntityIndex(false)]
    public string Value;
}
