using NSpec;
using Shouldly;
using Entitas.VisualDebugging.Unity;

class describe_SystemInfo : nspec {

    void when_SystemInfo() {

        it["creates systemInfo for initialize system"] = () => {
            var system = new TestInitializeSystem();
            var info = new SystemInfo(system);

            info.system.ShouldBeSameAs(system);
            info.systemName.ShouldBe("TestInitialize");

            info.isInitializeSystems.ShouldBeTrue();
            info.isExecuteSystems.ShouldBeFalse();
            info.isCleanupSystems.ShouldBeFalse();
            info.isTearDownSystems.ShouldBeFalse();
            info.isReactiveSystems.ShouldBeFalse();

            info.accumulatedExecutionDuration.ShouldBe(0);
            info.minExecutionDuration.ShouldBe(0);
            info.maxExecutionDuration.ShouldBe(0);
            info.averageExecutionDuration.ShouldBe(0);

            info.isActive.ShouldBeTrue();
        };

        it["creates systemInfo for execute system"] = () => {
            var system = new TestExecuteSystem();
            var info = new SystemInfo(system);

            info.systemName.ShouldBe("TestExecute");
            info.isInitializeSystems.ShouldBeFalse();
            info.isExecuteSystems.ShouldBeTrue();
            info.isCleanupSystems.ShouldBeFalse();
            info.isTearDownSystems.ShouldBeFalse();
            info.isReactiveSystems.ShouldBeFalse();
        };

        it["creates systemInfo for cleanup system"] = () => {
            var system = new TestCleanupSystem();
            var info = new SystemInfo(system);

            info.systemName.ShouldBe("TestCleanup");
            info.isInitializeSystems.ShouldBeFalse();
            info.isExecuteSystems.ShouldBeFalse();
            info.isCleanupSystems.ShouldBeTrue();
            info.isTearDownSystems.ShouldBeFalse();
            info.isReactiveSystems.ShouldBeFalse();
        };

        it["creates systemInfo for teardown system"] = () => {
            var system = new TestTearDownSystem();
            var info = new SystemInfo(system);

            info.systemName.ShouldBe("TestTearDown");
            info.isInitializeSystems.ShouldBeFalse();
            info.isExecuteSystems.ShouldBeFalse();
            info.isCleanupSystems.ShouldBeFalse();
            info.isTearDownSystems.ShouldBeTrue();
            info.isReactiveSystems.ShouldBeFalse();
        };

        it["creates systemInfo for reactive system"] = () => {
            var system = new TestReactiveSystem(new MyTestContext());
            var info = new SystemInfo(system);

            info.systemName.ShouldBe("TestReactive");

            info.isInitializeSystems.ShouldBeFalse();
            info.isExecuteSystems.ShouldBeFalse();
            info.isCleanupSystems.ShouldBeFalse();
            info.isTearDownSystems.ShouldBeFalse();
            info.isReactiveSystems.ShouldBeTrue();
        };

        xit["uses name of DebugSystem"] = () => {
            const string systemName = "My System";
            var system = new DebugSystems(systemName);
            var info = new SystemInfo(system);
            info.systemName.ShouldBe(systemName);
        };

        context["when created"] = () => {

            SystemInfo info = null;

            before = () => {
                info = new SystemInfo(new TestExecuteSystem());
            };

            it["adds execution duration"] = () => {
                info.AddExecutionDuration(42);

                info.accumulatedExecutionDuration.ShouldBe(42);
                info.minExecutionDuration.ShouldBe(42);
                info.maxExecutionDuration.ShouldBe(42);
                info.averageExecutionDuration.ShouldBe(42);
            };

            it["adds another execution duration"] = () => {
                info.AddExecutionDuration(20);
                info.AddExecutionDuration(10);

                info.accumulatedExecutionDuration.ShouldBe(30);
                info.minExecutionDuration.ShouldBe(10);
                info.maxExecutionDuration.ShouldBe(20);
                info.averageExecutionDuration.ShouldBe(15);
            };

            it["resets durations"] = () => {
                info.AddExecutionDuration(20);
                info.AddExecutionDuration(10);

                info.ResetDurations();

                info.accumulatedExecutionDuration.ShouldBe(0);
                info.minExecutionDuration.ShouldBe(10);
                info.maxExecutionDuration.ShouldBe(20);
                info.averageExecutionDuration.ShouldBe(0);
            };

            it["keeps min duration after reset"] = () => {
                info.AddExecutionDuration(20);
                info.AddExecutionDuration(10);

                info.ResetDurations();

                info.AddExecutionDuration(15);

                info.accumulatedExecutionDuration.ShouldBe(15);
                info.minExecutionDuration.ShouldBe(10);
                info.maxExecutionDuration.ShouldBe(20);
                info.averageExecutionDuration.ShouldBe(15);
            };
        };
    }
}
