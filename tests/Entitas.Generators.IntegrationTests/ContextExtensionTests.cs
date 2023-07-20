using System;
using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ContextExtensionTests
    {
        readonly MainContext _context;

        public ContextExtensionTests()
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
            _context.HasLoading().Should().BeFalse();
        }

        [Fact]
        public void SetsFlagComponent()
        {
            _context
                .SetLoading()
                .HasLoading()
                .Should().BeTrue();
        }

        [Fact]
        public void HasFlagComponent()
        {
            _context.SetLoading();
            _context
                .HasLoading()
                .Should().BeTrue();
        }

        [Fact]
        public void CanSetFlagComponentTwice()
        {
            _context.SetLoading();
            _context.SetLoading();
            _context.HasLoading().Should().BeTrue();
        }

        [Fact]
        public void SetFlagComponentReturnsSameEntity()
        {
            _context.SetLoading().Should().BeSameAs(_context.SetLoading());
        }

        [Fact]
        public void UnsetsAndDestroysFlagComponent()
        {
            var entity = _context.SetLoading();
            _context.UnsetLoading();
            _context.HasLoading().Should().BeFalse();
            entity.IsEnabled.Should().BeFalse();
        }

        [Fact]
        public void CanUnsetFlagComponentTwice()
        {
            _context.SetLoading();
            _context.UnsetLoading();
            _context.UnsetLoading();
            _context.HasLoading().Should().BeFalse();
        }

        [Fact]
        public void GetsFlagComponent()
        {
            _context.SetLoading();
            var entity = _context.GetLoadingEntity();
            entity.Should().NotBeNull();
            entity.HasLoading().Should().BeTrue();
        }

        [Fact]
        public void GettingUnsetFlagEntityReturnsNull()
        {
            _context.GetLoadingEntity().Should().BeNull();
        }

        /*
         * Normal Component
         */

        [Fact]
        public void DoesNotHaveComponent()
        {
            _context.HasUser().Should().BeFalse();
        }

        [Fact]
        public void SetsComponent()
        {
            var component = _context
                .SetUser("Test", 42)
                .GetUser();

            component.Name.Should().Be("Test");
            component.Age.Should().Be(42);
        }

        [Fact]
        public void ThrowsWhenSettingComponentTwice()
        {
            _context.SetUser("Test", 42);
            FluentActions.Invoking(() => _context.SetUser("Test", 42))
                .Should().Throw<EntitasException>();
        }

        [Fact]
        public void HasComponent()
        {
            _context.SetUser("Test", 42);
            _context
                .HasUser()
                .Should().BeTrue();
        }

        [Fact]
        public void GetsComponent()
        {
            _context.SetUser("Test", 42);
            var component = _context.GetUser();
            component.Name.Should().Be("Test");
            component.Age.Should().Be(42);
        }

        [Fact]
        public void ThrowsWhenGettingComponentWhenNotSet()
        {
            FluentActions.Invoking(() => _context.GetUser())
                .Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void ReplacesComponent()
        {
            _context.SetUser("Test", 42);
            var component = _context
                .ReplaceUser("Replaced", 24)
                .GetUser();

            component.Name.Should().Be("Replaced");
            component.Age.Should().Be(24);
        }

        [Fact]
        public void ReplaceComponentAddsComponent()
        {
            _context.ReplaceUser("Test", 42);
            var component = _context.GetUser();
            component.Name.Should().Be("Test");
            component.Age.Should().Be(42);
        }

        [Fact]
        public void RemovesAndDestroysComponent()
        {
            var entity = _context.SetUser("Test", 42);
            _context.RemoveUser();
            _context.HasUser().Should().BeFalse();
            entity.IsEnabled.Should().BeFalse();
        }

        [Fact]
        public void ThrowsWhenRemovingComponentTwice()
        {
            _context.SetUser("Test", 42);
            _context.RemoveUser();
            FluentActions.Invoking(() => _context.RemoveUser())
                .Should().Throw<NullReferenceException>();
        }

        [Fact]
        public void GettingUnsetEntityReturnsNull()
        {
            _context.GetUserEntity().Should().BeNull();
        }
    }
}
