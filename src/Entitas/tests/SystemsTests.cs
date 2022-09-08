using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class SystemsTests
    {
        readonly MyTestContext _context;
        readonly Systems _systems;

        public SystemsTests()
        {
            _context = new MyTestContext();
            _systems = new Systems();
        }

        [Fact]
        public void InitializesInitializeSystemSpy()
        {
            var system = new InitializeSystemSpy();
            system.didInitialize.Should().Be(0);
            system.Initialize();
            system.didInitialize.Should().Be(1);
        }

        [Fact]
        public void ExecutesExecuteSystemSpy()
        {
            var system = new ExecuteSystemSpy();
            system.didExecute.Should().Be(0);
            system.Execute();
            system.didExecute.Should().Be(1);
        }

        [Fact]
        public void CleansUpCleanupSystemSpy()
        {
            var system = new CleanupSystemSpy();
            system.didCleanup.Should().Be(0);
            system.Cleanup();
            system.didCleanup.Should().Be(1);
        }

        [Fact]
        public void TearsDownTearDownSystemSpy()
        {
            var system = new TearDownSystemSpy();
            system.didTearDown.Should().Be(0);
            system.TearDown();
            system.didTearDown.Should().Be(1);
        }

        [Fact]
        public void InitializesExecutesCleansUpAndTearsDownSystem()
        {
            var system = CreateReactiveSystem();

            system.didInitialize.Should().Be(0);
            system.Initialize();
            system.didInitialize.Should().Be(1);

            system.didExecute.Should().Be(0);
            system.Execute();
            system.didExecute.Should().Be(1);
            system.entities.Length.Should().Be(1);

            system.didCleanup.Should().Be(0);
            system.Cleanup();
            system.didCleanup.Should().Be(1);

            system.didTearDown.Should().Be(0);
            system.TearDown();
            system.didTearDown.Should().Be(1);
        }

        [Fact]
        public void ReturnsSystemsWhenAddingSystem()
        {
            _systems.Add(new InitializeSystemSpy()).Should().BeSameAs(_systems);
        }

        [Fact]
        public void InitializesIInitializeSystem()
        {
            var system = new InitializeSystemSpy();
            _systems.Add(system);
            _systems.Initialize();
            system.didInitialize.Should().Be(1);
        }

        [Fact]
        public void RemovesIInitializeSystem()
        {
            var system = new InitializeSystemSpy();
            _systems.Add(system);
            _systems.Remove(system);
            _systems.Initialize();
            system.didInitialize.Should().Be(0);
        }

        [Fact]
        public void ExecutesIExecuteSystem()
        {
            var system = new ExecuteSystemSpy();
            _systems.Add(system);
            _systems.Execute();
            system.didExecute.Should().Be(1);
        }

        [Fact]
        public void RemovesIExecuteSystem()
        {
            var system = new ExecuteSystemSpy();
            _systems.Add(system);
            _systems.Remove(system);
            _systems.Execute();
            system.didExecute.Should().Be(0);
        }

        [Fact]
        public void AddsReactiveSystem()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);
            _systems.Execute();
            system.didExecute.Should().Be(1);
        }

        [Fact]
        public void RemovesReactiveSystem()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);
            _systems.Remove(system);
            _systems.Execute();
            system.didExecute.Should().Be(0);
        }

        [Fact]
        public void CleansUpICleanupSystem()
        {
            var system = new CleanupSystemSpy();
            _systems.Add(system);
            _systems.Cleanup();
            system.didCleanup.Should().Be(1);
        }

        [Fact]
        public void RemovesICleanupSystem()
        {
            var system = new CleanupSystemSpy();
            _systems.Add(system);
            _systems.Remove(system);
            _systems.Cleanup();
            system.didCleanup.Should().Be(0);
        }

        [Fact]
        public void TearsDownITearDownSystem()
        {
            var system = new TearDownSystemSpy();
            _systems.Add(system);
            _systems.TearDown();
            system.didTearDown.Should().Be(1);
        }

        [Fact]
        public void RemovesITearDownSystem()
        {
            var system = new TearDownSystemSpy();
            _systems.Add(system);
            _systems.Remove(system);
            _systems.TearDown();
            system.didTearDown.Should().Be(0);
        }

        [Fact]
        public void RemovesMixedSystem()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);
            _systems.Remove(system);

            _systems.Initialize();
            system.didInitialize.Should().Be(0);

            _systems.Execute();
            system.didExecute.Should().Be(0);

            _systems.Cleanup();
            system.didCleanup.Should().Be(0);

            _systems.TearDown();
            system.didTearDown.Should().Be(0);
        }

        [Fact]
        public void InitializesExecutesCleansUpAndTearsDownSystems()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);

            system.didInitialize.Should().Be(0);
            _systems.Initialize();
            system.didInitialize.Should().Be(1);

            system.didExecute.Should().Be(0);
            _systems.Execute();
            system.didExecute.Should().Be(1);

            system.didCleanup.Should().Be(0);
            _systems.Cleanup();
            system.didCleanup.Should().Be(1);

            system.didTearDown.Should().Be(0);
            _systems.TearDown();
            system.didTearDown.Should().Be(1);
        }

        [Fact]
        public void InitializesExecutesCleansUpAndTearsDownSystemsRecursively()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);

            var parentSystems = new Systems();
            parentSystems.Add(_systems);

            system.didInitialize.Should().Be(0);
            parentSystems.Initialize();
            system.didInitialize.Should().Be(1);

            system.didExecute.Should().Be(0);
            parentSystems.Execute();
            parentSystems.Execute();
            system.didExecute.Should().Be(1);

            system.didCleanup.Should().Be(0);
            parentSystems.Cleanup();
            system.didCleanup.Should().Be(1);

            system.didTearDown.Should().Be(0);
            parentSystems.TearDown();
            system.didTearDown.Should().Be(1);
        }

        [Fact]
        public void ClearsReactiveSystems()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);

            _systems.Initialize();
            system.didInitialize.Should().Be(1);

            _systems.ClearReactiveSystems();
            _systems.Execute();
            system.didExecute.Should().Be(0);
        }

        [Fact]
        public void ClearsReactiveSystemsRecursively()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);

            var parentSystems = new Systems();
            parentSystems.Add(_systems);

            parentSystems.Initialize();
            system.didInitialize.Should().Be(1);

            parentSystems.ClearReactiveSystems();
            parentSystems.Execute();
            system.didExecute.Should().Be(0);
        }

        [Fact]
        public void DeactivatesReactiveSystems()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);

            _systems.Initialize();
            system.didInitialize.Should().Be(1);

            _systems.DeactivateReactiveSystems();
            _systems.Execute();
            system.didExecute.Should().Be(0);
        }

        [Fact]
        public void DeactivatesReactiveSystemsRecursively()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);

            var parentSystems = new Systems();
            parentSystems.Add(_systems);

            parentSystems.Initialize();
            system.didInitialize.Should().Be(1);

            parentSystems.DeactivateReactiveSystems();
            parentSystems.Execute();
            system.didExecute.Should().Be(0);
        }

        [Fact]
        public void ActivatesReactiveSystems()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);

            _systems.Initialize();
            system.didInitialize.Should().Be(1);

            _systems.DeactivateReactiveSystems();
            _systems.ActivateReactiveSystems();
            _systems.Execute();
            system.didExecute.Should().Be(0);

            _context.CreateEntity().AddComponentA();
            _systems.Execute();
            system.didExecute.Should().Be(1);
        }

        [Fact]
        public void ActivatesReactiveSystemsRecursively()
        {
            var system = CreateReactiveSystem();
            _systems.Add(system);

            var parentSystems = new Systems();
            parentSystems.Add(_systems);

            parentSystems.Initialize();
            system.didInitialize.Should().Be(1);

            parentSystems.DeactivateReactiveSystems();
            parentSystems.ActivateReactiveSystems();
            parentSystems.Execute();
            system.didExecute.Should().Be(0);

            _context.CreateEntity().AddComponentA();
            _systems.Execute();
            system.didExecute.Should().Be(1);
        }

        ReactiveSystemSpy CreateReactiveSystem()
        {
            var system = new ReactiveSystemSpy(_context.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA)));
            _context.CreateEntity().AddComponentA();
            return system;
        }
    }
}
