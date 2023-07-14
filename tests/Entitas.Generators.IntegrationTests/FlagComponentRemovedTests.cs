using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class FlagComponentRemovedTests
    {
        readonly MainContext _context;
        readonly MyApp.Main.Entity _entity;
        readonly LoadingRemovedListener _listener;
        readonly MyAppMainLoadingRemovedEventSystem _system;

        public FlagComponentRemovedTests()
        {
            ContextInitialization.InitializeMain();
            _context = new MainContext();
            _entity = _context.CreateEntity();
            _listener = new LoadingRemovedListener(_entity);
            _system = new MyAppMainLoadingRemovedEventSystem(_context);
        }

        [Fact]
        public void IsNullWhenNothingChanged()
        {
            _entity.AddLoading();
            _system.Execute();
            _listener.Entity.Should().BeNull();
        }

        [Fact]
        public void PassesEntityWhenRemovedOnSameEntity()
        {
            _entity
                .AddLoading()
                .RemoveLoading();

            _system.Execute();
            _listener.Entity.Should().BeSameAs(_entity);
        }

        [Fact]
        public void DoesNotPassEntityWhenRemovedOnDifferentEntity()
        {
            _context.CreateEntity()
                .AddLoading()
                .RemoveLoading();

            _system.Execute();
            _listener.Entity.Should().BeNull();
        }

        [Fact]
        public void SkipsWhenTriggeringComponentHasBeenAdded()
        {
            _entity
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
            _entity
                .AddLoading()
                .RemoveLoading();

            _system.Execute();
            _listener.Entity.Should().BeSameAs(_entity);
        }

        [Fact]
        public void CanDestroyListenerEntityInCallback()
        {
            _listener.DestroyListener = true;
            _entity
                .AddLoading()
                .RemoveLoading();

            _system.Execute();
            _listener.Entity.Should().BeSameAs(_entity);
        }
    }

#nullable disable

    public class LoadingRemovedListener : IMyAppMainLoadingRemovedListener
    {
        readonly MyApp.Main.Entity _listener;

        public LoadingRemovedListener(MyApp.Main.Entity entity)
        {
            _listener = entity.AddLoadingRemovedListener(this);
        }

        public Entity Entity { get; private set; }
        public bool Unsubscribe { get; set; }
        public bool DestroyListener { get; set; }

        public void OnLoadingRemoved(MyApp.Main.Entity entity)
        {
            Entity = entity;
            if (Unsubscribe)
            {
                _listener.RemoveLoadingRemovedListener(this);
            }

            if (DestroyListener)
            {
                _listener.Destroy();
            }
        }
    }
}
