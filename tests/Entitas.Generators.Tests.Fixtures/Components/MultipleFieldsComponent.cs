using Entitas;

#pragma warning disable CS0649

[MyApp.Main.Context]
public sealed class MultipleFieldsComponent : IComponent
{
    public string Value1;
    public string Value2;
    public string Value3;
}
