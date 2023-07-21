using FluentAssertions;
using Xunit;

namespace Entitas.Unity.Tests
{
    public class SystemInfoTests
    {
        readonly SystemInfo _systemInfo;

        public SystemInfoTests()
        {
            _systemInfo = new SystemInfo(new TestExecuteSystem());
        }

        [Fact]
        public void CreatesSystemInfoForInitializeSystem()
        {
            var system = new TestInitializeSystem();
            var info = new SystemInfo(system);

            info.system.Should().BeSameAs(system);
            info.systemName.Should().Be("TestInitialize");

            info.isInitializeSystems.Should().BeTrue();
            info.isExecuteSystems.Should().BeFalse();
            info.isCleanupSystems.Should().BeFalse();
            info.isTearDownSystems.Should().BeFalse();
            info.isReactiveSystems.Should().BeFalse();

            info.accumulatedExecutionDuration.Should().Be(0);
            info.minExecutionDuration.Should().Be(0);
            info.maxExecutionDuration.Should().Be(0);
            info.averageExecutionDuration.Should().Be(0);

            info.isActive.Should().BeTrue();
        }

        [Fact]
        public void CreatesSystemInfoForExecuteSystem()
        {
            var system = new TestExecuteSystem();
            var info = new SystemInfo(system);

            info.systemName.Should().Be("TestExecute");
            info.isInitializeSystems.Should().BeFalse();
            info.isExecuteSystems.Should().BeTrue();
            info.isCleanupSystems.Should().BeFalse();
            info.isTearDownSystems.Should().BeFalse();
            info.isReactiveSystems.Should().BeFalse();
        }

        [Fact]
        public void CreatesSystemInfoForCleanupSystem()
        {
            var system = new TestCleanupSystem();
            var info = new SystemInfo(system);

            info.systemName.Should().Be("TestCleanup");
            info.isInitializeSystems.Should().BeFalse();
            info.isExecuteSystems.Should().BeFalse();
            info.isCleanupSystems.Should().BeTrue();
            info.isTearDownSystems.Should().BeFalse();
            info.isReactiveSystems.Should().BeFalse();
        }

        [Fact]
        public void CreatesSystemInfoForTeardownSystem()
        {
            var system = new TestTearDownSystem();
            var info = new SystemInfo(system);

            info.systemName.Should().Be("TestTearDown");
            info.isInitializeSystems.Should().BeFalse();
            info.isExecuteSystems.Should().BeFalse();
            info.isCleanupSystems.Should().BeFalse();
            info.isTearDownSystems.Should().BeTrue();
            info.isReactiveSystems.Should().BeFalse();
        }

        [Fact]
        public void CreatesSystemInfoForReactiveSystem()
        {
            var system = new TestReactiveSystem(new TestContext(1));
            var info = new SystemInfo(system);

            info.systemName.Should().Be("TestReactive");

            info.isInitializeSystems.Should().BeFalse();
            info.isExecuteSystems.Should().BeFalse();
            info.isCleanupSystems.Should().BeFalse();
            info.isTearDownSystems.Should().BeFalse();
            info.isReactiveSystems.Should().BeTrue();
        }

        [Fact(Skip = "needs Unity")]
        public void UsesNameOfDebugSystem()
        {
            const string systemName = "My System";
            var system = new DebugSystems(systemName);
            var info = new SystemInfo(system);
            info.systemName.Should().Be(systemName);
        }

        [Fact]
        public void AddsExecutionDuration()
        {
            _systemInfo.AddExecutionDuration(42);
            _systemInfo.accumulatedExecutionDuration.Should().Be(42);
            _systemInfo.minExecutionDuration.Should().Be(42);
            _systemInfo.maxExecutionDuration.Should().Be(42);
            _systemInfo.averageExecutionDuration.Should().Be(42);
        }

        [Fact]
        public void AddsAnotherExecutionDuration()
        {
            _systemInfo.AddExecutionDuration(20);
            _systemInfo.AddExecutionDuration(10);
            _systemInfo.accumulatedExecutionDuration.Should().Be(30);
            _systemInfo.minExecutionDuration.Should().Be(10);
            _systemInfo.maxExecutionDuration.Should().Be(20);
            _systemInfo.averageExecutionDuration.Should().Be(15);
        }

        [Fact]
        public void ResetsDurations()
        {
            _systemInfo.AddExecutionDuration(20);
            _systemInfo.AddExecutionDuration(10);
            _systemInfo.ResetDurations();

            _systemInfo.accumulatedExecutionDuration.Should().Be(0);
            _systemInfo.minExecutionDuration.Should().Be(10);
            _systemInfo.maxExecutionDuration.Should().Be(20);
            _systemInfo.averageExecutionDuration.Should().Be(0);
        }

        [Fact]
        public void KeepsMinDurationAfterReset()
        {
            _systemInfo.AddExecutionDuration(20);
            _systemInfo.AddExecutionDuration(10);
            _systemInfo.ResetDurations();

            _systemInfo.AddExecutionDuration(15);
            _systemInfo.accumulatedExecutionDuration.Should().Be(15);
            _systemInfo.minExecutionDuration.Should().Be(10);
            _systemInfo.maxExecutionDuration.Should().Be(20);
            _systemInfo.averageExecutionDuration.Should().Be(15);
        }
    }
}
