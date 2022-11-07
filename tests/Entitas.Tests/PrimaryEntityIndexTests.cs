using FluentAssertions;
using Xunit;

namespace Entitas.Tests
{
    public class PrimaryEntityIndexTests
    {
        const string Name = "Max";

        readonly IContext<TestEntity> _context;
        readonly PrimaryEntityIndex<TestEntity, string> _index;
        readonly IContext<TestEntity> _multiKeyContext;
        readonly PrimaryEntityIndex<TestEntity, string> _multiKeyIndex;

        public PrimaryEntityIndexTests()
        {
            _context = new MyTestContext();
            _index = CreatePrimaryEntityIndex();
            _multiKeyContext = new MyTestContext();
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
            var entity = CreateNameAgeEntity(_context);
            _index.GetEntity(Name).Should().BeSameAs(entity);
        }

        [Fact]
        public void MultiKeyGetsEntityForKey()
        {
            var entity = CreateNameAgeEntity(_multiKeyContext);
            _multiKeyIndex.GetEntity($"{Name}1").Should().BeSameAs(entity);
            _multiKeyIndex.GetEntity($"{Name}2").Should().BeSameAs(entity);
        }

        [Fact]
        public void RetainsEntity()
        {
            var entity = CreateNameAgeEntity(_context);
            entity.retainCount.Should().Be(3); // Context, Group, EntityIndex
            (entity.aerc as SafeAERC)?.owners.Should().Contain(_index);
        }

        [Fact]
        public void MultiKeyRetainsEntity()
        {
            var entity = CreateNameAgeEntity(_multiKeyContext);
            entity.retainCount.Should().Be(3); // Context, Group, EntityIndex
            (entity.aerc as SafeAERC)?.owners.Should().Contain(_multiKeyIndex);
        }

        [Fact]
        public void HasExistingEntity()
        {
            var entity = CreateNameAgeEntity(_context);
            var newIndex = CreatePrimaryEntityIndex();
            newIndex.GetEntity(Name).Should().BeSameAs(entity);
        }

        [Fact]
        public void ReleasesAndRemovesEntityFromIndexWhenComponentGetsRemoved()
        {
            var entity = CreateNameAgeEntity(_context);
            entity.RemoveComponentA();
            _index.GetEntity(Name).Should().BeNull();
            entity.retainCount.Should().Be(1); // Context
            (entity.aerc as SafeAERC)?.owners.Should().NotContain(_index);
        }

        [Fact]
        public void MultiKeyReleasesAndRemovesEntityFromIndexWhenComponentGetsRemoved()
        {
            var entity = CreateNameAgeEntity(_multiKeyContext);
            entity.RemoveComponentA();
            _multiKeyIndex.GetEntity($"{Name}1").Should().BeNull();
            _multiKeyIndex.GetEntity($"{Name}2").Should().BeNull();
            entity.retainCount.Should().Be(1);
            (entity.aerc as SafeAERC)?.owners.Should().NotContain(_multiKeyIndex);
        }

        [Fact]
        public void ThrowsWhenAddingEntityForSameKey()
        {
            CreateNameAgeEntity(_context);
            FluentActions.Invoking(() => CreateNameAgeEntity(_context))
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
            var entity = CreateNameAgeEntity(_context);
            _index.Deactivate();
            _index.GetEntity(Name).Should().BeNull();
            entity.retainCount.Should().Be(2); // Context, Group
        }

        [Fact]
        public void DoesNotAddEntitiesAnymore()
        {
            CreateNameAgeEntity(_context);
            _index.Deactivate();
            CreateNameAgeEntity(_context);
            _index.GetEntity(Name).Should().BeNull();
        }

        [Fact]
        public void HasExistingEntityWhenActivating()
        {
            var entity = CreateNameAgeEntity(_context);
            _index.Deactivate();
            _index.Activate();
            _index.GetEntity(Name).Should().BeSameAs(entity);
        }

        [Fact]
        public void MultiKeyHasExistingEntityWhenActivating()
        {
            var entity = CreateNameAgeEntity(_multiKeyContext);
            _multiKeyIndex.Deactivate();
            _multiKeyIndex.Activate();
            _multiKeyIndex.GetEntity($"{Name}1").Should().BeSameAs(entity);
            _multiKeyIndex.GetEntity($"{Name}2").Should().BeSameAs(entity);
        }

        [Fact]
        public void AddsNewEntitiesWhenActivated()
        {
            CreateNameAgeEntity(_context);
            _index.Deactivate();
            _index.Activate();
            var entity = CreateNameAgeEntity(_context, "Jack");
            _index.GetEntity("Jack").Should().BeSameAs(entity);
        }

        PrimaryEntityIndex<TestEntity, string> CreatePrimaryEntityIndex()
        {
            return new PrimaryEntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA)),
                (e, c) => (c as NameAgeComponent ?? (NameAgeComponent)e.GetComponent(CID.ComponentA)).name);
        }

        PrimaryEntityIndex<TestEntity, string> CreateMultiKeyPrimaryEntityIndex()
        {
            return new PrimaryEntityIndex<TestEntity, string>(
                "TestIndex",
                _multiKeyContext.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA)),
                (e, c) =>
                {
                    var name = (c as NameAgeComponent ?? (NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
                    return new[] {$"{name}1", $"{name}2"};
                });
        }

        static TestEntity CreateNameAgeEntity(IContext<TestEntity> context, string name = Name)
        {
            var nameAgeComponent = new NameAgeComponent();
            nameAgeComponent.name = name;
            var entity = context.CreateEntity();
            entity.AddComponent(CID.ComponentA, nameAgeComponent);
            return entity;
        }
    }
}
