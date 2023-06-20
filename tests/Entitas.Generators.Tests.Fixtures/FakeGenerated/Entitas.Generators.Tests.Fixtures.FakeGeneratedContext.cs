// ReSharper disable once CheckNamespace

namespace Entitas.Generators.Tests.Fixtures
{
    public sealed partial class FakeGeneratedContext : Entitas.Context<FakeGenerated.Entity>
    {
        public FakeGeneratedContext()
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
                () => new FakeGenerated.Entity()
            ) { }
    }
}
