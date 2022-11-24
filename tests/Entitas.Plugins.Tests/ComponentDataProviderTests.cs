using DesperateDevs.Extensions;
using DesperateDevs.Serialization;
using Entitas.Plugins.Attributes;
using FluentAssertions;
using My.Namespace;
using Xunit;

namespace Entitas.Plugins.Tests
{
    public class ComponentDataProviderTests
    {
        readonly ComponentData _componentData;
        readonly ComponentData _classData;

        public ComponentDataProviderTests()
        {
            _componentData = GetData<MyNamespaceComponent>();
            _classData = GetData<ClassToGenerate>();
        }

        [Fact]
        public void GetsData()
        {
            _componentData.Should().NotBeNull();
        }

        [Fact]
        public void GetsType()
        {
            _componentData.Type.Should().Be(typeof(MyNamespaceComponent).ToCompilableString());
        }

        [Fact]
        public void GetsContexts()
        {
            var contexts = _componentData.Contexts;
            contexts.Length.Should().Be(2);
            contexts[0].Should().Be("Test1");
            contexts[1].Should().Be("Test2");
        }

        [Fact]
        public void SetsFirstContextAsDefaultWhenComponentHasNoContext()
        {
            var contexts = GetData<NoContextComponent>().Contexts;
            contexts.Length.Should().Be(1);
            contexts[0].Should().Be("Game");
        }

        [Fact]
        public void GetsUnique()
        {
            _componentData.IsUnique.Should().BeFalse();
            GetData<UniqueStandardComponent>().IsUnique.Should().BeTrue();
        }

        [Fact]
        public void GetsMemberData()
        {
            _componentData.MemberData.Length.Should().Be(1);
            _componentData.MemberData[0].Type.Should().Be("string");
        }

        [Fact]
        public void GetsGenerateComponent()
        {
            _componentData.GeneratesObject.Should().BeFalse();
            _componentData.ObjectType.Should().BeNull();
        }

        [Fact]
        public void GetsGenerateIndex()
        {
            _componentData.GeneratesIndex.Should().BeTrue();
            GetData<DontGenerateIndexComponent>().GeneratesIndex.Should().BeFalse();
        }

        [Fact]
        public void GetsGenerateMethods()
        {
            _componentData.Generates.Should().BeTrue();
            GetData<DontGenerateMethodsComponent>().Generates.Should().BeFalse();
        }

        [Fact]
        public void GetsFlagPrefix()
        {
            _componentData.FlagPrefix.Should().Be("is");
            GetData<CustomPrefixFlagComponent>().FlagPrefix.Should().Be("My");
        }

        [Fact]
        public void GetsIsNoEvent()
        {
            _componentData.IsEvent.Should().BeFalse();
        }

        [Fact]
        public void GetsEvent()
        {
            GetData<StandardEventComponent>().IsEvent.Should().BeTrue();
        }

        [Fact]
        public void GetsMultipleEvents()
        {
            var d = GetData<MultipleEventsStandardEventComponent>();
            d.IsEvent.Should().BeTrue();
            var eventData = d.EventData;
            eventData.Length.Should().Be(2);

            eventData[0].EventTarget.Should().Be(EventTarget.Any);
            eventData[0].EventType.Should().Be(EventType.Added);
            eventData[0].Order.Should().Be(1);

            eventData[1].EventTarget.Should().Be(EventTarget.Self);
            eventData[1].EventType.Should().Be(EventType.Removed);
            eventData[1].Order.Should().Be(2);
        }

        [Fact]
        public void GetsEventTarget()
        {
            GetData<StandardEventComponent>().EventData[0].EventTarget.Should().Be(EventTarget.Any);
            GetData<StandardEntityEventComponent>().EventData[0].EventTarget.Should().Be(EventTarget.Self);
        }

        [Fact]
        public void GetsEventType()
        {
            GetData<StandardEventComponent>().EventData[0].EventType.Should().Be(EventType.Added);
            GetData<StandardEntityEventComponent>().EventData[0].EventType.Should().Be(EventType.Removed);
        }

        [Fact]
        public void GetsEventOrder()
        {
            GetData<StandardEntityEventComponent>().EventData[0].Order.Should().Be(1);
        }

        [Fact]
        public void CreatesDataForEventListeners()
        {
            var d = GetMultipleData<StandardEventComponent>();
            d.Length.Should().Be(2);
            d[1].IsEvent.Should().BeFalse();
            d[1].Type.Should().Be("AnyStandardEventListenerComponent");
            var memberData = d[1].MemberData;
            memberData.Length.Should().Be(1);
            memberData[0].Name.Should().Be("Value");
            memberData[0].Type.Should().Be("System.Collections.Generic.List<IAnyStandardEventListener>");
        }

        [Fact]
        public void CreatesDataForUniqueEventListeners()
        {
            var d = GetMultipleData<UniqueEventComponent>();
            d.Length.Should().Be(2);
            d[1].IsEvent.Should().BeFalse();
            d[1].IsUnique.Should().BeFalse();
        }

        [Fact]
        public void CreatesDataForEventListenersWithMultipleContexts()
        {
            var d = GetMultipleData<MultipleContextStandardEventComponent>();
            d.Length.Should().Be(3);
            d[1].IsEvent.Should().BeFalse();
            d[1].Type.Should().Be("Test1AnyMultipleContextStandardEventListenerComponent");
            var memberData1 = d[1].MemberData;
            memberData1.Length.Should().Be(1);
            memberData1[0].Name.Should().Be("Value");
            memberData1[0].Type.Should().Be("System.Collections.Generic.List<ITest1AnyMultipleContextStandardEventListener>");

            d[2].IsEvent.Should().BeFalse();
            d[2].Type.Should().Be("Test2AnyMultipleContextStandardEventListenerComponent");
            var memberData2 = d[2].MemberData;
            memberData2.Length.Should().Be(1);
            memberData2[0].Name.Should().Be("Value");
            memberData2[0].Type.Should().Be("System.Collections.Generic.List<ITest2AnyMultipleContextStandardEventListener>");
        }

        [Fact]
        public void GetsDataForClass()
        {
            _classData.Should().NotBeNull();
        }

        [Fact]
        public void GetsFullTypeNameForClass()
        {
            // Not the type, but the component that should be generated
            _classData.Type.Should().Be("ClassToGenerateComponent");
        }

        [Fact]
        public void GetsContextsForClass()
        {
            var contexts = _classData.Contexts;
            contexts.Length.Should().Be(2);
            contexts[0].Should().Be("Test1");
            contexts[1].Should().Be("Test2");
        }

        [Fact]
        public void GetsUniqueForClass()
        {
            _classData.IsUnique.Should().BeFalse();
        }

        [Fact]
        public void GetsMemberDataForClass()
        {
            _classData.MemberData.Length.Should().Be(1);
            _classData.MemberData[0].Type.Should().Be(typeof(ClassToGenerate).ToCompilableString());
        }

        [Fact]
        public void GetsGenerateComponentForClass()
        {
            _classData.GeneratesObject.Should().BeTrue();
            _classData.ObjectType.Should().Be(typeof(ClassToGenerate).ToCompilableString());
        }

        [Fact]
        public void GetsGenerateIndexForClass()
        {
            _classData.GeneratesIndex.Should().BeTrue();
        }

        [Fact]
        public void GetsGenerateMethodsForClass()
        {
            _classData.Generates.Should().BeTrue();
        }

        [Fact]
        public void GetsFlagPrefixForClass()
        {
            _classData.FlagPrefix.Should().Be("is");
        }

        [Fact]
        public void GetsIsNoEventForClass()
        {
            _classData.IsEvent.Should().BeFalse();
        }

        [Fact]
        public void GetsEventForClass()
        {
            GetData<EventToGenerate>().EventData.Length.Should().Be(1);
            var eventData = GetData<EventToGenerate>().EventData[0];
            eventData.EventTarget.Should().Be(EventTarget.Any);
            eventData.EventType.Should().Be(EventType.Added);
            eventData.Order.Should().Be(0);
        }

        [Fact]
        public void CreatesDataForEventListenersForClass()
        {
            var d = GetMultipleData<EventToGenerate>();
            d.Length.Should().Be(3);
            d[1].IsEvent.Should().BeFalse();
            d[1].GeneratesObject.Should().BeFalse();
            d[1].Type.Should().Be("Test1AnyEventToGenerateListenerComponent");
            d[1].MemberData.Length.Should().Be(1);
            d[1].MemberData[0].Name.Should().Be("Value");
            d[1].MemberData[0].Type.Should().Be("System.Collections.Generic.List<ITest1AnyEventToGenerateListener>");

            d[2].IsEvent.Should().BeFalse();
            d[2].GeneratesObject.Should().BeFalse();
            d[2].Type.Should().Be("Test2AnyEventToGenerateListenerComponent");
            d[2].MemberData.Length.Should().Be(1);
            d[2].MemberData[0].Name.Should().Be("Value");
            d[2].MemberData[0].Type.Should().Be("System.Collections.Generic.List<ITest2AnyEventToGenerateListener>");
        }

        [Fact]
        public void CreatesDataForEachType()
        {
            var types = new[] {typeof(NameAgeComponent), typeof(Test2ContextComponent)};
            var provider = new ComponentDataProvider(types);
            provider.Configure(new TestPreferences(
                "Entitas.Plugins.Contexts = Game, GameState"
            ));
            var data = provider.GetData();
            data.Length.Should().Be(types.Length);
        }

        [Fact]
        public void IgnoresDuplicatesFromNonComponents()
        {
            var types = new[] {typeof(ClassToGenerate), typeof(ClassToGenerateComponent)};
            var provider = new ComponentDataProvider(types);
            provider.Configure(new TestPreferences(
                "Entitas.Plugins.Contexts = Game, GameState"
            ));
            var data = provider.GetData();
            data.Length.Should().Be(1);
        }

        [Fact]
        public void CreatesDataForEachCustomComponentName()
        {
            var type = typeof(CustomName);
            var data = GetMultipleData<CustomName>();
            data[0].ObjectType.Should().Be(type.ToCompilableString());
            data[0].ObjectType.Should().Be(type.ToCompilableString());
            data[1].ObjectType.Should().Be(type.ToCompilableString());
            data[0].Type.Should().Be("NewCustomNameComponent1Component");
            data[1].Type.Should().Be("NewCustomNameComponent2Component");
        }

        [Fact]
        public void GetsDefaultContext()
        {
            var preferences = new TestPreferences(
                "Entitas.Plugins.Contexts = ConfiguredContext" + "\n"
            );

            var data = GetData<NoContextComponent>(preferences);
            var contexts = data.Contexts;
            contexts.Length.Should().Be(1);
            contexts[0].Should().Be("ConfiguredContext");
        }

        ComponentData GetData<T>(Preferences preferences = null) => GetMultipleData<T>(preferences)[0];

        ComponentData[] GetMultipleData<T>(Preferences preferences = null)
        {
            var provider = new ComponentDataProvider(new[] {typeof(T)});
            preferences ??= new TestPreferences(
                @"Entitas.Plugins.Contexts = Game, GameState
Entitas.Plugins.IgnoreNamespaces = false");

            provider.Configure(preferences);

            return (ComponentData[])provider.GetData();
        }
    }
}
