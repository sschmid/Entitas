using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class AnyFlagComponentAddedTests
    {
        readonly MainContext _context;
        readonly AnyLoadingAddedListener _listener;
        readonly MyAppMainAnyLoadingAddedEventSystem _system;

        public AnyFlagComponentAddedTests()
        {
            ContextInitialization.InitializeMain();
            _context = new MainContext();
            _listener = new AnyLoadingAddedListener(_context);
            _system = new MyAppMainAnyLoadingAddedEventSystem(_context);
        }

        [Fact]
        public void IsNullWhenNothingChanged()
        {
            _system.Execute();
            _listener.Entity.Should().BeNull();
        }

        [Fact]
        public void PassesEntityWhenAdded()
        {
            var entity = _context.CreateEntity().AddLoading();
            _system.Execute();
            _listener.Entity.Should().BeSameAs(entity);
        }

        [Fact]
        public void SkipsWhenTriggeringComponentHasBeenRemoved()
        {
            _context
                .CreateEntity()
                .AddLoading()
                .RemoveLoading();

            _system.Execute();
            _listener.Entity.Should().BeNull();
        }

        [Fact]
        public void CanUnsubscribeInCallback()
        {
            _listener.Unsubscribe = true;
            var entity = _context.CreateEntity().AddLoading();
            _system.Execute();
            _listener.Entity.Should().BeSameAs(entity);
        }

        [Fact]
        public void CanDestroyListenerEntityInCallback()
        {
            _listener.DestroyListener = true;
            var entity = _context.CreateEntity().AddLoading();
            var dummy = new AnyLoadingAddedListener(_context)
            {
                DestroyListener = true
            };
            _system.Execute();
            _listener.Entity.Should().BeSameAs(entity);
        }
    }

#nullable disable

    public class AnyLoadingAddedListener : IMyAppMainAnyLoadingAddedListener
    {
        readonly MyApp.Main.Entity _listener;

        public AnyLoadingAddedListener(MainContext context)
        {
            _listener = context.CreateEntity().AddAnyLoadingAddedListener(this);
        }

        public Entity Entity { get; private set; }
        public bool Unsubscribe { get; set; }
        public bool DestroyListener { get; set; }

        public void OnAnyLoadingAdded(MyApp.Main.Entity entity)
        {
            Entity = entity;
            if (Unsubscribe)
            {
                _listener.RemoveAnyLoadingAddedListener(this);
            }

            if (DestroyListener)
            {
                _listener.Destroy();
            }
        }
    }
}
