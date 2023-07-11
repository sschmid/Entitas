using Entitas.Generators.Attributes;

namespace MyApp.Library
{
    public static partial class ContextInitialization
    {
        [ContextInitialization(typeof(LibraryContext))]
        public static partial void InitializeMain();
    }
}

namespace MyApp.Library
{
    public static partial class ContextInitialization
    {
        public static partial void InitializeMain()
        {
            MyApp.LibraryContext.ComponentNames = new string[] { };

            MyApp.LibraryContext.ComponentTypes = new global::System.Type[] { };
        }
    }
}
