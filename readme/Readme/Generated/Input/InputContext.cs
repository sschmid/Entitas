public sealed partial class InputContext : Entitas.Context<InputEntity>
{
    public InputContext() : base(
        InputComponentsLookup.TotalComponents,
        0,
        new Entitas.ContextInfo(
            "Input",
            InputComponentsLookup.ComponentNames,
            InputComponentsLookup.ComponentTypes
        ),
        entity =>
#if (ENTITAS_FAST_AND_UNSAFE)
            new Entitas.UnsafeAERC(),
#else
            new Entitas.SafeAERC(entity),
#endif
        () => new InputEntity()
    ) { }
}
