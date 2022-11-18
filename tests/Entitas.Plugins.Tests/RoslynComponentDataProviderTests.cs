using System.IO;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.Plugins.Attributes;
using Microsoft.CodeAnalysis;
using My.Namespace;
using FluentAssertions;
using Xunit;

namespace Entitas.Plugins.Tests
{
    [Collection("Entitas.Plugins.Tests")]
    public class RoslynComponentDataProviderTests
    {
        static readonly string ProjectRoot = TestExtensions.GetProjectRoot();
        static readonly string ProjectPath = Path.Combine(ProjectRoot, "tests", "Fixtures", "Fixtures.csproj");
        static readonly string RoslynProjectPath = Path.Combine(ProjectRoot, "tests", "Entitas.Roslyn.CodeGeneration.Plugins.Tests", "fixtures", "exclude", "Entitas.Roslyn.CodeGeneration.Plugins.Tests.Project");

        INamedTypeSymbol[] Types => _types ??= new ProjectParser(ProjectPath).GetTypes();
        INamedTypeSymbol[] _types;

        readonly ComponentData _componentData;
        readonly ComponentData _classData;

        public RoslynComponentDataProviderTests()
        {
            _componentData = GetData<MyNamespaceComponent>();
            _classData = GetData<ClassToGenerate>();
        }

        [Fact]
        public void GetsFullTypeName()
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
        public void GetsFieldAsMemberData()
        {
            GetData<ComponentWithFields>().MemberData.Length.Should().Be(1);
        }

        [Fact]
        public void GetsPropertyAsMemberData()
        {
            GetData<ComponentWithProperties>().MemberData.Length.Should().Be(1);
        }

        [Fact]
        public void GetsMemberDataFromBaseClass()
        {
            GetData<InheritedComponent>().MemberData.Length.Should().Be(1);
        }

        [Fact]
        public void GetsGenerateComponent()
        {
            _componentData.GeneratesObject.Should().BeFalse();
            _componentData.ContainsKey(ComponentData.ObjectTypeKey).Should().BeFalse();
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
            d[1].MemberData.Length.Should().Be(1);
            d[1].MemberData[0].Name.Should().Be("value");
            d[1].MemberData[0].Type.Should().Be("System.Collections.Generic.List<IAnyStandardEventListener>");
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
            d[1].MemberData.Length.Should().Be(1);
            d[1].MemberData[0].Name.Should().Be("value");
            d[1].MemberData[0].Type.Should().Be("System.Collections.Generic.List<ITest1AnyMultipleContextStandardEventListener>");

            d[2].IsEvent.Should().BeFalse();
            d[2].Type.Should().Be("Test2AnyMultipleContextStandardEventListenerComponent");
            d[2].MemberData.Length.Should().Be(1);
            d[2].MemberData[0].Name.Should().Be("value");
            d[2].MemberData[0].Type.Should().Be("System.Collections.Generic.List<ITest2AnyMultipleContextStandardEventListener>");
        }

        [Fact]
        public void ResolvesGeneratedContextAttribute()
        {
            var data = GetData<GeneratedContextComponent>();
            var contexts = data.Contexts;
            contexts.Length.Should().Be(1);
            contexts[0].Should().Be("Game");
        }

        [Fact]
        public void IgnoresUnknownAttributes()
        {
            var parser = new ProjectParser(RoslynProjectPath);
            var symbol = parser.GetTypes().Single(c => c.ToCompilableString() == "UnknownContextComponent");
            var provider = new Roslyn.ComponentDataProvider(new[] {symbol});
            provider.Configure(new TestPreferences("Entitas.Plugins.Contexts = Game, GameState"));
            var data = (ComponentData)provider.GetData()[0];
            var contexts = data.Contexts;
            contexts.Length.Should().Be(1);
            contexts[0].Should().Be("Game");
        }

        [Fact]
        public void ResolvesKnownAttributes()
        {
            var parser = new ProjectParser(RoslynProjectPath);
            var symbol = parser.GetTypes().Single(c => c.ToCompilableString() == "UnknownContextComponent");
            var provider = new Roslyn.ComponentDataProvider(new[] {symbol});
            provider.Configure(new TestPreferences("Entitas.Plugins.Contexts = KnownContext"));
            var data = (ComponentData)provider.GetData()[0];
            var contexts = data.Contexts;
            contexts.Length.Should().Be(1);
            contexts[0].Should().Be("KnownContext");
        }

        [Fact]
        public void SupportsHigherRankArrays()
        {
            var memberData = GetData<Array3dComponent>().MemberData;
            memberData.Length.Should().Be(1);
            memberData[0].Type.Should().Be("int[,,]");
        }

        [Fact]
        public void GetDataForClass()
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
            d[1].MemberData[0].Name.Should().Be("value");
            d[1].MemberData[0].Type.Should().Be("System.Collections.Generic.List<ITest1AnyEventToGenerateListener>");

            d[2].IsEvent.Should().BeFalse();
            d[2].GeneratesObject.Should().BeFalse();
            d[2].Type.Should().Be("Test2AnyEventToGenerateListenerComponent");
            d[2].MemberData.Length.Should().Be(1);
            d[2].MemberData[0].Name.Should().Be("value");
            d[2].MemberData[0].Type.Should().Be("System.Collections.Generic.List<ITest2AnyEventToGenerateListener>");
        }

        [Fact]
        public void CreatesDataForEachType()
        {
            var symbol1 = Types.Single(c => c.ToCompilableString() == typeof(NameAgeComponent).FullName);
            var symbol2 = Types.Single(c => c.ToCompilableString() == typeof(Test2ContextComponent).FullName);
            var provider = new Roslyn.ComponentDataProvider(new[] {symbol1, symbol2});
            provider.Configure(
                new TestPreferences("Entitas.Plugins.Contexts = Game, GameState")
            );
            var data = provider.GetData();
            data.Length.Should().Be(2);
        }

        [Fact]
        public void CreatesDataForEachCustomComponentName()
        {
            var type = typeof(CustomName);
            var data = GetMultipleData<CustomName>();
            var data1 = data[0];
            var data2 = data[1];

            data1.ObjectType.Should().Be(type.ToCompilableString());
            data2.ObjectType.Should().Be(type.ToCompilableString());

            data1.Type.Should().Be("NewCustomNameComponent1Component");
            data2.Type.Should().Be("NewCustomNameComponent2Component");
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
        public void GetsDefaultContext()
        {
            var preferences = new TestPreferences("Entitas.Plugins.Contexts = ConfiguredContext" + "\n");
            var data = GetData<NoContextComponent>(preferences);
            var contexts = data.Contexts;
            contexts.Length.Should().Be(1);
            contexts[0].Should().Be("ConfiguredContext");
        }

        ComponentData GetData<T>(Preferences preferences = null) => GetMultipleData<T>(preferences)[0];

        ComponentData[] GetMultipleData<T>(Preferences preferences = null)
        {
            var type = typeof(T);
            var symbol = Types.Single(c => c.ToCompilableString() == type.FullName);
            var provider = new Roslyn.ComponentDataProvider(new[] {symbol});
            preferences ??= new TestPreferences(
                @"Entitas.Plugins.Contexts = Game, GameState
Entitas.Plugins.IgnoreNamespaces = false");

            provider.Configure(preferences);
            return (ComponentData[])provider.GetData();
        }
    }
}
