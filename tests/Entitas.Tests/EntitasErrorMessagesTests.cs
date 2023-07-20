using System;
using Xunit;
using Xunit.Abstractions;

namespace Entitas.Tests
{
    public class EntitasErrorMessagesTests
    {
        readonly ITestOutputHelper _output;
        readonly MyTest1Context _context;
        readonly Test1Entity _entity;

        public EntitasErrorMessagesTests(ITestOutputHelper output)
        {
            _output = output;
            var componentNames = new[] { "Health", "Position", "View" };
            var contextInfo = new ContextInfo("My Context", componentNames, null);
            _context = new MyTest1Context(componentNames.Length, 42, contextInfo);
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
            var matcher = (Matcher<Test1Entity>)Matcher<Test1Entity>.AllOf(CID.ComponentA);
            matcher.componentNames = _context.contextInfo.componentNames;
            PrintErrorMessage(() => _context.GetGroup(matcher).GetSingleEntity());
        }

        [Fact]
        public void WhenCreatingUnbalancedGroup()
        {
            PrintErrorMessage(() => new Collector<Test1Entity>(new[]
                {
                    new Group<Test1Entity>(Matcher<Test1Entity>.AllOf(CID.ComponentA)),
                    new Group<Test1Entity>(Matcher<Test1Entity>.AllOf(CID.ComponentB))
                },
                new[] { GroupEvent.Added }));
        }

        [Fact]
        public void WhenWrongContextInfoComponentNamesCount()
        {
            var componentNames = new[] { "Health", "Position", "View" };
            var contextInfo = new ContextInfo("My Context", componentNames, null);
            PrintErrorMessage(() => new MyTest1Context(1, 0, contextInfo));
        }

        [Fact]
        public void WhenDestroyingRetainedEntity()
        {
            var e = _context.CreateEntity();
            e.Retain(this);
            e.Retain(new object());

            e = _context.CreateEntity();
            e.Retain(this);
            e.Retain(new object());

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
            var index = new PrimaryEntityIndex<Test1Entity, string>(
                "TestIndex",
                _context.GetGroup((Matcher<Test1Entity>)Matcher<Test1Entity>.AllOf(CID.ComponentA)),
                (_, _) => string.Empty
            );
            _context.AddEntityIndex(index);
            PrintErrorMessage(() => _context.AddEntityIndex(index));
        }

        [Fact]
        public void WhenGettingSingleEntityFromCollectionWhenMultipleExist()
        {
            PrintErrorMessage(() => new IEntity[2].SingleEntity());
        }

        [Fact]
        public void WhenPrimaryEntityIndexHasMultipleEntitiesForKey()
        {
            new PrimaryEntityIndex<Test1Entity, string>(
                "TestIndex",
                _context.GetGroup((Matcher<Test1Entity>)Matcher<Test1Entity>.AllOf(CID.ComponentA)),
                (_, c) => ((NameAgeComponent)c).name
            );

            var nameAge = new NameAgeComponent();
            nameAge.name = "Max";
            nameAge.age = 42;

            _context.CreateEntity().AddComponent(CID.ComponentA, nameAge);
            PrintErrorMessage(() => _context.CreateEntity().AddComponent(CID.ComponentA, nameAge));
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
