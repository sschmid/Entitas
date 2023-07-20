#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

[Context(typeof(MainContext))]
public sealed class PrimaryEntityIndexComponent : IComponent
{
    [EntityIndex(true)]
    public string Value;
}
