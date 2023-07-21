using Entitas;

namespace MyApp
{
    partial class LibraryContext : IContext { }
}

namespace MyApp
{
    public sealed partial class LibraryContext : global::Entitas.Context<Library.Entity>
    {
        public static string[] ComponentNames;
        public static global::System.Type[] ComponentTypes;

        public LibraryContext() :
            base(
                ComponentTypes.Length,
                0,
                new global::Entitas.ContextInfo(
                    "MyApp.LibraryContext",
                    ComponentNames,
                    ComponentTypes
                ),
                entity =>
#if (ENTITAS_FAST_AND_UNSAFE)
                    new global::Entitas.UnsafeAERC(),
#else
                    new global::Entitas.SafeAERC(entity),
#endif
                () => new Library.Entity()
            ) { }
    }
}
