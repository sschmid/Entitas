using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class PrimaryEntityIndexTests
    {
        readonly IContext<TestEntity> _context;
        readonly PrimaryEntityIndex<TestEntity, string> _index;
        readonly IContext<TestEntity> _multiKeyContext;
        readonly PrimaryEntityIndex<TestEntity, string> _multiKeyIndex;

        public PrimaryEntityIndexTests()
        {
            _context = new TestContext(1);
            _index = CreatePrimaryEntityIndex();
            _multiKeyContext = new TestContext(1);
            _multiKeyIndex = CreateMultiKeyPrimaryEntityIndex();
        }

        [Fact]
        public void ReturnsNullWhenGettingEntityForUnknownKey()
        {
            _index.GetEntity("unknownKey").Should().BeNull();
        }

        [Fact]
        public void GetsEntityForKey()
        {
            var entity = _context.CreateEntity().AddUser("Test", 42);
            _index.GetEntity("Test").Should().BeSameAs(entity);
        }

        [Fact]
        public void MultiKeyGetsEntityForKey()
        {
            var entity = _multiKeyContext.CreateEntity().AddUser("Test", 42);
            _multiKeyIndex.GetEntity("Test1").Should().BeSameAs(entity);
            _multiKeyIndex.GetEntity("Test2").Should().BeSameAs(entity);
        }

        [Fact]
        public void RetainsEntity()
        {
            var entity = _context.CreateEntity().AddUser("Test", 42);
            entity.RetainCount.Should().Be(3); // Context, Group, EntityIndex
            (entity.Aerc as SafeAERC)?.Owners.Should().Contain(_index);
        }

        [Fact]
        public void MultiKeyRetainsEntity()
        {
            var entity = _multiKeyContext.CreateEntity().AddUser("Test", 42);
            entity.RetainCount.Should().Be(3); // Context, Group, EntityIndex
            (entity.Aerc as SafeAERC)?.Owners.Should().Contain(_multiKeyIndex);
        }

        [Fact]
        public void HasExistingEntity()
        {
            var entity = _context.CreateEntity().AddUser("Test", 42);
            var newIndex = CreatePrimaryEntityIndex();
            newIndex.GetEntity("Test").Should().BeSameAs(entity);
        }

        [Fact]
        public void ReleasesAndRemovesEntityFromIndexWhenComponentGetsRemoved()
        {
            var entity = _context.CreateEntity().AddUser("Test", 42);
            entity.RemoveUser();
            _index.GetEntity("Test").Should().BeNull();
            entity.RetainCount.Should().Be(1); // Context
            (entity.Aerc as SafeAERC)?.Owners.Should().NotContain(_index);
        }

        [Fact]
        public void MultiKeyReleasesAndRemovesEntityFromIndexWhenComponentGetsRemoved()
        {
            var entity = _multiKeyContext.CreateEntity().AddUser("Test", 42);
            entity.RemoveUser();
            _multiKeyIndex.GetEntity("Test1").Should().BeNull();
            _multiKeyIndex.GetEntity("Test2").Should().BeNull();
            entity.RetainCount.Should().Be(1);
            (entity.Aerc as SafeAERC)?.Owners.Should().NotContain(_multiKeyIndex);
        }

        [Fact]
        public void ThrowsWhenAddingEntityForSameKey()
        {
            _context.CreateEntity().AddUser("Test", 42);
            FluentActions.Invoking(() => _context.CreateEntity().AddUser("Test", 42))
                .Should().Throw<EntityIndexException>();
        }

        [Fact]
        public void CanToString()
        {
            _index.ToString().Should().Be("PrimaryEntityIndex(TestIndex)");
        }

        [Fact]
        public void ClearsIndexAndReleasesEntity()
        {
            var entity = _context.CreateEntity().AddUser("Test", 42);
            _index.Deactivate();
            _index.GetEntity("Test").Should().BeNull();
            entity.RetainCount.Should().Be(2); // Context, Group
        }

        [Fact]
        public void DoesNotAddEntitiesAnymore()
        {
            _context.CreateEntity().AddUser("Test", 42);
            _index.Deactivate();
            _context.CreateEntity().AddUser("Test", 42);
            _index.GetEntity("Test").Should().BeNull();
        }

        [Fact]
        public void HasExistingEntityWhenActivating()
        {
            var entity = _context.CreateEntity().AddUser("Test", 42);
            ;
            _index.Deactivate();
            _index.Activate();
            _index.GetEntity("Test").Should().BeSameAs(entity);
        }

        [Fact]
        public void MultiKeyHasExistingEntityWhenActivating()
        {
            var entity = _multiKeyContext.CreateEntity().AddUser("Test", 42);
            _multiKeyIndex.Deactivate();
            _multiKeyIndex.Activate();
            _multiKeyIndex.GetEntity("Test1").Should().BeSameAs(entity);
            _multiKeyIndex.GetEntity("Test2").Should().BeSameAs(entity);
        }

        [Fact]
        public void AddsNewEntitiesWhenActivated()
        {
            _context.CreateEntity().AddUser("Test1", 42);
            _index.Deactivate();
            _index.Activate();
            var entity = _context.CreateEntity().AddUser("Test2", 24);
            _index.GetEntity("Test2").Should().BeSameAs(entity);
        }

        PrimaryEntityIndex<TestEntity, string> CreatePrimaryEntityIndex()
        {
            return new PrimaryEntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup(TestUserMatcher.User),
                (entity, c) => (c as UserComponent ?? entity.GetUser()).Name);
        }

        PrimaryEntityIndex<TestEntity, string> CreateMultiKeyPrimaryEntityIndex()
        {
            return new PrimaryEntityIndex<TestEntity, string>(
                "TestIndex",
                _multiKeyContext.GetGroup(TestUserMatcher.User),
                (entity, c) =>
                {
                    var name = (c as UserComponent ?? entity.GetUser()).Name;
                    return new[] { $"{name}1", $"{name}2" };
                });
        }
    }
}
