#nullable disable

using Entitas;

namespace MyApp
{
    partial class MainContext : IContext { }
}

namespace MyApp
{
    public sealed partial class MainContext : global::Entitas.Context<Main.Entity>
    {
        public static string[] ComponentNames;
        public static global::System.Type[] ComponentTypes;

        public MainContext()
            : base(
                ComponentTypes.Length,
                0,
                new global::Entitas.ContextInfo(
                    "MyApp.MainContext",
                    ComponentNames,
                    ComponentTypes
                ),
                entity =>
#if (ENTITAS_FAST_AND_UNSAFE)
                new global::Entitas.UnsafeAERC(),
#else
                    new global::Entitas.SafeAERC(entity),
#endif
                () => new Main.Entity()
            ) { }
    }
}
