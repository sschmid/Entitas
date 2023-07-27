using System;
using Xunit;
using Xunit.Abstractions;

namespace Entitas.Tests
{
    public class EntitasErrorMessagesTests
    {
        readonly ITestOutputHelper _output;
        readonly TestContext _context;
        readonly TestEntity _entity;

        public EntitasErrorMessagesTests(ITestOutputHelper output)
        {
            _output = output;
            var componentNames = new[] { "Health", "Position", "View" };
            var contextInfo = new ContextInfo("My Context", componentNames, null);
            _context = new TestContext(componentNames.Length, 42, contextInfo);
            _entity = _context.CreateEntity();
        }

        [Fact]
        public void WhenAddingComponentToDestroyedEntity()
        {
            _entity.Destroy();
            PrintErrorMessage(() => _entity.AddComponentA());
        }

        [Fact]
        public void WhenRemovingComponentFromDestroyedEntity()
        {
            _entity.Destroy();
            PrintErrorMessage(() => _entity.RemoveComponentA());
        }

        [Fact]
        public void WhenReplacingComponentOnDestroyedEntity()
        {
            _entity.Destroy();
            PrintErrorMessage(() => _entity.ReplaceComponentA(Component.A));
        }

        [Fact]
        public void WhenAddingComponentTwice()
        {
            _entity.AddComponentA();
            PrintErrorMessage(() => _entity.AddComponentA());
        }

        [Fact]
        public void WhenRemovingComponentThatDoesNotExist()
        {
            PrintErrorMessage(() => _entity.RemoveComponentA());
        }

        [Fact]
        public void WhenGettingComponentThatDoesNotExist()
        {
            PrintErrorMessage(() => _entity.GetComponentA());
        }

        [Fact]
        public void WhenRetainingEntityTwice()
        {
            var owner = new object();
            _entity.Retain(owner);
            PrintErrorMessage(() => _entity.Retain(owner));
        }

        [Fact]
        public void WhenReleasingEntityWithWrongOwner()
        {
            PrintErrorMessage(() => _entity.Release(new object()));
        }

        [Fact]
        public void WhenGettingSingleEntityFromGroupWhenMultipleExist()
        {
            _context.CreateEntity().AddComponentA();
            _context.CreateEntity().AddComponentA();
            var matcher = (Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentA);
            matcher.ComponentNames = _context.ContextInfo.ComponentNames;
            PrintErrorMessage(() => _context.GetGroup(matcher).GetSingleEntity());
        }

        [Fact]
        public void WhenCreatingUnbalancedGroup()
        {
            PrintErrorMessage(() => new Collector<TestEntity>(new[]
                {
                    new Group<TestEntity>(Matcher<TestEntity>.AllOf(CID.ComponentA)),
                    new Group<TestEntity>(Matcher<TestEntity>.AllOf(CID.ComponentB))
                },
                new[] { GroupEvent.Added }));
        }

        [Fact]
        public void WhenWrongContextInfoComponentNamesCount()
        {
            var componentNames = new[] { "Health", "Position", "View" };
            var contextInfo = new ContextInfo("My Context", componentNames, null);
            PrintErrorMessage(() => new TestContext(999, 0, contextInfo));
        }

        [Fact]
        public void WhenDestroyingRetainedEntity()
        {
            var entity = _context.CreateEntity();
            entity.Retain(this);
            entity.Retain(new object());

            entity = _context.CreateEntity();
            entity.Retain(this);
            entity.Retain(new object());

            PrintErrorMessage(() => _context.DestroyAllEntities());
        }

        [Fact]
        public void WhenReleasingEntityBeforeDestroy()
        {
            PrintErrorMessage(() => _entity.Release(_context));
        }

        [Fact]
        public void WhenUnknownEntityIndex()
        {
            PrintErrorMessage(() => _context.GetEntityIndex("unknown"));
        }

        [Fact]
        public void WhenDuplicateEntityIndex()
        {
            var index = new PrimaryEntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup((Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentA)),
                (_, _) => string.Empty
            );
            _context.AddEntityIndex(index);
            PrintErrorMessage(() => _context.AddEntityIndex(index));
        }

        [Fact]
        public void WhenGettingSingleEntityFromCollectionWhenMultipleExist()
        {
            PrintErrorMessage(() => new Entity[2].SingleEntity());
        }

        [Fact]
        public void WhenPrimaryEntityIndexHasMultipleEntitiesForKey()
        {
            new PrimaryEntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup(TestUserMatcher.User),
                (_, c) => ((UserComponent)c).Name
            );

            _context.CreateEntity().AddUser("Test", 42);
            PrintErrorMessage(() => _context.CreateEntity().AddUser("Test", 42));
        }

        void PrintErrorMessage(Action action)
        {
            try
            {
                action();
            }
            catch (Exception exception)
            {
                _output.WriteLine(exception.Message);
            }
        }
    }
}
