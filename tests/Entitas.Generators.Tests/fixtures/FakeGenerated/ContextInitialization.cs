#nullable disable

using Entitas.Generators.Attributes;

namespace MyApp
{
    public static partial class ContextInitialization
    {
        [ContextInitialization(typeof(MainContext))]
        public static partial void InitializeMain();
    }
}

namespace MyApp
{
    public static partial class ContextInitialization
    {
        public static partial void InitializeMain()
        {
            MyApp.MainContext.ComponentNames = new string[] { };

            MyApp.MainContext.ComponentTypes = new global::System.Type[] { };
        }
    }
}
