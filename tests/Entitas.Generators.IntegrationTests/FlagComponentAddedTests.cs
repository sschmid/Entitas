using FluentAssertions;
using MyApp;
using MyFeature;
using Xunit;

namespace Entitas.Generators.IntegrationTests
{
    public class FlagComponentAddedTests
    {
        readonly MainContext _context;
        readonly MyApp.Main.Entity _entity;
        readonly LoadingAddedListener _listener;
        readonly MyAppMainLoadingAddedEventSystem _system;

        public FlagComponentAddedTests()
        {
            ContextInitialization.InitializeMain();
            _context = new MainContext();
            _entity = _context.CreateEntity();
            _listener = new LoadingAddedListener(_entity);
            _system = new MyAppMainLoadingAddedEventSystem(_context);
        }

        [Fact]
        public void IsNullWhenNothingChanged()
        {
            _system.Execute();
            _listener.Entity.Should().BeNull();
        }

        [Fact]
        public void PassesEntityWhenAddedOnSameEntity()
        {
            _entity.AddLoading();
            _system.Execute();
            _listener.Entity.Should().BeSameAs(_entity);
        }

        [Fact]
        public void DoesNotPassEntityWhenAddedOnDifferentEntity()
        {
            _context.CreateEntity().AddLoading();
            _system.Execute();
            _listener.Entity.Should().BeNull();
        }

        [Fact]
        public void SkipsWhenTriggeringComponentHasBeenRemoved()
        {
            _entity
                .AddLoading()
                .RemoveLoading();

            _system.Execute();
            _listener.Entity.Should().BeNull();
        }

        [Fact]
        public void CanUnsubscribeInCallback()
        {
            _listener.Unsubscribe = true;
            _entity.AddLoading();
            _system.Execute();
            _listener.Entity.Should().BeSameAs(_entity);
        }

        [Fact]
        public void CanDestroyListenerEntityInCallback()
        {
            _listener.DestroyListener = true;
            _entity.AddLoading();
            _system.Execute();
            _listener.Entity.Should().BeSameAs(_entity);
        }
    }

#nullable disable

    public class LoadingAddedListener : IMyAppMainLoadingAddedListener
    {
        readonly MyApp.Main.Entity _listener;

        public LoadingAddedListener(MyApp.Main.Entity entity)
        {
            _listener = entity.AddLoadingAddedListener(this);
        }

        public Entity Entity { get; private set; }
        public bool Unsubscribe { get; set; }
        public bool DestroyListener { get; set; }

        public void OnLoadingAdded(MyApp.Main.Entity entity)
        {
            Entity = entity;
            if (Unsubscribe)
            {
                _listener.RemoveLoadingAddedListener(this);
            }

            if (DestroyListener)
            {
                _listener.Destroy();
            }
        }
    }
}
