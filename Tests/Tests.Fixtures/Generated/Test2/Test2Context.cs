public sealed partial class Test2Context : Entitas.Context<Test2Entity> {

    public Test2Context()
        : base(
            Test2ComponentsLookup.TotalComponents,
            0,
            new Entitas.ContextInfo(
                "Test2",
                Test2ComponentsLookup.componentNames,
                Test2ComponentsLookup.componentTypes
            )
        ) {
    }
}
