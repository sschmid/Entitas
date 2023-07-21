using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class EntityIndexTests
    {
        readonly IContext<TestEntity> _context;
        readonly EntityIndex<TestEntity, string> _index;
        readonly IContext<TestEntity> _multiKeyContext;
        readonly EntityIndex<TestEntity, string> _multiKeyIndex;

        public EntityIndexTests()
        {
            _context = new TestContext(CID.TotalComponents);
            _index = CreateEntityIndex();
            _multiKeyContext = new TestContext(CID.TotalComponents);
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
            var entity1 = _context.CreateEntity().AddUser("Test", 1);
            var entity2 = _context.CreateEntity().AddUser("Test", 2);
            var entities = _index.GetEntities("Test");
            entities.Count.Should().Be(2);
            entities.Should().Contain(entity1);
            entities.Should().Contain(entity2);
        }

        [Fact]
        public void MultiKeyGetsEntityForKey()
        {
            var entity1 = _multiKeyContext.CreateEntity().AddUser("Test", 1);
            var entity2 = _multiKeyContext.CreateEntity().AddUser("Test", 2);
            _multiKeyIndex.GetEntities("1").First().Should().BeSameAs(entity1);
            _multiKeyIndex.GetEntities("2").Should().Contain(entity1);
            _multiKeyIndex.GetEntities("2").Should().Contain(entity2);
            _multiKeyIndex.GetEntities("3").First().Should().BeSameAs(entity2);
        }

        [Fact]
        public void RetainsEntity()
        {
            var entity1 = _context.CreateEntity().AddUser("Test", 1);
            var entity2 = _context.CreateEntity().AddUser("Test", 2);
            entity1.RetainCount.Should().Be(3); // Context, Group, EntityIndex
            entity2.RetainCount.Should().Be(3); // Context, Group, EntityIndex
        }

        [Fact]
        public void MultiKeyRetainsEntity()
        {
            var entity1 = _multiKeyContext.CreateEntity().AddUser("Test", 1);
            var entity2 = _multiKeyContext.CreateEntity().AddUser("Test", 2);
            entity1.RetainCount.Should().Be(3);
            entity2.RetainCount.Should().Be(3);
            (entity1.Aerc as SafeAERC)?.Owners.Should().Contain(_multiKeyIndex);
            (entity1.Aerc as SafeAERC)?.Owners.Should().Contain(_multiKeyIndex);
        }

        [Fact]
        public void HasExistingEntities()
        {
            _context.CreateEntity().AddUser("Test", 1);
            _context.CreateEntity().AddUser("Test", 2);
            CreateEntityIndex().GetEntities("Test").Count.Should().Be(2);
        }

        [Fact]
        public void MultiKeyHasExistingEntities()
        {
            _multiKeyContext.CreateEntity().AddUser("Test", 1);
            _multiKeyContext.CreateEntity().AddUser("Test", 2);
            _multiKeyIndex.GetEntities("1").Count.Should().Be(1);
            _multiKeyIndex.GetEntities("2").Count.Should().Be(2);
            _multiKeyIndex.GetEntities("3").Count.Should().Be(1);
        }

        [Fact]
        public void ReleasesAndRemovesEntityFromIndexWhenComponentGetsRemoved()
        {
            var entity1 = _context.CreateEntity().AddUser("Test", 1);
            _context.CreateEntity().AddUser("Test", 2);
            entity1.RemoveUser();
            _index.GetEntities("Test").Count.Should().Be(1);
            entity1.RetainCount.Should().Be(1); // Context
            (entity1.Aerc as SafeAERC)?.Owners.Should().NotContain(_multiKeyIndex);
        }

        [Fact]
        public void MultiKeyReleasesAndRemovesEntityFromIndexWhenComponentGetsRemoved()
        {
            var entity1 = _multiKeyContext.CreateEntity().AddUser("Test", 1);
            var entity2 = _multiKeyContext.CreateEntity().AddUser("Test", 2);
            entity1.RemoveUser();
            _multiKeyIndex.GetEntities("1").Count.Should().Be(0);
            _multiKeyIndex.GetEntities("2").Count.Should().Be(1);
            _multiKeyIndex.GetEntities("3").Count.Should().Be(1);
            entity1.RetainCount.Should().Be(1);
            entity2.RetainCount.Should().Be(3);
            (entity1.Aerc as SafeAERC)?.Owners.Should().NotContain(_multiKeyIndex);
            (entity2.Aerc as SafeAERC)?.Owners.Should().Contain(_multiKeyIndex);
        }

        [Fact]
        public void CanToString()
        {
            _index.ToString().Should().Be("EntityIndex(TestIndex)");
        }

        [Fact]
        public void ClearsIndexAndReleasesEntity()
        {
            var entity1 = _context.CreateEntity().AddUser("Test", 1);
            var entity2 = _context.CreateEntity().AddUser("Test", 2);
            _index.Deactivate();
            _index.GetEntities("Test").Should().BeEmpty();
            entity1.RetainCount.Should().Be(2); // Context, Group
            entity2.RetainCount.Should().Be(2); // Context, Group
        }

        [Fact]
        public void DoesNotAddEntitiesAnymore()
        {
            _index.Deactivate();
            _context.CreateEntity().AddUser("Test", 1);
            _index.GetEntities("Test").Should().BeEmpty();
        }

        [Fact]
        public void HasExistingEntitiesWhenActivating()
        {
            var entity1 = _context.CreateEntity().AddUser("Test", 1);
            var entity2 = _context.CreateEntity().AddUser("Test", 2);
            _index.Deactivate();
            _index.Activate();
            var entities = _index.GetEntities("Test");
            entities.Count.Should().Be(2);
            entities.Should().Contain(entity1);
            entities.Should().Contain(entity2);
        }

        [Fact]
        public void MultiKeyHasExistingEntitiesWhenActivating()
        {
            var entity1 = _multiKeyContext.CreateEntity().AddUser("Test", 1);
            var entity2 = _multiKeyContext.CreateEntity().AddUser("Test", 2);
            _multiKeyIndex.Deactivate();
            _multiKeyIndex.Activate();
            _multiKeyIndex.GetEntities("1").First().Should().BeSameAs(entity1);
            _multiKeyIndex.GetEntities("2").Should().Contain(entity1);
            _multiKeyIndex.GetEntities("2").Should().Contain(entity2);
            _multiKeyIndex.GetEntities("3").First().Should().BeSameAs(entity2);
        }

        [Fact]
        public void AddsNewEntitiesWhenActivated()
        {
            var entity1 = _context.CreateEntity().AddUser("Test", 1);
            var entity2 = _context.CreateEntity().AddUser("Test", 2);
            _index.Deactivate();
            _index.Activate();
            var entity3 = _context.CreateEntity().AddUser("Test", 3);

            var entities = _index.GetEntities("Test");
            entities.Count.Should().Be(3);
            entities.Should().Contain(entity1);
            entities.Should().Contain(entity2);
            entities.Should().Contain(entity3);
        }

        [Fact]
        public void GetsLastComponentThatTriggeredAddingEntityToGroup()
        {
            IComponent lastComponent = null;

            var group = _context.GetGroup(Matcher<TestEntity>.AllOf(1, 2));
            new EntityIndex<TestEntity, string>(
                "TestIndex",
                group, (_, c) =>
                {
                    lastComponent = c;
                    return ((UserComponent)c).Name;
                });

            var user1 = new UserComponent { Name = "Test1", Age = 42 };
            var user2 = new UserComponent { Name = "Test2", Age = 24 };

            var entity = _context.CreateEntity();
            entity.AddComponent(CID.ComponentA, user1);
            entity.AddComponent(CID.ComponentB, user2);

            lastComponent.Should().BeSameAs(user2);
        }

        [Fact]
        public void WorksWithNoneOf()
        {
            var lastComponents = new List<IComponent>();

            var user1 = new UserComponent { Name = "Test1", Age = 42 };
            var user2 = new UserComponent { Name = "Test2", Age = 24 };

            var index = new EntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA).NoneOf(CID.ComponentB)),
                (e, c) =>
                {
                    lastComponents.Add(c);
                    return c == user1
                        ? ((UserComponent)c).Name
                        : ((UserComponent)e.GetComponent(CID.ComponentA)).Name;
                }
            );

            var entity = _context.CreateEntity();
            entity.AddComponent(CID.ComponentA, user1);
            entity.AddComponent(CID.ComponentB, user2);

            lastComponents.Count.Should().Be(2);
            lastComponents[0].Should().Be(user1);
            lastComponents[1].Should().Be(user2);

            index.GetEntities("Max").Should().BeEmpty();
            index.GetEntities("Jack").Should().BeEmpty();
        }

        EntityIndex<TestEntity, string> CreateEntityIndex() =>
            new EntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup(Matcher<TestEntity>.AllOf(0)),
                (entity, component) => (component as UserComponent)?.Name ?? entity.GetUser().Name);

        EntityIndex<TestEntity, string> CreateMultiKeyEntityIndex() => new EntityIndex<TestEntity, string>(
            "TestIndex",
            _multiKeyContext.GetGroup(Matcher<TestEntity>.AllOf(0)),
            (entity, c) => (c as UserComponent ?? entity.GetUser()).Age == 1
                ? new[] { "1", "2" }
                : new[] { "2", "3" });
    }
}
