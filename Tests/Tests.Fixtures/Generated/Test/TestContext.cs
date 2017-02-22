public sealed partial class TestContext : Entitas.Context<TestEntity> {

    public TestContext()
        : base(
            TestComponentsLookup.TotalComponents,
            0,
            new Entitas.ContextInfo(
                "Test",
                TestComponentsLookup.componentNames,
                TestComponentsLookup.componentTypes
            )
        ) {
    }
}
