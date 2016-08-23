using NSpec;
using Entitas;

class describe_Systems : nspec {

    static ReactiveSystem createReactiveSystem(Pool pool) {
        var subSystem = new ReactiveSubSystemSpy(Matcher.AllOf(CID.ComponentA), GroupEventType.OnEntityAdded);
        var reactiveSystem = new ReactiveSystem(pool, subSystem);
        pool.CreateEntity().AddComponentA();

        return reactiveSystem;
    }

    void when_systems() {

        Pool pool = null;

        before = () => {
            pool = new Pool(10);
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

            it["deinitializes up DeinitializeSystemSpy"] = () => {
                var system = new DeinitializeSystemSpy();
                system.didDeinitialize.should_be(0);
                system.Deinitialize();
                system.didDeinitialize.should_be(1);
            };

            it["initializes, executes, cleans up and deinitializes InitializeExecuteCleanupDeinitializeSystemSpy"] = () => {
                var system = new InitializeExecuteCleanupDeinitializeSystemSpy();

                system.didInitialize.should_be(0);
                system.Initialize();
                system.didInitialize.should_be(1);

                system.didExecute.should_be(0);
                system.Execute();
                system.didExecute.should_be(1);

                system.didCleanup.should_be(0);
                system.Cleanup();
                system.didCleanup.should_be(1);

                system.didDeinitialize.should_be(0);
                system.Deinitialize();
                system.didDeinitialize.should_be(1);
            };

            it["executes ReactiveSystemSpy"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;

                system.Execute();

                spy.entities.Length.should_be(1);
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

            it["cleans up ICleanupSystem"] = () => {
                var system = new CleanupSystemSpy();
                systems.Add(system);
                systems.Cleanup();
                system.didCleanup.should_be(1);
            };

            it["initializes, executes, cleans up and deinitializes InitializeExecuteCleanupDeinitializeSystemSpy"] = () => {
                var system = new InitializeExecuteCleanupDeinitializeSystemSpy();
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

                system.didDeinitialize.should_be(0);
                systems.Deinitialize();
                system.didDeinitialize.should_be(1);
            };

            it["initializes, executes, cleans up and deinitializes ReactiveSystem"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;

                systems.Add(system);

                spy.didInitialize.should_be(0);
                systems.Initialize();
                spy.didInitialize.should_be(1);

                spy.didExecute.should_be(0);
                systems.Execute();
                systems.Execute();
                spy.didExecute.should_be(1);

                spy.didCleanup.should_be(0);
                systems.Cleanup();
                spy.didCleanup.should_be(1);

                spy.didDeinitialize.should_be(0);
                systems.Deinitialize();
                spy.didDeinitialize.should_be(1);
            };


            it["initializes, executes, cleans up and deinitializes systems recursively"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;

                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);

                spy.didInitialize.should_be(0);
                parentSystems.Initialize();
                spy.didInitialize.should_be(1);

                spy.didExecute.should_be(0);
                parentSystems.Execute();
                parentSystems.Execute();
                spy.didExecute.should_be(1);

                spy.didCleanup.should_be(0);
                parentSystems.Cleanup();
                spy.didCleanup.should_be(1);

                spy.didDeinitialize.should_be(0);
                parentSystems.Deinitialize();
                spy.didDeinitialize.should_be(1);
            };

            it["clears reactive systems"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;

                systems.Add(system);

                systems.Initialize();
                spy.didInitialize.should_be(1);

                systems.ClearReactiveSystems();
                systems.Execute();
                spy.didExecute.should_be(0);
            };

            it["clears reactive systems recursively"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;
                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);
                
                parentSystems.Initialize();
                spy.didInitialize.should_be(1);

                parentSystems.ClearReactiveSystems();
                parentSystems.Execute();
                spy.didExecute.should_be(0);
            };

            it["deactivates reactive systems"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;

                systems.Add(system);

                systems.Initialize();
                spy.didInitialize.should_be(1);

                systems.DeactivateReactiveSystems();
                systems.Execute();
                spy.didExecute.should_be(0);
            };

            it["deactivates reactive systems recursively"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;
                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);

                parentSystems.Initialize();
                spy.didInitialize.should_be(1);

                parentSystems.DeactivateReactiveSystems();
                parentSystems.Execute();
                spy.didExecute.should_be(0);
            };

            it["activates reactive systems"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;

                systems.Add(system);

                systems.Initialize();
                spy.didInitialize.should_be(1);

                systems.DeactivateReactiveSystems();
                systems.ActivateReactiveSystems();
                systems.Execute();
                spy.didExecute.should_be(0);

                pool.CreateEntity().AddComponentA();
                systems.Execute();

                spy.didExecute.should_be(1);
            };

            it["activates reactive systems recursively"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;
                systems.Add(system);

                var parentSystems = new Systems();
                parentSystems.Add(systems);

                parentSystems.Initialize();
                spy.didInitialize.should_be(1);

                parentSystems.DeactivateReactiveSystems();
                parentSystems.ActivateReactiveSystems();
                parentSystems.Execute();
                spy.didExecute.should_be(0);

                pool.CreateEntity().AddComponentA();
                systems.Execute();

                spy.didExecute.should_be(1);
            };
        };
    }
}

