using NSpec;
using Entitas;

class describe_Systems : nspec {

    static ReactiveSystem createReactiveSystem() {
        var subSystem = new ReactiveSubSystemSpy(Matcher.AllOf(new[] {
            CID.ComponentA
        }), GroupEventType.OnEntityAdded);
        var pool = new Pool(10);
        var reactiveSystem = new ReactiveSystem(pool, subSystem);
        pool.CreateEntity().AddComponentA();

        return reactiveSystem;
    }

    void when_systems() {

        context["fixtures"] = () => {
            it["starts StartSystemSpy"] = () => {
                var startSystem = new StartSystemSpy();
                startSystem.started.should_be_false();
                startSystem.Start();
                startSystem.started.should_be_true();
            };

            it["executes ExecuteSystemSpy"] = () => {
                var startSystem = new ExecuteSystemSpy();
                startSystem.executed.should_be_false();
                startSystem.Execute();
                startSystem.executed.should_be_true();
            };

            it["starts and executes StartExecuteSystemSpy"] = () => {
                var startSystem = new StartExecuteSystemSpy();
                startSystem.started.should_be_false();
                startSystem.executed.should_be_false();
                startSystem.Start();
                startSystem.Execute();
                startSystem.started.should_be_true();
                startSystem.executed.should_be_true();
            };

            it["executes ReactiveSystemSpy"] = () => {
                var system = createReactiveSystem();
                var spy = (ReactiveSubSystemSpy)system.subsystem;
                spy.didExecute.should_be(0);
                spy.started.should_be_false();
                system.Execute();
                spy.didExecute.should_be(1);
                spy.started.should_be_false();
            };
        };

        context["systems"] = () => {
            Systems systems = null;
            before = () => {
                systems = new Systems();
            };

            it["starts IStartSystem"] = () => {
                var system = new StartSystemSpy();
                systems.Add(system);
                systems.Start();
                system.started.should_be_true();
            };

            it["executes IExecuteSystem"] = () => {
                var system = new ExecuteSystemSpy();
                systems.Add(system);
                systems.Execute();
                system.executed.should_be_true();
            };

            it["starts and executes IStartSystem, IExecuteSystem"] = () => {
                var system = new StartExecuteSystemSpy();
                systems.Add(system);
                systems.Start();
                systems.Execute();
                system.started.should_be_true();
                system.executed.should_be_true();
            };

            it["starts and executes ReactiveSystem"] = () => {
                var system = createReactiveSystem();

                systems.Add(system);
                systems.Start();
                systems.Execute();
                systems.Execute();

                var spy = (ReactiveSubSystemSpy)system.subsystem;
                spy.didExecute.should_be(1);
                spy.started.should_be_true();
            };
        };
    }
}

