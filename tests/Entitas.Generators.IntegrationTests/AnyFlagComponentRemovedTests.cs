using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class AnyFlagComponentRemovedTests
    {
        readonly MainContext _context;
        readonly AnyLoadingRemovedListener _listener;
        readonly MyAppMainAnyLoadingRemovedEventSystem _system;

        public AnyFlagComponentRemovedTests()
        {
            ContextInitialization.InitializeMain();
            _context = new MainContext();
            _listener = new AnyLoadingRemovedListener(_context);
            _system = new MyAppMainAnyLoadingRemovedEventSystem(_context);
        }

        [Fact]
        public void IsNullWhenNothingChanged()
        {
            _context.CreateEntity().AddLoading();
            _system.Execute();
            _listener.Entity.Should().BeNull();
        }

        [Fact]
        public void PassesEntityWhenRemoved()
        {
            var entity = _context
                .CreateEntity()
                .AddLoading()
                .RemoveLoading();

            _system.Execute();
            _listener.Entity.Should().BeSameAs(entity);
        }

        [Fact]
        public void SkipsWhenTriggeringComponentHasBeenAdded()
        {
            _context
                .CreateEntity()
                .AddLoading()
                .RemoveLoading()
                .AddLoading();

            _system.Execute();
            _listener.Entity.Should().BeNull();
        }

        [Fact]
        public void CanUnsubscribeInCallback()
        {
            _listener.Unsubscribe = true;
            var entity = _context
                .CreateEntity()
                .AddLoading()
                .RemoveLoading();

            _system.Execute();
            _listener.Entity.Should().BeSameAs(entity);
        }

        [Fact]
        public void CanDestroyListenerEntityInCallback()
        {
            _listener.DestroyListener = true;
            var entity = _context
                .CreateEntity()
                .AddLoading()
                .RemoveLoading();

            var dummy = new AnyLoadingRemovedListener(_context)
            {
                DestroyListener = true
            };

            _system.Execute();
            _listener.Entity.Should().BeSameAs(entity);
        }
    }

#nullable disable

    public class AnyLoadingRemovedListener : IMyAppMainAnyLoadingRemovedListener
    {
        readonly MyApp.Main.Entity _listener;

        public AnyLoadingRemovedListener(MainContext context)
        {
            _listener = context.CreateEntity().AddAnyLoadingRemovedListener(this);
        }

        public Entity Entity { get; private set; }
        public bool Unsubscribe { get; set; }
        public bool DestroyListener { get; set; }

        public void OnAnyLoadingRemoved(MyApp.Main.Entity entity)
        {
            Entity = entity;
            if (Unsubscribe)
            {
                _listener.RemoveAnyLoadingRemovedListener(this);
            }

            if (DestroyListener)
            {
                _listener.Destroy();
            }
        }
    }
}
