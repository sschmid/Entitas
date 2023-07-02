using Entitas.Generators.Attributes;

namespace MyApp
{
    public static partial class ContextInitialization
    {
        [ContextInitialization(typeof(MainContext))]
        public static partial void InitializeMain();
    }
}
