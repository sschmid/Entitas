using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class EntityIndexTests
    {
        const string Name = "Max";

        readonly IContext<TestEntity> _context;
        readonly EntityIndex<TestEntity, string> _index;
        readonly IContext<TestEntity> _multiKeyContext;
        readonly EntityIndex<TestEntity, string> _multiKeyIndex;

        public EntityIndexTests()
        {
            _context = new MyTestContext();
            _index = CreateEntityIndex();
            _multiKeyContext = new MyTestContext();
            _multiKeyIndex = CreateMultiKeyEntityIndex();
        }

        [Fact]
        public void HasNoEntities()
        {
            _index.GetEntities("unknownKey").Should().BeEmpty();
        }

        [Fact]
        public void GetsEntitiesForKey()
        {
            var e1 = CreateNameAgeEntity(_context, 1);
            var e2 = CreateNameAgeEntity(_context, 2);
            var entities = _index.GetEntities(Name);
            entities.Count.Should().Be(2);
            entities.Should().Contain(e1);
            entities.Should().Contain(e2);
        }

        [Fact]
        public void MultiKeyGetsEntityForKey()
        {
            var e1 = CreateNameAgeEntity(_multiKeyContext, 1);
            var e2 = CreateNameAgeEntity(_multiKeyContext, 2);
            _multiKeyIndex.GetEntities("1").First().Should().BeSameAs(e1);
            _multiKeyIndex.GetEntities("2").Should().Contain(e1);
            _multiKeyIndex.GetEntities("2").Should().Contain(e2);
            _multiKeyIndex.GetEntities("3").First().Should().BeSameAs(e2);
        }

        [Fact]
        public void RetainsEntity()
        {
            var e1 = CreateNameAgeEntity(_context, 1);
            var e2 = CreateNameAgeEntity(_context, 2);
            e1.retainCount.Should().Be(3); // Context, Group, EntityIndex
            e2.retainCount.Should().Be(3); // Context, Group, EntityIndex
        }

        [Fact]
        public void MultiKeyRetainsEntity()
        {
            var e1 = CreateNameAgeEntity(_multiKeyContext, 1);
            var e2 = CreateNameAgeEntity(_multiKeyContext, 2);
            e1.retainCount.Should().Be(3);
            e2.retainCount.Should().Be(3);
            (e1.aerc as SafeAERC)?.owners.Should().Contain(_multiKeyIndex);
            (e1.aerc as SafeAERC)?.owners.Should().Contain(_multiKeyIndex);
        }

        [Fact]
        public void HasExistingEntities()
        {
            CreateNameAgeEntity(_context, 1);
            CreateNameAgeEntity(_context, 2);
            CreateEntityIndex().GetEntities(Name).Count.Should().Be(2);
        }

        [Fact]
        public void MultiKeyHasExistingEntities()
        {
            CreateNameAgeEntity(_multiKeyContext, 1);
            CreateNameAgeEntity(_multiKeyContext, 2);
            _multiKeyIndex.GetEntities("1").Count.Should().Be(1);
            _multiKeyIndex.GetEntities("2").Count.Should().Be(2);
            _multiKeyIndex.GetEntities("3").Count.Should().Be(1);
        }

        [Fact]
        public void ReleasesAndRemovesEntityFromIndexWhenComponentGetsRemoved()
        {
            var e1 = CreateNameAgeEntity(_context, 1);
            CreateNameAgeEntity(_context, 2);
            e1.RemoveComponentA();
            _index.GetEntities(Name).Count.Should().Be(1);
            e1.retainCount.Should().Be(1); // Context
            (e1.aerc as SafeAERC)?.owners.Should().NotContain(_multiKeyIndex);
        }

        [Fact]
        public void MultiKeyReleasesAndRemovesEntityFromIndexWhenComponentGetsRemoved()
        {
            var e1 = CreateNameAgeEntity(_multiKeyContext, 1);
            var e2 = CreateNameAgeEntity(_multiKeyContext, 2);
            e1.RemoveComponentA();
            _multiKeyIndex.GetEntities("1").Count.Should().Be(0);
            _multiKeyIndex.GetEntities("2").Count.Should().Be(1);
            _multiKeyIndex.GetEntities("3").Count.Should().Be(1);
            e1.retainCount.Should().Be(1);
            e2.retainCount.Should().Be(3);
            (e1.aerc as SafeAERC)?.owners.Should().NotContain(_multiKeyIndex);
            (e2.aerc as SafeAERC)?.owners.Should().Contain(_multiKeyIndex);
        }

        [Fact]
        public void CanToString()
        {
            _index.ToString().Should().Be("EntityIndex(TestIndex)");
        }

        [Fact]
        public void ClearsIndexAndReleasesEntity()
        {
            var e1 = CreateNameAgeEntity(_context, 1);
            var e2 = CreateNameAgeEntity(_context, 2);
            _index.Deactivate();
            _index.GetEntities(Name).Should().BeEmpty();
            e1.retainCount.Should().Be(2); // Context, Group
            e2.retainCount.Should().Be(2); // Context, Group
        }

        [Fact]
        public void DoesNotAddEntitiesAnymore()
        {
            _index.Deactivate();
            CreateNameAgeEntity(_context, 1);
            _index.GetEntities(Name).Should().BeEmpty();
        }

        [Fact]
        public void HasExistingEntitiesWhenActivating()
        {
            var e1 = CreateNameAgeEntity(_context, 1);
            var e2 = CreateNameAgeEntity(_context, 2);
            _index.Deactivate();
            _index.Activate();
            var entities = _index.GetEntities(Name);
            entities.Count.Should().Be(2);
            entities.Should().Contain(e1);
            entities.Should().Contain(e2);
        }

        [Fact]
        public void MultiKeyHasExistingEntitiesWhenActivating()
        {
            var e1 = CreateNameAgeEntity(_multiKeyContext, 1);
            var e2 = CreateNameAgeEntity(_multiKeyContext, 2);
            _multiKeyIndex.Deactivate();
            _multiKeyIndex.Activate();
            _multiKeyIndex.GetEntities("1").First().Should().BeSameAs(e1);
            _multiKeyIndex.GetEntities("2").Should().Contain(e1);
            _multiKeyIndex.GetEntities("2").Should().Contain(e2);
            _multiKeyIndex.GetEntities("3").First().Should().BeSameAs(e2);
        }

        [Fact]
        public void AddsNewEntitiesWhenActivated()
        {
            var e1 = CreateNameAgeEntity(_context, 1);
            var e2 = CreateNameAgeEntity(_context, 2);
            _index.Deactivate();
            _index.Activate();
            var e3 = CreateNameAgeEntity(_context, 3);

            var entities = _index.GetEntities(Name);
            entities.Count.Should().Be(3);
            entities.Should().Contain(e1);
            entities.Should().Contain(e2);
            entities.Should().Contain(e3);
        }

        [Fact]
        public void GetsLastComponentThatTriggeredAddingEntityToGroup()
        {
            IComponent lastComponent = null;

            var group = _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB));
            new EntityIndex<TestEntity, string>(
                "TestIndex",
                group, (_, c) =>
                {
                    lastComponent = c;
                    return ((NameAgeComponent)c).name;
                });

            var nameAge1 = new NameAgeComponent();
            nameAge1.name = "Max";

            var nameAge2 = new NameAgeComponent();
            nameAge2.name = "Jack";

            var entity = _context.CreateEntity();
            entity.AddComponent(CID.ComponentA, nameAge1);
            entity.AddComponent(CID.ComponentB, nameAge2);

            lastComponent.Should().BeSameAs(nameAge2);
        }

        [Fact]
        public void WorksWithNoneOf()
        {
            var lastComponents = new List<IComponent>();

            var nameAge1 = new NameAgeComponent();
            nameAge1.name = "Max";
            var nameAge2 = new NameAgeComponent();
            nameAge2.name = "Jack";

            var index = new EntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA).NoneOf(CID.ComponentB)),
                (e, c) =>
                {
                    lastComponents.Add(c);
                    return c == nameAge1
                        ? ((NameAgeComponent)c).name
                        : ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
                }
            );

            var entity = _context.CreateEntity();
            entity.AddComponent(CID.ComponentA, nameAge1);
            entity.AddComponent(CID.ComponentB, nameAge2);

            lastComponents.Count.Should().Be(2);
            lastComponents[0].Should().Be(nameAge1);
            lastComponents[1].Should().Be(nameAge2);

            index.GetEntities("Max").Should().BeEmpty();
            index.GetEntities("Jack").Should().BeEmpty();
        }

        EntityIndex<TestEntity, string> CreateEntityIndex() =>
            new EntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA)),
                (e, c) => (c as NameAgeComponent)?.name ?? ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name);

        EntityIndex<TestEntity, string> CreateMultiKeyEntityIndex() => new EntityIndex<TestEntity, string>(
            "TestIndex",
            _multiKeyContext.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA)),
            (e, c) => (c as NameAgeComponent ?? (NameAgeComponent)e.GetComponent(CID.ComponentA)).age == 1
                ? new[] {"1", "2"}
                : new[] {"2", "3"});

        static TestEntity CreateNameAgeEntity(IContext<TestEntity> context, int age)
        {
            var nameAgeComponent = new NameAgeComponent();
            nameAgeComponent.name = Name;
            nameAgeComponent.age = age;
            var entity = context.CreateEntity();
            entity.AddComponent(CID.ComponentA, nameAgeComponent);
            return entity;
        }
    }
}
