using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

[Context(typeof(MainContext))]
public sealed class OneFieldComponent : IComponent
{
    public string Value;
}
