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

            it["has no systems"] = () => {
                systems.startSystemsCount.should_be(0);
                systems.executeSystemsCount.should_be(0);
            };

            it["returns systems when adding system"] = () => {
                systems.Add(new StartSystemSpy()).should_be_same(systems);
            };

            it["has systems count when adding IStartSystem"] = () => {
                systems.Add(new StartSystemSpy());
                systems.startSystemsCount.should_be(1);
                systems.executeSystemsCount.should_be(0);
            };

            it["contains IStartSystem when added"] = () => {
                var system = new StartSystemSpy();
                systems.Add(system);
                systems.systems.should_contain(system);
                systems.systems.Length.should_be(1);
            };

            it["updates systems cache"] = () => {
                var system = new StartSystemSpy();
                systems.Add(system);
                var s = systems.systems;
                systems.Add(system);
                systems.systems.should_contain(system);
                systems.systems.Length.should_be(2);
                systems.systems.should_not_be_same(s);
            };

            it["starts IStartSystem"] = () => {
                var system = new StartSystemSpy();
                systems.Add(system);
                systems.Start();
                system.started.should_be_true();
            };

            it["has systems count when adding IExecuteSystem"] = () => {
                systems.Add(new ExecuteSystemSpy());
                systems.startSystemsCount.should_be(0);
                systems.executeSystemsCount.should_be(1);
            };

            it["contains IExecuteSystem when added"] = () => {
                var system = new ExecuteSystemSpy();
                systems.Add(system);
                systems.systems.should_contain(system);
                systems.systems.Length.should_be(1);
            };

            it["executes IExecuteSystem"] = () => {
                var system = new ExecuteSystemSpy();
                systems.Add(system);
                systems.Execute();
                system.executed.should_be_true();
            };

            it["has systems count when adding IStartSystem, IExecuteSystem"] = () => {
                systems.Add(new StartExecuteSystemSpy());
                systems.startSystemsCount.should_be(1);
                systems.executeSystemsCount.should_be(1);
            };

            it["contains IStartSystem, IExecuteSystem when added"] = () => {
                var system = new StartExecuteSystemSpy();
                systems.Add(system);
                systems.systems.should_contain(system);
                systems.systems.Length.should_be(1);
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

