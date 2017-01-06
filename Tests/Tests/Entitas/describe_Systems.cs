using NSpec;
using Entitas;

class describe_Systems : nspec {

    static ReactiveSystemSpy createReactiveSystem(Pools pools) {
        var system = new ReactiveSystemSpy(pools.test.CreateCollector(Matcher.AllOf(CID.ComponentA)));
        pools.test.CreateEntity().AddComponentA();

        return system;
    }

    void when_systems() {

        Pools pools = null;

        before = () => {
            pools = new Pools { test = new Context(10) };
        };

        context["fixtures"] = () => {

            it["initializes InitializeSystemSpy"] = () => {
                var system = new InitializeSystemSpy();
                system.didInitialize.should_be(0);
                system.Initialize();
                system.didInitialize.should_be(1);
            };

            it["executes ExecuteSystemSpy"] = () => {
                var system = new ExecuteSystemSpy();
                system.didExecute.should_be(0);
                system.Execute();
                system.didExecute.should_be(1);
            };

            it["cleans up CleanupSystemSpy"] = () => {
                var system = new CleanupSystemSpy();
                system.didCleanup.should_be(0);
                system.Cleanup();
                system.didCleanup.should_be(1);
            };

            it["tears down TearDownSystemSpy"] = () => {
                var system = new TearDownSystemSpy();
                system.didTearDown.should_be(0);
                system.TearDown();
                system.didTearDown.should_be(1);
            };

            it["initializes, executes, cleans up and tears down system"] = () => {
                var system = new ReactiveSystemSpy(pools.test.CreateCollector(Matcher.AllOf(CID.ComponentA)));
                pools.test.CreateEntity().AddComponentA();

                system.didInitialize.should_be(0);
                system.Initialize();
                system.didInitialize.should_be(1);

                system.didExecute.should_be(0);
                system.Execute();
                system.didExecute.should_be(1);

                system.didCleanup.should_be(0);
                system.Cleanup();
                system.didCleanup.should_be(1);

                system.didTearDown.should_be(0);
                system.TearDown();
                system.didTearDown.should_be(1);
            };

            it["executes ReactiveSystemSpy"] = () => {
                var system = createReactiveSystem(pools);

                system.Execute();

                system.entities.Length.should_be(1);
            };
        };

        context["systems"] = () => {

            Systems systems = null;

            before = () => {
                systems = new Systems();
            };

            it["returns systems when adding system"] = () => {
                systems.Add(new InitializeSystemSpy()).should_be_same(systems);
            };

            it["initializes IInitializeSystem"] = () => {
                var system = new InitializeSystemSpy();
                systems.Add(system);
                systems.Initialize();
                system.didInitialize.should_be(1);
            };

            it["executes IExecuteSystem"] = () => {
                var system = new ExecuteSystemSpy();
                systems.Add(system);
                systems.Execute();
                system.didExecute.should_be(1);
            };

            it["wraps IReactiveSystem in a ReactiveSystem"] = () => {
                var system = new ReactiveSystemSpy(pools.test.CreateCollector(Matcher.AllOf(CID.ComponentA)));
                systems.Add(system);
                pools.test.CreateEntity().AddComponentA();
                systems.Execute();
                system.didExecute.should_be(1);
            };

            it["adds ReactiveSystem"] = () => {
                var system = new ReactiveSystemSpy(pools.test.CreateCollector(Matcher.AllOf(CID.ComponentA)));
                systems.Add(system);
                pools.test.CreateEntity().AddComponentA();
                systems.Execute();
                system.didExecute.should_be(1);
            };

            it["cleans up ICleanupSystem"] = () => {
                var system = new CleanupSystemSpy();
                systems.Add(system);
                systems.Cleanup();
                system.didCleanup.should_be(1);
            };

            it["initializes, executes, cleans up and tears down InitializeExecuteCleanupTearDownSystemSpy"] = () => {
                var system = new ReactiveSystemSpy(pools.test.CreateCollector(Matcher.AllOf(CID.ComponentA)));
                pools.test.CreateEntity().AddComponentA();

                systems.Add(system);

                system.didInitialize.should_be(0);
                systems.Initialize();
                system.didInitialize.should_be(1);

                system.didExecute.should_be(0);
                systems.Execute();
                system.didExecute.should_be(1);

                system.didCleanup.should_be(0);
                systems.Cleanup();
                system.didCleanup.should_be(1);

                system.didTearDown.should_be(0);
                systems.TearDown();
                system.didTearDown.should_be(1);
            };

            it["initializes, executes, cleans up and tears down ReactiveSystem"] = () => {
                var system = createReactiveSystem(pools);

                systems.Add(system);

                system.didInitialize.should_be(0);
                systems.Initialize();
                system.didInitialize.should_be(1);

                system.didExecute.should_be(0);
                systems.Execute();
                systems.Execute();
                system.didExecute.should_be(1);

                system.didCleanup.should_be(0);
                systems.Cleanup();
                system.didCleanup.should_be(1);

                system.didTearDown.should_be(0);
                systems.TearDown();
                system.didTearDown.should_be(1);
            };


            it["initializes, executes, cleans up and tears down systems recursively"] = () => {
                var system = createReactiveSystem(pools);

                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);

                system.didInitialize.should_be(0);
                parentSystems.Initialize();
                system.didInitialize.should_be(1);

                system.didExecute.should_be(0);
                parentSystems.Execute();
                parentSystems.Execute();
                system.didExecute.should_be(1);

                system.didCleanup.should_be(0);
                parentSystems.Cleanup();
                system.didCleanup.should_be(1);

                system.didTearDown.should_be(0);
                parentSystems.TearDown();
                system.didTearDown.should_be(1);
            };

            it["clears reactive systems"] = () => {
                var system = createReactiveSystem(pools);

                systems.Add(system);

                systems.Initialize();
                system.didInitialize.should_be(1);

                systems.ClearReactiveSystems();
                systems.Execute();
                system.didExecute.should_be(0);
            };

            it["clears reactive systems recursively"] = () => {
                var system = createReactiveSystem(pools);
                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);
                
                parentSystems.Initialize();
                system.didInitialize.should_be(1);

                parentSystems.ClearReactiveSystems();
                parentSystems.Execute();
                system.didExecute.should_be(0);
            };

            it["deactivates reactive systems"] = () => {
                var system = createReactiveSystem(pools);

                systems.Add(system);

                systems.Initialize();
                system.didInitialize.should_be(1);

                systems.DeactivateReactiveSystems();
                systems.Execute();
                system.didExecute.should_be(0);
            };

            it["deactivates reactive systems recursively"] = () => {
                var system = createReactiveSystem(pools);
                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);

                parentSystems.Initialize();
                system.didInitialize.should_be(1);

                parentSystems.DeactivateReactiveSystems();
                parentSystems.Execute();
                system.didExecute.should_be(0);
            };

            it["activates reactive systems"] = () => {
                var system = createReactiveSystem(pools);

                systems.Add(system);

                systems.Initialize();
                system.didInitialize.should_be(1);

                systems.DeactivateReactiveSystems();
                systems.ActivateReactiveSystems();
                systems.Execute();
                system.didExecute.should_be(0);

                pools.test.CreateEntity().AddComponentA();
                systems.Execute();

                system.didExecute.should_be(1);
            };

            it["activates reactive systems recursively"] = () => {
                var system = createReactiveSystem(pools);
                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);

                parentSystems.Initialize();
                system.didInitialize.should_be(1);

                parentSystems.DeactivateReactiveSystems();
                parentSystems.ActivateReactiveSystems();
                parentSystems.Execute();
                system.didExecute.should_be(0);

                pools.test.CreateEntity().AddComponentA();
                systems.Execute();

                system.didExecute.should_be(1);
            };
        };
    }
}
