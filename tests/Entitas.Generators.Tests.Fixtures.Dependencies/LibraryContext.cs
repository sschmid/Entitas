namespace MyApp
{
    public sealed partial class LibraryContext : Entitas.Context<Library.Entity>
    {
        public LibraryContext()
            : base(
                Library.ComponentsLookup.ComponentTypes.Length,
                0,
                new Entitas.ContextInfo(
                    "MyApp.LibraryContext",
                    Library.ComponentsLookup.ComponentNames,
                    Library.ComponentsLookup.ComponentTypes
                ),
                entity =>
#if (ENTITAS_FAST_AND_UNSAFE)
                new Entitas.UnsafeAERC(),
#else
                    new Entitas.SafeAERC(entity),
#endif
                () => new Library.Entity()
            ) { }
    }
}
