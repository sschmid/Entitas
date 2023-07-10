using System;
using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ComponentTests
    {
        readonly MainContext _context;

        public ComponentTests()
        {
            ContextInitialization.InitializeMain();
            _context = new MainContext();
        }

        [Fact]
        public void DoesNotHaveComponent()
        {
            var entity = _context.CreateEntity();
            entity.HasPosition().Should().BeFalse();
        }

        [Fact]
        public void AddsComponent()
        {
            var entity = _context.CreateEntity();
            entity.AddPosition(1, 2);
            entity.HasComponent(MyAppMainPositionComponentIndex.Index.Value);
        }

        [Fact]
        public void HasComponent()
        {
            var entity = _context.CreateEntity();
            entity.AddPosition(1, 2);
            entity.HasPosition().Should().BeTrue();
        }

        [Fact]
        public void GetsComponent()
        {
            var entity = _context.CreateEntity();
            entity.AddPosition(1, 2);

            var position = entity.GetPosition();
            position.X.Should().Be(1);
            position.Y.Should().Be(2);
        }

        [Fact]
        public void ReplacesComponent()
        {
            var entity = _context.CreateEntity();
            entity.AddPosition(1, 2);
            entity.ReplacePosition(3, 4);

            var position = entity.GetPosition();
            position.X.Should().Be(3);
            position.Y.Should().Be(4);
        }

        [Fact]
        public void AddComponentUsesComponentPool()
        {
            var component = new PositionComponent { X = 1, Y = 2 };
            var entity = _context.CreateEntity();
            entity
                .GetComponentPool(MyAppMainPositionComponentIndex.Index.Value)
                .Push(component);

            entity.AddPosition(3, 4);
            entity.GetPosition().Should().BeSameAs(component);
        }

        [Fact]
        public void ReplaceComponentUsesComponentPool()
        {
            var component = new PositionComponent { X = 1, Y = 2 };
            var entity = _context.CreateEntity();
            entity.AddPosition(3, 4);
            entity
                .GetComponentPool(MyAppMainPositionComponentIndex.Index.Value)
                .Push(component);

            entity.ReplacePosition(5, 6);
            entity.GetPosition().Should().BeSameAs(component);
        }

        [Fact]
        public void UsesSingleComponent()
        {
            var entity1 = _context
                .CreateEntity()
                .AddMovable();

            var entity2 = _context
                .CreateEntity()
                .AddMovable();

            entity1.GetMovable().Should().BeSameAs(entity2.GetMovable());
        }

        /*
         * Unique
         */

        [Fact]
        public void HasNoUniqueEntity()
        {
            // Flag
            _context.HasLoading().Should().BeFalse();

            // Normal
            _context.HasUser().Should().BeFalse();
        }

        [Fact]
        public void SetsUniqueEntity()
        {
            // Flag
            var flagEntity = _context.SetLoading();
            _context.HasLoading().Should().BeTrue();
            flagEntity.HasLoading().Should().BeTrue();

            // Normal
            var entity = _context.SetUser("Test", 42);
            _context.HasUser().Should().BeTrue();
            entity.HasUser().Should().BeTrue();
        }

        [Fact]
        public void SetsValuesOfUniqueEntity()
        {
            _context.SetUser("Test", 42);
            var user = _context.GetUser();
            user.Name.Should().Be("Test");
            user.Age.Should().Be(42);
        }

        [Fact]
        public void ThrowsWhenGettingUniqueComponentWhenNotSet()
        {
            FluentActions.Invoking(() => _context.GetUser())
                .Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void SettingUniqueFlagEntityTwiceReturnsEntity()
        {
            _context.SetLoading().Should().BeSameAs(_context.SetLoading());
        }

        [Fact]
        public void ThrowsWhenSettingUniqueEntityTwice()
        {
            _context.SetUser("Test", 42);
            FluentActions.Invoking(() => _context.SetUser("Test", 42))
                .Should().Throw<EntitasException>();
        }

        [Fact]
        public void ReplacesUniqueEntity()
        {
            _context.SetUser("Test", 42);
            _context.ReplaceUser("Replaced", 24);
            _context.HasUser().Should().BeTrue();
            var user = _context.GetUser();
            user.Name.Should().Be("Replaced");
            user.Age.Should().Be(24);
        }

        [Fact]
        public void ReplaceAddsUniqueEntity()
        {
            _context.ReplaceUser("Replaced", 24);
            _context.HasUser().Should().BeTrue();
            var user = _context.GetUser();
            user.Name.Should().Be("Replaced");
            user.Age.Should().Be(24);
        }

        [Fact]
        public void UnsetsAndDestroysUniqueFlagEntity()
        {
            var entity = _context.SetLoading();
            _context.UnsetLoading();
            _context.HasLoading().Should().BeFalse();
            entity.isEnabled.Should().BeFalse();
        }

        [Fact]
        public void RemovesAndDestroysUniqueEntity()
        {
            var entity = _context.SetUser("Test", 42);
            _context.RemoveUser();
            _context.HasLoading().Should().BeFalse();
            entity.isEnabled.Should().BeFalse();
        }

        [Fact]
        public void CanUnsetUniqueFlagEntityWhenNotSet()
        {
            _context.UnsetLoading();
        }

        [Fact]
        public void ThrowsWhenRemovingUniqueEntityWhenNotSet()
        {
            FluentActions.Invoking(() => _context.RemoveUser())
                .Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void GetsUniqueEntity()
        {
            // Flag
            _context.SetLoading().Should().BeSameAs(_context.GetLoadingEntity());

            // Normal
            _context.SetUser("Test", 42).Should().BeSameAs(_context.GetUserEntity());
        }

        [Fact]
        public void NotSetUniqueEntityReturnsNull()
        {
            // Flag
            _context.GetLoadingEntity().Should().BeNull();

            // Normal
            _context.GetUserEntity().Should().BeNull();
        }
    }
}
