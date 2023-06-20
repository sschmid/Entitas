namespace MyApp.Main
{
    public sealed class ContextAttribute : Entitas.Generators.Attributes.ContextAttribute
    {
        public ContextAttribute() : base("MyApp.MainContext") { }
    }
}
