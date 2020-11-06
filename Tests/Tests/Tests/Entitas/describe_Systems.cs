using NSpec;
using Shouldly;
using Entitas;

class describe_Systems : nspec {

    static ReactiveSystemSpy createReactiveSystem(MyTestContext context) {
        var system = new ReactiveSystemSpy(context.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA)));
        context.CreateEntity().AddComponentA();

        return system;
    }

    void when_systems() {

        MyTestContext ctx = null;

        before = () => {
            ctx = new MyTestContext();
        };

        context["fixtures"] = () => {

            it["initializes InitializeSystemSpy"] = () => {
                var system = new InitializeSystemSpy();
                system.didInitialize.ShouldBe(0);
                system.Initialize();
                system.didInitialize.ShouldBe(1);
            };

            it["executes ExecuteSystemSpy"] = () => {
                var system = new ExecuteSystemSpy();
                system.didExecute.ShouldBe(0);
                system.Execute();
                system.didExecute.ShouldBe(1);
            };

            it["cleans up CleanupSystemSpy"] = () => {
                var system = new CleanupSystemSpy();
                system.didCleanup.ShouldBe(0);
                system.Cleanup();
                system.didCleanup.ShouldBe(1);
            };

            it["tears down TearDownSystemSpy"] = () => {
                var system = new TearDownSystemSpy();
                system.didTearDown.ShouldBe(0);
                system.TearDown();
                system.didTearDown.ShouldBe(1);
            };

            it["initializes, executes, cleans up and tears down system"] = () => {
                var system = new ReactiveSystemSpy(ctx.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA)));
                ctx.CreateEntity().AddComponentA();

                system.didInitialize.ShouldBe(0);
                system.Initialize();
                system.didInitialize.ShouldBe(1);

                system.didExecute.ShouldBe(0);
                system.Execute();
                system.didExecute.ShouldBe(1);

                system.didCleanup.ShouldBe(0);
                system.Cleanup();
                system.didCleanup.ShouldBe(1);

                system.didTearDown.ShouldBe(0);
                system.TearDown();
                system.didTearDown.ShouldBe(1);
            };

            it["executes ReactiveSystemSpy"] = () => {
                var system = createReactiveSystem(ctx);

                system.Execute();

                system.entities.Length.ShouldBe(1);
            };
        };

        context["systems"] = () => {

            Systems systems = null;

            before = () => {
                systems = new Systems();
            };

            it["returns systems when adding system"] = () => {
                systems.Add(new InitializeSystemSpy()).ShouldBeSameAs(systems);
            };

            it["initializes IInitializeSystem"] = () => {
                var system = new InitializeSystemSpy();
                systems.Add(system);
                systems.Initialize();
                system.didInitialize.ShouldBe(1);
            };

            it["executes IExecuteSystem"] = () => {
                var system = new ExecuteSystemSpy();
                systems.Add(system);
                systems.Execute();
                system.didExecute.ShouldBe(1);
            };

            it["wraps IReactiveSystem in a ReactiveSystem"] = () => {
                var system = new ReactiveSystemSpy(ctx.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA)));
                systems.Add(system);
                ctx.CreateEntity().AddComponentA();
                systems.Execute();
                system.didExecute.ShouldBe(1);
            };

            it["adds ReactiveSystem"] = () => {
                var system = new ReactiveSystemSpy(ctx.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA)));
                systems.Add(system);
                ctx.CreateEntity().AddComponentA();
                systems.Execute();
                system.didExecute.ShouldBe(1);
            };

            it["cleans up ICleanupSystem"] = () => {
                var system = new CleanupSystemSpy();
                systems.Add(system);
                systems.Cleanup();
                system.didCleanup.ShouldBe(1);
            };

            it["initializes, executes, cleans up and tears down InitializeExecuteCleanupTearDownSystemSpy"] = () => {
                var system = new ReactiveSystemSpy(ctx.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA)));
                ctx.CreateEntity().AddComponentA();

                systems.Add(system);

                system.didInitialize.ShouldBe(0);
                systems.Initialize();
                system.didInitialize.ShouldBe(1);

                system.didExecute.ShouldBe(0);
                systems.Execute();
                system.didExecute.ShouldBe(1);

                system.didCleanup.ShouldBe(0);
                systems.Cleanup();
                system.didCleanup.ShouldBe(1);

                system.didTearDown.ShouldBe(0);
                systems.TearDown();
                system.didTearDown.ShouldBe(1);
            };

            it["initializes, executes, cleans up and tears down ReactiveSystem"] = () => {
                var system = createReactiveSystem(ctx);

                systems.Add(system);

                system.didInitialize.ShouldBe(0);
                systems.Initialize();
                system.didInitialize.ShouldBe(1);

                system.didExecute.ShouldBe(0);
                systems.Execute();
                systems.Execute();
                system.didExecute.ShouldBe(1);

                system.didCleanup.ShouldBe(0);
                systems.Cleanup();
                system.didCleanup.ShouldBe(1);

                system.didTearDown.ShouldBe(0);
                systems.TearDown();
                system.didTearDown.ShouldBe(1);
            };


            it["initializes, executes, cleans up and tears down systems recursively"] = () => {
                var system = createReactiveSystem(ctx);

                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);

                system.didInitialize.ShouldBe(0);
                parentSystems.Initialize();
                system.didInitialize.ShouldBe(1);

                system.didExecute.ShouldBe(0);
                parentSystems.Execute();
                parentSystems.Execute();
                system.didExecute.ShouldBe(1);

                system.didCleanup.ShouldBe(0);
                parentSystems.Cleanup();
                system.didCleanup.ShouldBe(1);

                system.didTearDown.ShouldBe(0);
                parentSystems.TearDown();
                system.didTearDown.ShouldBe(1);
            };

            it["clears reactive systems"] = () => {
                var system = createReactiveSystem(ctx);

                systems.Add(system);

                systems.Initialize();
                system.didInitialize.ShouldBe(1);

                systems.ClearReactiveSystems();
                systems.Execute();
                system.didExecute.ShouldBe(0);
            };

            it["clears reactive systems recursively"] = () => {
                var system = createReactiveSystem(ctx);
                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);

                parentSystems.Initialize();
                system.didInitialize.ShouldBe(1);

                parentSystems.ClearReactiveSystems();
                parentSystems.Execute();
                system.didExecute.ShouldBe(0);
            };

            it["deactivates reactive systems"] = () => {
                var system = createReactiveSystem(ctx);

                systems.Add(system);

                systems.Initialize();
                system.didInitialize.ShouldBe(1);

                systems.DeactivateReactiveSystems();
                systems.Execute();
                system.didExecute.ShouldBe(0);
            };

            it["deactivates reactive systems recursively"] = () => {
                var system = createReactiveSystem(ctx);
                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);

                parentSystems.Initialize();
                system.didInitialize.ShouldBe(1);

                parentSystems.DeactivateReactiveSystems();
                parentSystems.Execute();
                system.didExecute.ShouldBe(0);
            };

            it["activates reactive systems"] = () => {
                var system = createReactiveSystem(ctx);

                systems.Add(system);

                systems.Initialize();
                system.didInitialize.ShouldBe(1);

                systems.DeactivateReactiveSystems();
                systems.ActivateReactiveSystems();
                systems.Execute();
                system.didExecute.ShouldBe(0);

                ctx.CreateEntity().AddComponentA();
                systems.Execute();

                system.didExecute.ShouldBe(1);
            };

            it["activates reactive systems recursively"] = () => {
                var system = createReactiveSystem(ctx);
                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);

                parentSystems.Initialize();
                system.didInitialize.ShouldBe(1);

                parentSystems.DeactivateReactiveSystems();
                parentSystems.ActivateReactiveSystems();
                parentSystems.Execute();
                system.didExecute.ShouldBe(0);

                ctx.CreateEntity().AddComponentA();
                systems.Execute();

                system.didExecute.ShouldBe(1);
            };
        };
    }
}
