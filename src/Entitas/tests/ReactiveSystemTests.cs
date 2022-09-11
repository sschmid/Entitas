using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class ReactiveSystemTests
    {
        readonly MyTestContext _context;

        public ReactiveSystemTests()
        {
            _context = new MyTestContext();
        }

        [Fact]
        public void DoesNotExecuteWhenNoEntitiesWereCollected()
        {
            var system = CreateAddedSystem();
            system.Execute();
            AssertEntities(system, null);
        }

        [Fact]
        public void ExecutesWhenTriggered()
        {
            var system = CreateAddedSystem();
            var e = CreateEntityAB();
            system.Execute();
            AssertEntities(system, e);
        }

        [Fact]
        public void ExecutesOnlyOnceWhenTriggered()
        {
            var system = CreateAddedSystem();
            var e = CreateEntityAB();
            system.Execute();
            system.Execute();
            AssertEntities(system, e);
        }

        [Fact]
        public void RetainsAndReleasesCollectedEntities()
        {
            var system = CreateAddedSystem();
            var e = CreateEntityAB();
            var retainCount = e.retainCount;
            system.Execute();
            retainCount.Should().Be(3); // retained by context, group and collector
            e.retainCount.Should().Be(2); // retained by context and group
        }

        [Fact]
        public void CollectsChangedEntitiesInExecute()
        {
            var system = CreateAddedSystem();
            var e = CreateEntityAB();
            system.executeAction = entities => { entities[0].ReplaceComponentA(Component.A); };
            system.Execute();
            system.Execute();
            AssertEntities(system, e, 2);
        }

        [Fact]
        public void CollectsCreatedEntitiesInExecute()
        {
            var system = CreateAddedSystem();
            var e1 = CreateEntityAB();
            TestEntity e2 = null;
            system.executeAction = delegate { e2 ??= CreateEntityAB(); };
            system.Execute();
            AssertEntities(system, e1);
            system.Execute();
            AssertEntities(system, e2, 2);
        }

        [Fact]
        public void DoesNotExecuteWhenNotTriggered()
        {
            var system = CreateAddedSystem();
            _context.CreateEntity().AddComponentA();
            system.Execute();
            AssertEntities(system, null);
        }

        [Fact]
        public void DeactivatesAndWillNotTrigger()
        {
            var system = CreateAddedSystem();
            system.Deactivate();
            CreateEntityAB();
            system.Execute();
            AssertEntities(system, null);
        }

        [Fact]
        public void ActivatesAndWillTriggerAgain()
        {
            var system = CreateAddedSystem();
            system.Deactivate();
            system.Activate();
            var e = CreateEntityAB();
            system.Execute();
            AssertEntities(system, e);
        }

        [Fact]
        public void ClearsCollectedEntities()
        {
            var system = CreateAddedSystem();
            CreateEntityAB();
            system.Clear();
            system.Execute();
            AssertEntities(system, null);
        }

        [Fact]
        public void CanToString()
        {
            CreateAddedSystem().ToString().Should().Be("ReactiveSystem(ReactiveSystemSpy)");
        }

        [Fact]
        public void RemovedExecutesWhenTriggered()
        {
            var system = CreateRemovedSystem();
            var e = CreateEntityAB()
                .RemoveComponentA();

            system.Execute();
            AssertEntities(system, e);
        }

        [Fact]
        public void RemovedExecutesOnlyOnceWhenTriggered()
        {
            var system = CreateRemovedSystem();
            var e = CreateEntityAB()
                .RemoveComponentA();

            system.Execute();
            system.Execute();
            AssertEntities(system, e);
        }

        [Fact]
        public void RemovedDoesNotExecuteWhenNotTriggered()
        {
            var system = CreateRemovedSystem();
            CreateEntityAB()
                .AddComponentC()
                .RemoveComponentC();

            system.Execute();
            AssertEntities(system, null);
        }

        [Fact]
        public void RetainsEntitiesUntilExecuteCompleted()
        {
            var system = CreateRemovedSystem();
            var e = CreateEntityAB();
            var didExecute = 0;
            system.executeAction = entities =>
            {
                didExecute += 1;
                entities[0].retainCount.Should().Be(1);
            };

            e.Destroy();
            system.Execute();
            didExecute.Should().Be(1);
            e.retainCount.Should().Be(0);
        }

        [Fact]
        public void AddedRemovedExecutesWhenAdded()
        {
            var system = CreateAddedRemovedSystem();
            var e = CreateEntityAB();
            system.Execute();
            AssertEntities(system, e);
        }

        [Fact]
        public void AddedRemovedExecutesWhenRemoved()
        {
            var system = CreateAddedRemovedSystem();
            var e = CreateEntityAB();
            system.Execute();
            e.RemoveComponentA();
            system.Execute();
            AssertEntities(system, e, 2);
        }

        [Fact]
        public void ExecutesWhenTriggeredOnMultipleContexts()
        {
            var context1 = new MyTestContext();
            var context2 = new MyTestContext();

            var groupA = context1.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
            var groupB = context2.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));

            var groups = new[] {groupA, groupB};
            var groupEvents = new[]
            {
                GroupEvent.Added,
                GroupEvent.Removed
            };
            var collector = new Collector<TestEntity>(groups, groupEvents);

            var system = new ReactiveSystemSpy(collector);

            var eA1 = context1.CreateEntity().AddComponentA();
            context2.CreateEntity().AddComponentA();

            var eB1 = context1.CreateEntity().AddComponentB();
            var eB2 = context2.CreateEntity().AddComponentB();

            system.Execute();
            AssertEntities(system, eA1);

            eB1.RemoveComponentB();
            eB2.RemoveComponentB();
            system.Execute();
            AssertEntities(system, eB2, 2);
        }

        [Fact]
        public void FiltersEntities()
        {
            var system = new ReactiveSystemSpy(
                _context.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB)),
                e => ((NameAgeComponent)e.GetComponent(CID.ComponentA)).age > 42
            );

            _context.CreateEntity()
                .AddComponentA()
                .AddComponentC();

            var e1 = _context.CreateEntity();
            e1.AddComponentB();
            e1.AddComponent(CID.ComponentA, new NameAgeComponent {age = 10});

            var e2 = _context.CreateEntity();
            e2.AddComponentB();
            e2.AddComponent(CID.ComponentA, new NameAgeComponent {age = 50});

            var didExecute = 0;
            system.executeAction = delegate
            {
                didExecute += 1;
                e2.retainCount.Should().Be(3); // retained by context, group and collector
            };

            system.Execute();
            didExecute.Should().Be(1);

            system.Execute();

            system.entities.Length.Should().Be(1);
            system.entities[0].Should().BeSameAs(e2);

            e1.retainCount.Should().Be(2); // retained by context and group
            e2.retainCount.Should().Be(2);
        }

        [Fact]
        public void ClearsReactiveSystemAfterExecute()
        {
            var system = new ReactiveSystemSpy(_context.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB)));
            system.executeAction = entities => { entities[0].ReplaceComponentA(Component.A); };
            var e = CreateEntityAB();
            system.Execute();
            system.Clear();
            system.Execute();
            AssertEntities(system, e);
        }

        ReactiveSystemSpy CreateAddedSystem() => new ReactiveSystemSpy(_context.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB)));
        ReactiveSystemSpy CreateRemovedSystem() => new ReactiveSystemSpy(_context.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB).Removed()));
        ReactiveSystemSpy CreateAddedRemovedSystem() => new ReactiveSystemSpy(_context.CreateCollector(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB).AddedOrRemoved()));

        TestEntity CreateEntityAB() => _context.CreateEntity()
            .AddComponentA()
            .AddComponentB();

        TestEntity CreateEntityAC() => _context.CreateEntity()
            .AddComponentA()
            .AddComponentC();

        TestEntity CreateEntityABC() => _context.CreateEntity()
            .AddComponentA()
            .AddComponentB()
            .AddComponentC();

        static void AssertEntities(IReactiveSystemSpy system, IEntity entity, int didExecute = 1)
        {
            if (entity == null)
            {
                system.didExecute.Should().Be(0);
                system.entities.Should().BeNull();
            }
            else
            {
                system.didExecute.Should().Be(didExecute);
                system.entities.Length.Should().Be(1);
                system.entities.Should().Contain(entity);
            }
        }
    }
}
