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
                var initializeSystem = new InitializeSystemSpy();
                initializeSystem.didInitialize.should_be(0);
                initializeSystem.Initialize();
                initializeSystem.didInitialize.should_be(1);
            };

            it["executes ExecuteSystemSpy"] = () => {
                var executeSystem = new ExecuteSystemSpy();
                executeSystem.didExecute.should_be(0);
                executeSystem.Execute();
                executeSystem.didExecute.should_be(1);
            };

            it["cleans up CleanupSystemSpy"] = () => {
                var cleanupSystem = new CleanupSystemSpy();
                cleanupSystem.didCleanup.should_be(0);
                cleanupSystem.Cleanup();
                cleanupSystem.didCleanup.should_be(1);
            };

            it["initializes and executes InitializeExecuteSystemSpy"] = () => {
                var system = new InitializeExecuteCleanupSystemSpy();
                system.didInitialize.should_be(0);
                system.didExecute.should_be(0);
                system.didCleanup.should_be(0);
                system.Initialize();
                system.Execute();
                system.Cleanup();
                system.didInitialize.should_be(1);
                system.didExecute.should_be(1);
                system.didCleanup.should_be(1);
            };

            it["initializes ReactiveSystemSpy"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;

                spy.didInitialize.should_be(0);
                spy.didExecute.should_be(0);

                spy.Initialize();

                spy.didInitialize.should_be(1);
                spy.didExecute.should_be(0);
            };

            it["executes ReactiveSystemSpy"] = () => {
                var system = createReactiveSystem(pool);
                var spy = (ReactiveSubSystemSpy)system.subsystem;

                spy.didInitialize.should_be(0);
                spy.didExecute.should_be(0);

                system.Execute();

                spy.didInitialize.should_be(0);
                spy.didExecute.should_be(1);
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

            it["initializes and executes and cleans up IInitializeSystem, IExecuteSystem, ICleanupSystem"] = () => {
                var system = new InitializeExecuteCleanupSystemSpy();
                systems.Add(system);
                systems.Initialize();
                systems.Execute();
                systems.Cleanup();
                system.didInitialize.should_be(1);
                system.didExecute.should_be(1);
                system.didCleanup.should_be(1);
            };

            it["initializes and executes and cleans up ReactiveSystem"] = () => {
                var system = createReactiveSystem(pool);

                systems.Add(system);
                systems.Initialize();
                systems.Execute();
                systems.Execute();
                systems.Cleanup();

                var spy = (ReactiveSubSystemSpy)system.subsystem;
                spy.didInitialize.should_be(1);
                spy.didExecute.should_be(1);
                spy.didCleanup.should_be(1);
            };

            it["clears reactive systems"] = () => {
                var system = createReactiveSystem(pool);

                systems.Add(system);
                systems.Initialize();
                systems.ClearReactiveSystems();
                systems.Execute();

                var spy = (ReactiveSubSystemSpy)system.subsystem;
                spy.didInitialize.should_be(1);
                spy.didExecute.should_be(0);
            };

            it["clears reactive systems recursively"] = () => {
                var system = createReactiveSystem(pool);
                systems.Add(system);
                var parentSystems = new Systems();
                parentSystems.Add(systems);
                
                parentSystems.Initialize();
                parentSystems.ClearReactiveSystems();
                parentSystems.Execute();

                var spy = (ReactiveSubSystemSpy)system.subsystem;
                spy.didInitialize.should_be(1);
                spy.didExecute.should_be(0);
            };

            it["deactivates reactive systems"] = () => {
                var system = createReactiveSystem(pool);

                systems.Add(system);
                systems.Initialize();
                systems.DeactivateReactiveSystems();
                systems.Execute();

                var spy = (ReactiveSubSystemSpy)system.subsystem;
                spy.didInitialize.should_be(1);
                spy.didExecute.should_be(0);
            };

            it["deactivates reactive systems recursively"] = () => {
                var system = createReactiveSystem(pool);
                systems.Add(system);
                var parentSystems = new Systems();
                parentSystems.Add(systems);

                parentSystems.Initialize();
                parentSystems.DeactivateReactiveSystems();
                parentSystems.Execute();

                var spy = (ReactiveSubSystemSpy)system.subsystem;
                spy.didInitialize.should_be(1);
                spy.didExecute.should_be(0);
            };

            it["activates reactive systems"] = () => {
                var system = createReactiveSystem(pool);

                systems.Add(system);
                systems.Initialize();
                systems.DeactivateReactiveSystems();
                systems.ActivateReactiveSystems();
                systems.Execute();

                var spy = (ReactiveSubSystemSpy)system.subsystem;
                spy.didInitialize.should_be(1);
                spy.didExecute.should_be(0);

                pool.CreateEntity().AddComponentA();
                systems.Execute();

                spy.didExecute.should_be(1);
            };

            it["activates reactive systems recursively"] = () => {
                var system = createReactiveSystem(pool);
                systems.Add(system);
                var parentSystems = new Systems();
                parentSystems.Add(systems);

                parentSystems.Initialize();
                parentSystems.DeactivateReactiveSystems();
                parentSystems.ActivateReactiveSystems();
                parentSystems.Execute();

                var spy = (ReactiveSubSystemSpy)system.subsystem;
                spy.didInitialize.should_be(1);
                spy.didExecute.should_be(0);

                pool.CreateEntity().AddComponentA();
                systems.Execute();

                spy.didExecute.should_be(1);
            };
        };
    }
}

