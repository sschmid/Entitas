namespace MyApp
{
    public sealed partial class MainContext : Entitas.Context<Main.Entity>
    {
        public MainContext()
            : base(
                Main.ComponentsLookup.ComponentTypes.Length,
                0,
                new Entitas.ContextInfo(
                    "MyApp.MainContext",
                    Main.ComponentsLookup.ComponentNames,
                    Main.ComponentsLookup.ComponentTypes
                ),
                entity =>
#if (ENTITAS_FAST_AND_UNSAFE)
                new Entitas.UnsafeAERC(),
#else
                    new Entitas.SafeAERC(entity),
#endif
                () => new Main.Entity()
            ) { }
    }
}
