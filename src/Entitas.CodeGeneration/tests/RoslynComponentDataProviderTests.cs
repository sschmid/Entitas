using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Roslyn;
using DesperateDevs.Serialization;
using Entitas.CodeGeneration.Attributes;
using Microsoft.CodeAnalysis;
using My.Namespace;
using Entitas.CodeGeneration.Plugins;
using FluentAssertions;
using Xunit;

namespace Entitas.CodeGeneration.Tests
{
    public class RoslynComponentDataProviderTests
    {
        static readonly string ProjectRoot = TestExtensions.GetProjectRoot();
        static readonly string ProjectPath = $"{ProjectRoot}/Tests/TestFixtures/TestFixtures.csproj";
        static readonly string RoslynProjectPath = $"{ProjectRoot}/src/Entitas.Roslyn.CodeGeneration.Plugins/fixtures/Entitas.Roslyn.CodeGeneration.Plugins.Tests.Project";

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
            _componentData.GetTypeName().Should().Be(typeof(MyNamespaceComponent).ToCompilableString());
        }

        [Fact]
        public void GetsContexts()
        {
            var contextNames = _componentData.GetContextNames();
            contextNames.Length.Should().Be(2);
            contextNames[0].Should().Be("Test");
            contextNames[1].Should().Be("Test2");
        }

        [Fact]
        public void SetsFirstContextAsDefaultWhenComponentHasNoContext()
        {
            var contextNames = GetData<NoContextComponent>().GetContextNames();
            contextNames.Length.Should().Be(1);
            contextNames[0].Should().Be("Game");
        }

        [Fact]
        public void GetsUnique()
        {
            _componentData.IsUnique().Should().BeFalse();
            GetData<UniqueStandardComponent>().IsUnique().Should().BeTrue();
        }

        [Fact]
        public void GetsFieldAsMemberData()
        {
            GetData<ComponentWithFields>().GetMemberData().Length.Should().Be(1);
        }

        [Fact]
        public void GetsPropertyAsMemberData()
        {
            GetData<ComponentWithProperties>().GetMemberData().Length.Should().Be(1);
        }

        [Fact]
        public void GetsMemberDataFromBaseClass()
        {
            GetData<InheritedComponent>().GetMemberData().Length.Should().Be(1);
        }

        [Fact]
        public void GetsGenerateComponent()
        {
            _componentData.ShouldGenerateComponent().Should().BeFalse();
            _componentData.ContainsKey(ShouldGenerateComponentComponentDataExtension.COMPONENT_OBJECT_TYPE).Should().BeFalse();
        }

        [Fact]
        public void GetsGenerateIndex()
        {
            _componentData.ShouldGenerateIndex().Should().BeTrue();
            GetData<DontGenerateIndexComponent>().ShouldGenerateIndex().Should().BeFalse();
        }

        [Fact]
        public void GetsGenerateMethods()
        {
            _componentData.ShouldGenerateMethods().Should().BeTrue();
            GetData<DontGenerateMethodsComponent>().ShouldGenerateMethods().Should().BeFalse();
        }

        [Fact]
        public void GetsFlagPrefix()
        {
            _componentData.GetFlagPrefix().Should().Be("is");
            GetData<CustomPrefixFlagComponent>().GetFlagPrefix().Should().Be("My");
        }

        [Fact]
        public void GetsIsNoEvent()
        {
            _componentData.IsEvent().Should().BeFalse();
        }

        [Fact]
        public void GetsEvent()
        {
            GetData<StandardEventComponent>().IsEvent().Should().BeTrue();
        }

        [Fact]
        public void GetsMultipleEvents()
        {
            var d = GetData<MultipleEventsStandardEventComponent>();
            d.IsEvent().Should().BeTrue();
            var eventData = d.GetEventData();
            eventData.Length.Should().Be(2);

            eventData[0].eventTarget.Should().Be(EventTarget.Any);
            eventData[0].eventType.Should().Be(EventType.Added);
            eventData[0].priority.Should().Be(1);

            eventData[1].eventTarget.Should().Be(EventTarget.Self);
            eventData[1].eventType.Should().Be(EventType.Removed);
            eventData[1].priority.Should().Be(2);
        }

        [Fact]
        public void GetsEventTarget()
        {
            GetData<StandardEventComponent>().GetEventData()[0].eventTarget.Should().Be(EventTarget.Any);
            GetData<StandardEntityEventComponent>().GetEventData()[0].eventTarget.Should().Be(EventTarget.Self);
        }

        [Fact]
        public void GetsEventType()
        {
            GetData<StandardEventComponent>().GetEventData()[0].eventType.Should().Be(EventType.Added);
            GetData<StandardEntityEventComponent>().GetEventData()[0].eventType.Should().Be(EventType.Removed);
        }

        [Fact]
        public void GetsEventPriority()
        {
            GetData<StandardEntityEventComponent>().GetEventData()[0].priority.Should().Be(1);
        }

        [Fact]
        public void CreatesDataForEventListeners()
        {
            var d = GetMultipleData<StandardEventComponent>();
            d.Length.Should().Be(2);
            d[1].IsEvent().Should().BeFalse();
            d[1].GetTypeName().Should().Be("AnyStandardEventListenerComponent");
            d[1].GetMemberData().Length.Should().Be(1);
            d[1].GetMemberData()[0].name.Should().Be("value");
            d[1].GetMemberData()[0].type.Should().Be("System.Collections.Generic.List<IAnyStandardEventListener>");
        }

        [Fact]
        public void CreatesDataForUniqueEventListeners()
        {
            var d = GetMultipleData<UniqueEventComponent>();
            d.Length.Should().Be(2);
            d[1].IsEvent().Should().BeFalse();
            d[1].IsUnique().Should().BeFalse();
        }

        [Fact]
        public void CreatesDataForEventListenersWithMultipleContexts()
        {
            var d = GetMultipleData<MultipleContextStandardEventComponent>();
            d.Length.Should().Be(3);
            d[1].IsEvent().Should().BeFalse();
            d[1].GetTypeName().Should().Be("TestAnyMultipleContextStandardEventListenerComponent");
            d[1].GetMemberData().Length.Should().Be(1);
            d[1].GetMemberData()[0].name.Should().Be("value");
            d[1].GetMemberData()[0].type.Should().Be("System.Collections.Generic.List<ITestAnyMultipleContextStandardEventListener>");

            d[2].IsEvent().Should().BeFalse();
            d[2].GetTypeName().Should().Be("Test2AnyMultipleContextStandardEventListenerComponent");
            d[2].GetMemberData().Length.Should().Be(1);
            d[2].GetMemberData()[0].name.Should().Be("value");
            d[2].GetMemberData()[0].type.Should().Be("System.Collections.Generic.List<ITest2AnyMultipleContextStandardEventListener>");
        }

        [Fact]
        public void ResolvesGeneratedContextAttribute()
        {
            var data = GetData<GeneratedContextComponent>();
            var contextNames = data.GetContextNames();
            contextNames.Length.Should().Be(1);
            contextNames[0].Should().Be("Game");
        }

        [Fact]
        public void IgnoresUnknownAttributes()
        {
            var parser = new ProjectParser(RoslynProjectPath);
            var symbol = parser.GetTypes().Single(c => c.ToCompilableString() == "UnknownContextComponent");
            var provider = new Entitas.Roslyn.CodeGeneration.Plugins.ComponentDataProvider(new[] {symbol});
            provider.Configure(new TestPreferences("Entitas.CodeGeneration.Plugins.Contexts = Game, GameState"));
            var data = (ComponentData)provider.GetData()[0];
            var contextNames = data.GetContextNames();
            contextNames.Length.Should().Be(1);
            contextNames[0].Should().Be("Game");
        }

        [Fact]
        public void ResolvesKnownAttributes()
        {
            var parser = new ProjectParser(RoslynProjectPath);
            var symbol = parser.GetTypes().Single(c => c.ToCompilableString() == "UnknownContextComponent");
            var provider = new Entitas.Roslyn.CodeGeneration.Plugins.ComponentDataProvider(new[] {symbol});
            provider.Configure(new TestPreferences("Entitas.CodeGeneration.Plugins.Contexts = KnownContext"));
            var data = (ComponentData)provider.GetData()[0];
            var contextNames = data.GetContextNames();
            contextNames.Length.Should().Be(1);
            contextNames[0].Should().Be("KnownContext");
        }

        [Fact]
        public void SupportsHigherRankArrays()
        {
            var memberData = GetData<Array3dComponent>().GetMemberData();
            memberData.Length.Should().Be(1);
            memberData[0].type.Should().Be("int[,,]");
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
            _classData.GetTypeName().Should().Be("ClassToGenerateComponent");
        }

        [Fact]
        public void GetsContextsForClass()
        {
            var contextNames = _classData.GetContextNames();
            contextNames.Length.Should().Be(2);
            contextNames[0].Should().Be("Test");
            contextNames[1].Should().Be("Test2");
        }

        [Fact]
        public void GetsUniqueForClass()
        {
            _classData.IsUnique().Should().BeFalse();
        }

        [Fact]
        public void GetsMemberDataForClass()
        {
            _classData.GetMemberData().Length.Should().Be(1);
            _classData.GetMemberData()[0].type.Should().Be(typeof(ClassToGenerate).ToCompilableString());
        }

        [Fact]
        public void GetsGenerateComponentForClass()
        {
            _classData.ShouldGenerateComponent().GetType().Should().Be(typeof(bool));
            _classData.ShouldGenerateComponent().Should().BeTrue();
            _classData.GetObjectTypeName().Should().Be(typeof(ClassToGenerate).ToCompilableString());
        }

        [Fact]
        public void GetsGenerateIndexForClass()
        {
            _classData.ShouldGenerateIndex().Should().BeTrue();
        }

        [Fact]
        public void GetsGenerateMethodsForClass()
        {
            _classData.ShouldGenerateMethods().Should().BeTrue();
        }

        [Fact]
        public void GetsFlagPrefixForClass()
        {
            _classData.GetFlagPrefix().Should().Be("is");
        }

        [Fact]
        public void GetsIsNoEventForClass()
        {
            _classData.IsEvent().Should().BeFalse();
        }

        [Fact]
        public void GetsEventForClass()
        {
            GetData<EventToGenerate>().GetEventData().Length.Should().Be(1);
            var eventData = GetData<EventToGenerate>().GetEventData()[0];
            eventData.eventTarget.Should().Be(EventTarget.Any);
            eventData.eventType.Should().Be(EventType.Added);
            eventData.priority.Should().Be(0);
        }

        [Fact]
        public void CreatesDataForEventListenersForClass()
        {
            var d = GetMultipleData<EventToGenerate>();
            d.Length.Should().Be(3);
            d[1].IsEvent().Should().BeFalse();
            d[1].ShouldGenerateComponent().Should().BeFalse();
            d[1].GetTypeName().Should().Be("TestAnyEventToGenerateListenerComponent");
            d[1].GetMemberData().Length.Should().Be(1);
            d[1].GetMemberData()[0].name.Should().Be("value");
            d[1].GetMemberData()[0].type.Should().Be("System.Collections.Generic.List<ITestAnyEventToGenerateListener>");

            d[2].IsEvent().Should().BeFalse();
            d[2].ShouldGenerateComponent().Should().BeFalse();
            d[2].GetTypeName().Should().Be("Test2AnyEventToGenerateListenerComponent");
            d[2].GetMemberData().Length.Should().Be(1);
            d[2].GetMemberData()[0].name.Should().Be("value");
            d[2].GetMemberData()[0].type.Should().Be("System.Collections.Generic.List<ITest2AnyEventToGenerateListener>");
        }

        [Fact]
        public void CreatesDataForEachType()
        {
            var symbol1 = Types.Single(c => c.ToCompilableString() == typeof(NameAgeComponent).FullName);
            var symbol2 = Types.Single(c => c.ToCompilableString() == typeof(Test2ContextComponent).FullName);
            var provider = new Entitas.Roslyn.CodeGeneration.Plugins.ComponentDataProvider(new[] {symbol1, symbol2});
            provider.Configure(
                new TestPreferences("Entitas.CodeGeneration.Plugins.Contexts = Game, GameState")
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

            data1.GetObjectTypeName().Should().Be(type.ToCompilableString());
            data2.GetObjectTypeName().Should().Be(type.ToCompilableString());

            data1.GetTypeName().Should().Be("NewCustomNameComponent1Component");
            data2.GetTypeName().Should().Be("NewCustomNameComponent2Component");
        }

        [Fact]
        public void IgnoresDuplicatesFromNonComponents()
        {
            var types = new[] {typeof(ClassToGenerate), typeof(ClassToGenerateComponent)};
            var provider = new ComponentDataProvider(types);
            provider.Configure(new TestPreferences(
                "Entitas.CodeGeneration.Plugins.Contexts = Game, GameState"
            ));
            var data = provider.GetData();
            data.Length.Should().Be(1);
        }

        [Fact]
        public void GetsDefaultContext()
        {
            var preferences = new TestPreferences("Entitas.CodeGeneration.Plugins.Contexts = ConfiguredContext" + "\n");
            var data = GetData<NoContextComponent>(preferences);
            var contextNames = data.GetContextNames();
            contextNames.Length.Should().Be(1);
            contextNames[0].Should().Be("ConfiguredContext");
        }

        ComponentData GetData<T>(Preferences preferences = null) => GetMultipleData<T>(preferences)[0];

        ComponentData[] GetMultipleData<T>(Preferences preferences = null)
        {
            var type = typeof(T);
            var symbol = Types.Single(c => c.ToCompilableString() == type.FullName);
            var provider = new Entitas.Roslyn.CodeGeneration.Plugins.ComponentDataProvider(new[] {symbol});
            preferences ??= new TestPreferences(
                @"Entitas.CodeGeneration.Plugins.Contexts = Game, GameState
Entitas.CodeGeneration.Plugins.IgnoreNamespaces = false");

            provider.Configure(preferences);
            return (ComponentData[])provider.GetData();
        }
    }
}
