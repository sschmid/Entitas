using System;
using Entitas.Blueprints;
using Xunit;
using Xunit.Abstractions;

namespace Entitas.Tests
{
    public class EntitasErrorMessagesTests
    {
        readonly ITestOutputHelper _output;
        readonly MyTestContext _context;
        readonly TestEntity _entity;

        public EntitasErrorMessagesTests(ITestOutputHelper output)
        {
            _output = output;
            var componentNames = new[] {"Health", "Position", "View"};
            var contextInfo = new ContextInfo("My Context", componentNames, null);
            _context = new MyTestContext(componentNames.Length, 42, contextInfo);
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
            matcher.componentNames = _context.contextInfo.componentNames;
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
                new[] {GroupEvent.Added}));
        }

        [Fact]
        public void WhenWrongContextInfoComponentNamesCount()
        {
            var componentNames = new[] {"Health", "Position", "View"};
            var contextInfo = new ContextInfo("My Context", componentNames, null);
            PrintErrorMessage(() => new MyTestContext(1, 0, contextInfo));
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
            PrintErrorMessage(() => new IEntity[2].SingleEntity());
        }

        [Fact]
        public void WhenComponentBluePrintTypeDoesNotImplementIComponent()
        {
            var componentBlueprint = new ComponentBlueprint();
            componentBlueprint.fullTypeName = "string";
            PrintErrorMessage(() => componentBlueprint.CreateComponent(_entity));
        }

        [Fact]
        public void WhenComponentBluePrintTypeDoesNotExist()
        {
            var componentBlueprint = new ComponentBlueprint();
            componentBlueprint.fullTypeName = "UnknownType";
            PrintErrorMessage(() => componentBlueprint.CreateComponent(_entity));
        }

        [Fact]
        public void WhenComponentBluePrintHasInvalidFieldName()
        {
            var componentBlueprint = new ComponentBlueprint();
            componentBlueprint.index = 0;
            componentBlueprint.fullTypeName = typeof(NameAgeComponent).FullName;
            componentBlueprint.members = new[]
            {
                new SerializableMember("xxx", "publicFieldValue"),
                new SerializableMember("publicProperty", "publicPropertyValue")
            };
            PrintErrorMessage(() => componentBlueprint.CreateComponent(_entity));
        }

        [Fact]
        public void WhenPrimaryEntityIndexHasMultipleEntitiesForKey()
        {
            new PrimaryEntityIndex<TestEntity, string>(
                "TestIndex",
                _context.GetGroup((Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentA)),
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
