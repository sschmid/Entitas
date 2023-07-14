using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class EntityExtensionTests
    {
        readonly MainContext _context;

        public EntityExtensionTests()
        {
            ContextInitialization.InitializeMain();
            _context = new MainContext();
        }

        /*
         * Flag Component
         */

        [Fact]
        public void DoesNotHaveFlagComponent()
        {
            _context.CreateEntity().HasLoading().Should().BeFalse();
        }

        [Fact]
        public void AddsFlagComponent()
        {
            _context
                .CreateEntity()
                .AddLoading()
                .HasComponent(MyAppMainLoadingComponentIndex.Index.Value)
                .Should().BeTrue();
        }

        [Fact]
        public void HasFlagComponent()
        {
            _context.CreateEntity()
                .AddLoading()
                .HasLoading()
                .Should().BeTrue();
        }

        [Fact]
        public void GetsFlagComponent()
        {
            _context
                .CreateEntity()
                .AddLoading()
                .GetLoading()
                .Should().BeAssignableTo<LoadingComponent>();
        }

        [Fact]
        public void UsesSingleComponent()
        {
            var component1 = _context
                .CreateEntity()
                .AddLoading()
                .GetLoading();

            var component2 = _context
                .CreateEntity()
                .AddLoading()
                .GetLoading();

            component1.Should().BeSameAs(component2);
        }

        [Fact]
        public void ReplacesFlagComponent()
        {
            var entity = _context.CreateEntity();
            var didReplace = 0;
            entity.OnComponentReplaced += (e, index, component, newComponent) =>
            {
                didReplace += 1;
                e.Should().BeSameAs(entity);
                index.Should().Be(MyAppMainLoadingComponentIndex.Index.Value);
                component.Should().BeSameAs(newComponent);
            };

            entity
                .AddLoading()
                .ReplaceLoading()
                .HasLoading()
                .Should().BeTrue();

            didReplace.Should().Be(1);
        }

        [Fact]
        public void RemovesFlagComponent()
        {
            _context
                .CreateEntity()
                .AddLoading()
                .RemoveLoading()
                .HasLoading()
                .Should().BeFalse();
        }

        /*
         * Normal Component
         */

        [Fact]
        public void DoesNotHaveComponent()
        {
            _context.CreateEntity().HasUser().Should().BeFalse();
        }

        [Fact]
        public void AddsComponent()
        {
            _context
                .CreateEntity()
                .AddUser("Test", 42)
                .HasComponent(MyAppMainUserComponentIndex.Index.Value)
                .Should().BeTrue();
        }

        [Fact]
        public void HasComponent()
        {
            _context.CreateEntity()
                .AddUser("Test", 42)
                .HasUser()
                .Should().BeTrue();
        }

        [Fact]
        public void GetsComponent()
        {
            var component = _context
                .CreateEntity()
                .AddUser("Test", 42)
                .GetUser();

            component.Name.Should().Be("Test");
            component.Age.Should().Be(42);
        }

        [Fact]
        public void ReplacesComponent()
        {
            var component = _context
                .CreateEntity()
                .AddUser("Test", 42)
                .ReplaceUser("Replaced", 24)
                .GetUser();

            component.Name.Should().Be("Replaced");
            component.Age.Should().Be(24);
        }

        [Fact]
        public void AddComponentUsesComponentPool()
        {
            var component = new UserComponent { Name = "Pooled", Age = 24 };
            var entity = _context.CreateEntity();

            entity
                .GetComponentPool(MyAppMainUserComponentIndex.Index.Value)
                .Push(component);

            entity
                .AddUser("Test", 42)
                .GetUser()
                .Should().BeSameAs(component);
        }

        [Fact]
        public void ReplaceComponentUsesComponentPool()
        {
            var component = new UserComponent { Name = "Pooled", Age = 24 };
            var entity = _context.CreateEntity();

            entity
                .GetComponentPool(MyAppMainUserComponentIndex.Index.Value)
                .Push(component);

            entity
                .ReplaceUser("Test", 42)
                .GetUser()
                .Should().BeSameAs(component);
        }

        [Fact]
        public void RemovesComponent()
        {
            _context
                .CreateEntity()
                .AddUser("Test", 42)
                .RemoveUser()
                .HasUser()
                .Should().BeFalse();
        }
    }
}
