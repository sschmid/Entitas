using System;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.Plugins;
using My.Namespace;
using NSpec;

class describe_ComponentDataProvider : nspec {

    ComponentData getData<T>(Preferences preferences = null) {
        return getMultipleData<T>(preferences)[0];
    }

    ComponentData[] getMultipleData<T>(Preferences preferences = null) {
        var provider = new ComponentDataProvider(new Type[] { typeof(T) });
        if (preferences == null) {
            preferences = new TestPreferences(
                @"Entitas.CodeGeneration.Plugins.Contexts = Game, GameState
Entitas.CodeGeneration.Plugins.IgnoreNamespaces = false");
        }

        provider.Configure(preferences);

        return (ComponentData[])provider.GetData();
    }

    void when_providing() {

        context["component"] = () => {

            Type type = null;
            ComponentData data = null;

            before = () => {
                type = typeof(MyNamespaceComponent);
                data = getData<MyNamespaceComponent>();
            };

            it["get data"] = () => { data.should_not_be_null(); };

            it["gets full type name"] = () => {
                data.GetTypeName().GetType().should_be(typeof(string));
                data.GetTypeName().should_be(type.ToCompilableString());
            };

            it["gets contexts"] = () => {
                var contextNames = data.GetContextNames();
                contextNames.GetType().should_be(typeof(string[]));
                contextNames.Length.should_be(2);
                contextNames[0].should_be("Test");
                contextNames[1].should_be("Test2");
            };

            it["sets first context as default when component has no context"] = () => {
                var contextNames = getData<NoContextComponent>().GetContextNames();
                contextNames.Length.should_be(1);
                contextNames[0].should_be("Game");
            };

            it["gets unique"] = () => {
                data.IsUnique().GetType().should_be(typeof(bool));
                data.IsUnique().should_be_false();

                getData<UniqueStandardComponent>().IsUnique().should_be_true();
            };

            it["gets member data"] = () => {
                data.GetMemberData().GetType().should_be(typeof(MemberData[]));
                data.GetMemberData().Length.should_be(1);
                data.GetMemberData()[0].type.should_be("string");
            };

            it["gets generate component"] = () => {
                data.ShouldGenerateComponent().GetType().should_be(typeof(bool));
                data.ShouldGenerateComponent().should_be_false();
                data.ContainsKey(ShouldGenerateComponentComponentDataExtension.COMPONENT_OBJECT_TYPE).should_be_false();
            };

            it["gets generate index"] = () => {
                data.ShouldGenerateIndex().GetType().should_be(typeof(bool));
                data.ShouldGenerateIndex().should_be_true();

                getData<DontGenerateIndexComponent>().ShouldGenerateIndex().should_be_false();
            };

            it["gets generate methods"] = () => {
                data.ShouldGenerateMethods().GetType().should_be(typeof(bool));
                data.ShouldGenerateMethods().should_be_true();

                getData<DontGenerateMethodsComponent>().ShouldGenerateMethods().should_be_false();
            };

            it["gets flag prefix"] = () => {
                data.GetFlagPrefix().GetType().should_be(typeof(string));
                data.GetFlagPrefix().should_be("is");

                getData<CustomPrefixFlagComponent>().GetFlagPrefix().should_be("My");
            };

            it["gets is no event"] = () => {
                data.IsEvent().GetType().should_be(typeof(bool));
                data.IsEvent().should_be_false();
            };

            it["gets event"] = () => { getData<StandardEventComponent>().IsEvent().should_be_true(); };

            it["gets multiple events"] = () => {
                var d = getData<MultipleEventsStandardEventComponent>();
                d.IsEvent().should_be_true();
                var eventData = d.GetEventData();
                eventData.Length.should_be(2);

                eventData[0].eventTarget.should_be(EventTarget.Any);
                eventData[0].eventType.should_be(EventType.Added);
                eventData[0].priority.should_be(1);

                eventData[1].eventTarget.should_be(EventTarget.Self);
                eventData[1].eventType.should_be(EventType.Removed);
                eventData[1].priority.should_be(2);
            };

            it["gets event target"] = () => {
                getData<StandardEventComponent>().GetEventData()[0].eventTarget.GetType().should_be(typeof(EventTarget));
                getData<StandardEventComponent>().GetEventData()[0].eventTarget.should_be(EventTarget.Any);
                getData<StandardEntityEventComponent>().GetEventData()[0].eventTarget.should_be(EventTarget.Self);
            };

            it["gets event type"] = () => {
                getData<StandardEventComponent>().GetEventData()[0].eventType.GetType().should_be(typeof(EventType));
                getData<StandardEventComponent>().GetEventData()[0].eventType.should_be(EventType.Added);
                getData<StandardEntityEventComponent>().GetEventData()[0].eventType.should_be(EventType.Removed);
            };

            it["gets event priority"] = () => {
                getData<StandardEventComponent>().GetEventData()[0].priority.GetType().should_be(typeof(int));
                getData<StandardEntityEventComponent>().GetEventData()[0].priority.should_be(1);
            };

            it["creates data for event listeners"] = () => {
                var d = getMultipleData<StandardEventComponent>();
                d.Length.should_be(2);
                d[1].IsEvent().should_be_false();
                d[1].GetTypeName().should_be("AnyStandardEventListenerComponent");
                d[1].GetMemberData().Length.should_be(1);
                d[1].GetMemberData()[0].name.should_be("value");
                d[1].GetMemberData()[0].type.should_be("System.Collections.Generic.List<IAnyStandardEventListener>");
            };

            it["creates data for unique event listeners"] = () => {
                var d = getMultipleData<UniqueEventComponent>();
                d.Length.should_be(2);
                d[1].IsEvent().should_be_false();
                d[1].IsUnique().should_be_false();
            };

            it["creates data for event listeners with multiple contexts"] = () => {
                var d = getMultipleData<MultipleContextStandardEventComponent>();
                d.Length.should_be(3);
                d[1].IsEvent().should_be_false();
                d[1].GetTypeName().should_be("TestAnyMultipleContextStandardEventListenerComponent");
                d[1].GetMemberData().Length.should_be(1);
                d[1].GetMemberData()[0].name.should_be("value");
                d[1].GetMemberData()[0].type.should_be("System.Collections.Generic.List<ITestAnyMultipleContextStandardEventListener>");

                d[2].IsEvent().should_be_false();
                d[2].GetTypeName().should_be("Test2AnyMultipleContextStandardEventListenerComponent");
                d[2].GetMemberData().Length.should_be(1);
                d[2].GetMemberData()[0].name.should_be("value");
                d[2].GetMemberData()[0].type.should_be("System.Collections.Generic.List<ITest2AnyMultipleContextStandardEventListener>");
            };
        };

        context["non component"] = () => {

            Type type = null;
            ComponentData data = null;

            before = () => {
                type = typeof(ClassToGenerate);
                data = getData<ClassToGenerate>();
            };

            it["get data"] = () => { data.should_not_be_null(); };

            it["gets full type name"] = () => {
                // Not the type, but the component that should be generated
                // See: no namespace
                data.GetTypeName().should_be("ClassToGenerateComponent");
            };

            it["gets contexts"] = () => {
                var contextNames = data.GetContextNames();
                contextNames.Length.should_be(2);
                contextNames[0].should_be("Test");
                contextNames[1].should_be("Test2");
            };

            it["gets unique"] = () => { data.IsUnique().should_be_false(); };

            it["gets member data"] = () => {
                data.GetMemberData().Length.should_be(1);
                data.GetMemberData()[0].type.should_be(type.ToCompilableString());
            };

            it["gets generate component"] = () => {
                data.ShouldGenerateComponent().GetType().should_be(typeof(bool));
                data.ShouldGenerateComponent().should_be_true();
                data.GetObjectTypeName().should_be(typeof(ClassToGenerate).ToCompilableString());
            };

            it["gets generate index"] = () => { data.ShouldGenerateIndex().should_be_true(); };

            it["gets generate methods"] = () => { data.ShouldGenerateMethods().should_be_true(); };

            it["gets flag prefix"] = () => { data.GetFlagPrefix().should_be("is"); };

            it["gets is no event"] = () => { data.IsEvent().should_be_false(); };

            it["gets event"] = () => {
                getData<EventToGenerate>().GetEventData().Length.should_be(1);
                var eventData = getData<EventToGenerate>().GetEventData()[0];
                eventData.eventTarget.should_be(EventTarget.Any);
                eventData.eventType.should_be(EventType.Added);
                eventData.priority.should_be(0);
            };

            it["creates data for event listeners"] = () => {
                var d = getMultipleData<EventToGenerate>();
                d.Length.should_be(3);
                d[1].IsEvent().should_be_false();
                d[1].ShouldGenerateComponent().should_be_false();
                d[1].GetTypeName().should_be("TestAnyEventToGenerateListenerComponent");
                d[1].GetMemberData().Length.should_be(1);
                d[1].GetMemberData()[0].name.should_be("value");
                d[1].GetMemberData()[0].type.should_be("System.Collections.Generic.List<ITestAnyEventToGenerateListener>");

                d[2].IsEvent().should_be_false();
                d[2].ShouldGenerateComponent().should_be_false();
                d[2].GetTypeName().should_be("Test2AnyEventToGenerateListenerComponent");
                d[2].GetMemberData().Length.should_be(1);
                d[2].GetMemberData()[0].name.should_be("value");
                d[2].GetMemberData()[0].type.should_be("System.Collections.Generic.List<ITest2AnyEventToGenerateListener>");
            };
        };

        context["multiple types"] = () => {

            it["creates data for each type"] = () => {
                var types = new[] { typeof(NameAgeComponent), typeof(Test2ContextComponent) };
                var provider = new ComponentDataProvider(types);
                provider.Configure(new TestPreferences(
                    "Entitas.CodeGeneration.Plugins.Contexts = Game, GameState"
                ));
                var data = provider.GetData();
                data.Length.should_be(types.Length);
            };

            it["ignores duplicates from non components"] = () => {
                var types = new[] { typeof(ClassToGenerate), typeof(ClassToGenerateComponent) };
                var provider = new ComponentDataProvider(types);
                provider.Configure(new TestPreferences(
                    "Entitas.CodeGeneration.Plugins.Contexts = Game, GameState"
                ));
                var data = provider.GetData();
                data.Length.should_be(1);
            };
        };

        context["multiple custom component names"] = () => {

            Type type = null;
            ComponentData data1 = null;
            ComponentData data2 = null;

            before = () => {
                type = typeof(CustomName);
                var data = getMultipleData<CustomName>();
                data1 = data[0];
                data2 = data[1];
            };

            it["creates data for each custom component name"] = () => {
                data1.GetObjectTypeName().should_be(type.ToCompilableString());
                data2.GetObjectTypeName().should_be(type.ToCompilableString());

                data1.GetTypeName().should_be("NewCustomNameComponent1Component");
                data2.GetTypeName().should_be("NewCustomNameComponent2Component");
            };
        };

        context["configure"] = () => {

            Type type = null;
            ComponentData data = null;

            before = () => {
                var preferences = new TestPreferences(
                    "Entitas.CodeGeneration.Plugins.Contexts = ConfiguredContext" + "\n"
                );

                type = typeof(NoContextComponent);
                data = getData<NoContextComponent>(preferences);
            };

            it["gets default context"] = () => {
                var contextNames = data.GetContextNames();
                contextNames.Length.should_be(1);
                contextNames[0].should_be("ConfiguredContext");
            };
        };
    }
}
