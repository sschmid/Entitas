#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

[Context(typeof(MainContext))]
public sealed class MultipleFieldsComponent : IComponent
{
    public string Value1;
    public string Value2;
    public string Value3;
}
