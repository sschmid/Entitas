using NSpec;
using Entitas.Unity.VisualDebugging;
using Entitas;

class describe_SystemInfo : nspec {

    void when_SystemInfo() {

        it["creates systemInfo for initialize system"] = () => {
            var system = new TestInitializeSystem();
            var info = new SystemInfo(system);

            info.system.should_be_same(system);
            info.systemName.should_be("TestInitialize");

            info.isInitializeSystems.should_be_true();
            info.isExecuteSystems.should_be_false();
            info.isCleanupSystems.should_be_false();
            info.isTearDownSystems.should_be_false();
            info.isReactiveSystems.should_be_false();

            info.accumulatedExecutionDuration.should_be(0);
            info.minExecutionDuration.should_be(0);
            info.maxExecutionDuration.should_be(0);
            info.averageExecutionDuration.should_be(0);

            info.isActive.should_be_true();
        };

        it["creates systemInfo for execute system"] = () => {
            var system = new TestExecuteSystem();
            var info = new SystemInfo(system);

            info.systemName.should_be("TestExecute");
            info.isInitializeSystems.should_be_false();
            info.isExecuteSystems.should_be_true();
            info.isCleanupSystems.should_be_false();
            info.isTearDownSystems.should_be_false();
            info.isReactiveSystems.should_be_false();
        };

        it["creates systemInfo for cleanup system"] = () => {
            var system = new TestCleanupSystem();
            var info = new SystemInfo(system);

            info.systemName.should_be("TestCleanup");
            info.isInitializeSystems.should_be_false();
            info.isExecuteSystems.should_be_false();
            info.isCleanupSystems.should_be_true();
            info.isTearDownSystems.should_be_false();
            info.isReactiveSystems.should_be_false();
        };

        it["creates systemInfo for teardown system"] = () => {
            var system = new TestTearDownSystem();
            var info = new SystemInfo(system);

            info.systemName.should_be("TestTearDown");
            info.isInitializeSystems.should_be_false();
            info.isExecuteSystems.should_be_false();
            info.isCleanupSystems.should_be_false();
            info.isTearDownSystems.should_be_true();
            info.isReactiveSystems.should_be_false();
        };

        it["creates systemInfo for reactive system"] = () => {
            var system = new ReactiveSystem(new TestReactiveSystem(), new Pools { test = new Pool(1) });
            var info = new SystemInfo(system);

            info.systemName.should_be("TestReactive");

            info.isInitializeSystems.should_be_false();
            info.isExecuteSystems.should_be_false();
            info.isCleanupSystems.should_be_false();
            info.isTearDownSystems.should_be_false();
            info.isReactiveSystems.should_be_true();
        };

        xit["uses name of DebugSystem"] = () => {
            const string systemName = "My System";
            var system = new DebugSystems(systemName);
            var info = new SystemInfo(system);
            info.systemName.should_be(systemName);
        };

        context["when created"] = () => {

            SystemInfo info = null;

            before = () => {
                info = new SystemInfo(new TestExecuteSystem());
            };

            it["adds execution duration"] = () => {
                info.AddExecutionDuration(42);

                info.accumulatedExecutionDuration.should_be(42);
                info.minExecutionDuration.should_be(42);
                info.maxExecutionDuration.should_be(42);
                info.averageExecutionDuration.should_be(42);
            };

            it["adds another execution duration"] = () => {
                info.AddExecutionDuration(20);
                info.AddExecutionDuration(10);

                info.accumulatedExecutionDuration.should_be(30);
                info.minExecutionDuration.should_be(10);
                info.maxExecutionDuration.should_be(20);
                info.averageExecutionDuration.should_be(15);
            };

            it["resets durations"] = () => {
                info.AddExecutionDuration(20);
                info.AddExecutionDuration(10);

                info.ResetDurations();

                info.accumulatedExecutionDuration.should_be(0);
                info.minExecutionDuration.should_be(10);
                info.maxExecutionDuration.should_be(20);
                info.averageExecutionDuration.should_be(0);
            };

            it["keeps min duration after reset"] = () => {
                info.AddExecutionDuration(20);
                info.AddExecutionDuration(10);

                info.ResetDurations();

                info.AddExecutionDuration(15);

                info.accumulatedExecutionDuration.should_be(15);
                info.minExecutionDuration.should_be(10);
                info.maxExecutionDuration.should_be(20);
                info.averageExecutionDuration.should_be(15);
            };
        };
    }
}
