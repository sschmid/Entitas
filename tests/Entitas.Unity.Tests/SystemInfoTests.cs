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

            info.System.Should().BeSameAs(system);

            info.SystemName.Should().Be("TestInitialize");
            info.IsInitializeSystems.Should().BeTrue();
            info.IsExecuteSystems.Should().BeFalse();
            info.IsCleanupSystems.Should().BeFalse();
            info.IsTearDownSystems.Should().BeFalse();
            info.IsReactiveSystems.Should().BeFalse();

            info.AccumulatedExecutionDuration.Should().Be(0);
            info.MinExecutionDuration.Should().Be(0);
            info.MaxExecutionDuration.Should().Be(0);
            info.AverageExecutionDuration.Should().Be(0);

            info.IsActive.Should().BeTrue();
        }

        [Fact]
        public void CreatesSystemInfoForExecuteSystem()
        {
            var system = new TestExecuteSystem();
            var info = new SystemInfo(system);

            info.SystemName.Should().Be("TestExecute");
            info.IsInitializeSystems.Should().BeFalse();
            info.IsExecuteSystems.Should().BeTrue();
            info.IsCleanupSystems.Should().BeFalse();
            info.IsTearDownSystems.Should().BeFalse();
            info.IsReactiveSystems.Should().BeFalse();
        }

        [Fact]
        public void CreatesSystemInfoForCleanupSystem()
        {
            var system = new TestCleanupSystem();
            var info = new SystemInfo(system);

            info.SystemName.Should().Be("TestCleanup");
            info.IsInitializeSystems.Should().BeFalse();
            info.IsExecuteSystems.Should().BeFalse();
            info.IsCleanupSystems.Should().BeTrue();
            info.IsTearDownSystems.Should().BeFalse();
            info.IsReactiveSystems.Should().BeFalse();
        }

        [Fact]
        public void CreatesSystemInfoForTeardownSystem()
        {
            var system = new TestTearDownSystem();
            var info = new SystemInfo(system);

            info.SystemName.Should().Be("TestTearDown");
            info.IsInitializeSystems.Should().BeFalse();
            info.IsExecuteSystems.Should().BeFalse();
            info.IsCleanupSystems.Should().BeFalse();
            info.IsTearDownSystems.Should().BeTrue();
            info.IsReactiveSystems.Should().BeFalse();
        }

        [Fact]
        public void CreatesSystemInfoForReactiveSystem()
        {
            var system = new TestReactiveSystem(new Context<Entity>(1, () => new Entity()));
            var info = new SystemInfo(system);

            info.SystemName.Should().Be("TestReactive");
            info.IsInitializeSystems.Should().BeFalse();
            info.IsExecuteSystems.Should().BeFalse();
            info.IsCleanupSystems.Should().BeFalse();
            info.IsTearDownSystems.Should().BeFalse();
            info.IsReactiveSystems.Should().BeTrue();
        }

        [Fact(Skip = "Needs Unity")]
        public void UsesNameOfDebugSystem()
        {
            const string systemName = "My System";
            var system = new DebugSystems(systemName);
            var info = new SystemInfo(system);
            info.SystemName.Should().Be(systemName);
        }

        [Fact]
        public void AddsExecutionDuration()
        {
            _systemInfo.AddExecutionDuration(42);
            _systemInfo.AccumulatedExecutionDuration.Should().Be(42);
            _systemInfo.MinExecutionDuration.Should().Be(42);
            _systemInfo.MaxExecutionDuration.Should().Be(42);
            _systemInfo.AverageExecutionDuration.Should().Be(42);
        }

        [Fact]
        public void AddsCleanupDuration()
        {
            _systemInfo.AddCleanupDuration(42);
            _systemInfo.AccumulatedCleanupDuration.Should().Be(42);
            _systemInfo.MinCleanupDuration.Should().Be(42);
            _systemInfo.MaxCleanupDuration.Should().Be(42);
            _systemInfo.AverageCleanupDuration.Should().Be(42);
        }

        [Fact]
        public void AddsAnotherExecutionDuration()
        {
            _systemInfo.AddExecutionDuration(20);
            _systemInfo.AddExecutionDuration(10);
            _systemInfo.AccumulatedExecutionDuration.Should().Be(30);
            _systemInfo.MinExecutionDuration.Should().Be(10);
            _systemInfo.MaxExecutionDuration.Should().Be(20);
            _systemInfo.AverageExecutionDuration.Should().Be(15);
        }

        [Fact]
        public void AddsAnotherCleanupDuration()
        {
            _systemInfo.AddCleanupDuration(20);
            _systemInfo.AddCleanupDuration(10);
            _systemInfo.AccumulatedCleanupDuration.Should().Be(30);
            _systemInfo.MinCleanupDuration.Should().Be(10);
            _systemInfo.MaxCleanupDuration.Should().Be(20);
            _systemInfo.AverageCleanupDuration.Should().Be(15);
        }

        [Fact]
        public void ResetsDurations()
        {
            _systemInfo.AddExecutionDuration(20);
            _systemInfo.AddExecutionDuration(10);
            _systemInfo.AddCleanupDuration(200);
            _systemInfo.AddCleanupDuration(100);
            _systemInfo.ResetDurations();

            _systemInfo.AccumulatedExecutionDuration.Should().Be(0);
            _systemInfo.MinExecutionDuration.Should().Be(10);
            _systemInfo.MaxExecutionDuration.Should().Be(20);
            _systemInfo.AverageExecutionDuration.Should().Be(0);

            _systemInfo.AccumulatedCleanupDuration.Should().Be(0);
            _systemInfo.MinCleanupDuration.Should().Be(100);
            _systemInfo.MaxCleanupDuration.Should().Be(200);
            _systemInfo.AverageCleanupDuration.Should().Be(0);
        }

        [Fact]
        public void KeepsMinDurationAfterReset()
        {
            _systemInfo.AddExecutionDuration(20);
            _systemInfo.AddExecutionDuration(10);
            _systemInfo.AddCleanupDuration(200);
            _systemInfo.AddCleanupDuration(100);
            _systemInfo.ResetDurations();

            _systemInfo.AddExecutionDuration(15);
            _systemInfo.AddCleanupDuration(150);

            _systemInfo.AccumulatedExecutionDuration.Should().Be(15);
            _systemInfo.MinExecutionDuration.Should().Be(10);
            _systemInfo.MaxExecutionDuration.Should().Be(20);
            _systemInfo.AverageExecutionDuration.Should().Be(15);

            _systemInfo.AccumulatedCleanupDuration.Should().Be(150);
            _systemInfo.MinCleanupDuration.Should().Be(100);
            _systemInfo.MaxCleanupDuration.Should().Be(200);
            _systemInfo.AverageCleanupDuration.Should().Be(150);
        }
    }
}
