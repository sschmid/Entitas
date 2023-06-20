namespace MyApp
{
    public sealed partial class MainContext : Entitas.Context<Main.Entity>
    {
        public MainContext()
            : base(
                0,
                0,
                new Entitas.ContextInfo(
                    "MyApp.MainContext",
                    System.Array.Empty<string>(),
                    System.Array.Empty<System.Type>()
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
