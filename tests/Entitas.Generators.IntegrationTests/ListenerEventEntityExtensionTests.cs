using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class ListenerEventEntityExtensionTests
    {
        readonly MainContext _context;
        readonly ListenerEventEntityExtensionListener _listener;

        public ListenerEventEntityExtensionTests()
        {
            ContextInitialization.InitializeMain();
            _context = new MainContext();
            _listener = new ListenerEventEntityExtensionListener(_context);
        }

        [Fact]
        public void AddsListener()
        {
            var listener = _context.CreateEntity().AddAnyLoadingAddedListener(_listener);
            listener.HasAnyLoadingAddedListener().Should().BeTrue();
        }

        [Fact]
        public void RemovesListener()
        {
            var listener = _context.CreateEntity().AddAnyLoadingAddedListener(_listener);
            listener.RemoveAnyLoadingAddedListener(_listener, false);
            listener.GetAnyLoadingAddedListener().Value.Should().HaveCount(0);
        }

        [Fact]
        public void RemovesListenerWhenEntityIsNotEmpty()
        {
            var listener = _context
                .CreateEntity()
                .AddLoading()
                .AddAnyLoadingAddedListener(_listener);

            listener.RemoveAnyLoadingAddedListener(_listener, true);
            listener.HasAnyLoadingAddedListener().Should().BeFalse();
            listener.HasLoading().Should().BeTrue();
        }

        [Fact]
        public void DestroysListenerWhenEntityIsEmpty()
        {
            var listener = _context
                .CreateEntity()
                .AddAnyLoadingAddedListener(_listener);

            listener.RemoveAnyLoadingAddedListener(_listener, true);
            listener.HasAnyLoadingAddedListener().Should().BeFalse();
            listener.IsEnabled.Should().BeFalse();
        }
    }

#nullable disable

    public class ListenerEventEntityExtensionListener : IMyAppMainAnyLoadingAddedListener
    {
        public ListenerEventEntityExtensionListener(MainContext context)
        {
            context.CreateEntity().AddAnyLoadingAddedListener(this);
        }

        public void OnAnyLoadingAdded(MyApp.Main.Entity entity) { }
    }
}
