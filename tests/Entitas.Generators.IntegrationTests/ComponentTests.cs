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
        public void SetsComponent()
        {
            var entity = _context.CreateEntity();
            entity.SetPosition(1, 2);
            entity.HasComponent(MyAppMainPositionComponentIndex.Index.Value)
                .Should().BeTrue();
        }

        [Fact]
        public void GetsComponent()
        {
            var entity = _context.CreateEntity();
            entity.SetPosition(1, 2);

            var position = entity.GetPosition()!;
            position.X.Should().Be(1);
            position.Y.Should().Be(2);
        }

        [Fact]
        public void GettingNonExistingComponentReturnNull()
        {
            var entity = _context.CreateEntity();
            entity.GetPosition().Should().BeNull();
        }

        [Fact]
        public void ReplacesComponent()
        {
            var entity = _context.CreateEntity();
            entity.SetPosition(1, 2);
            entity.SetPosition(3, 4);

            var position = entity.GetPosition()!;
            position.X.Should().Be(3);
            position.Y.Should().Be(4);
        }

        [Fact]
        public void SetComponentUsesComponentPool()
        {
            var component = new PositionComponent { X = 1, Y = 2 };
            var entity = _context.CreateEntity();
            entity
                .GetComponentPool(MyAppMainPositionComponentIndex.Index.Value)
                .Push(component);

            entity.SetPosition(3, 4);
            entity.GetPosition().Should().BeSameAs(component);
        }

        [Fact]
        public void UnsetsComponent()
        {
            var entity = _context.CreateEntity();
            entity.SetPosition(1, 2);
            entity.UnsetPosition();
            entity.HasComponent(MyAppMainPositionComponentIndex.Index.Value)
                .Should().BeFalse();
        }

        [Fact]
        public void UnsettingNonExistingComponentDoesNothing()
        {
            _context.CreateEntity().UnsetPosition();
        }

        [Fact]
        public void FlagComponentsUseSingleComponent()
        {
            var entity1 = _context
                .CreateEntity()
                .SetMovable();

            var entity2 = _context
                .CreateEntity()
                .SetMovable();

            entity1.GetMovable().Should().BeSameAs(entity2.GetMovable());
        }

        /*
         * Unique
         */

        [Fact]
        public void SetsUniqueEntity()
        {
            _context.GetLoadingEntity().Should().BeNull();
            _context.GetLoading().Should().BeNull();

            _context.SetLoading();

            _context.GetLoadingEntity().Should().NotBeNull();
            _context.GetLoading().Should().NotBeNull();
        }

        [Fact]
        public void SettingUniqueEntityTwiceReturnsEntity()
        {
            _context.SetLoading().Should().BeSameAs(_context.SetLoading());
        }

        [Fact]
        public void UnsettingDestroysUniqueEntity()
        {
            var entity = _context.SetLoading();
            _context.UnsetLoading();
            _context.GetLoadingEntity().Should().BeNull();
            entity.isEnabled.Should().BeFalse();
        }

        [Fact]
        public void UnsettingNonExistingEntityDoesNothing()
        {
            _context.UnsetLoading();
        }

        [Fact]
        public void GetsUniqueEntity()
        {
            _context.SetLoading().Should().BeSameAs(_context.GetLoadingEntity());
        }

        [Fact]
        public void NotSetUniqueEntityReturnsNull()
        {
            _context.GetLoadingEntity().Should().BeNull();
        }

        [Fact]
        public void SetsValuesOfUniqueEntity()
        {
            _context.SetUser("Test", 42);
            var user = _context.GetUserEntity()!.GetUser()!;
            user.Name.Should().Be("Test");
            user.Age.Should().Be(42);
        }

        [Fact]
        public void SetsNewValuesOfUniqueEntity()
        {
            _context.SetUser("Test", 42);
            _context.SetUser("Replaced", 24);
            var user = _context.GetUserEntity()!.GetUser()!;
            user.Name.Should().Be("Replaced");
            user.Age.Should().Be(24);
        }

        [Fact]
        public void GetsComponentOfUniqueEntity()
        {
            _context.SetUser("Test", 42);
            var user = _context.GetUser()!;
            user.Name.Should().Be("Test");
            user.Age.Should().Be(42);
        }

        [Fact]
        public void NotSetComponentUniqueEntityReturnsNull()
        {
            _context.GetUser().Should().BeNull();
        }
    }
}
