public sealed partial class InputContext : Entitas.Context<InputEntity> {

    public InputContext()
        : base(
            InputComponentsLookup.TotalComponents,
            0,
            new Entitas.ContextInfo(
                "Input",
                InputComponentsLookup.componentNames,
                InputComponentsLookup.componentTypes
            )
        ) {
    }
}
