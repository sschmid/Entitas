using System;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;
using Entitas.CodeGeneration.Plugins;
using My.Namespace;
using NSpec;
using Shouldly;

class describe_ComponentDataProvider : nspec
{
    ComponentData getData<T>(Preferences preferences = null)
    {
        return getMultipleData<T>(preferences)[0];
    }

    ComponentData[] getMultipleData<T>(Preferences preferences = null)
    {
        var provider = new ComponentDataProvider(new Type[] {typeof(T)});
        if (preferences == null)
        {
            preferences = new TestPreferences("Entitas.CodeGeneration.Plugins.Contexts = Game, GameState");
        }

        provider.Configure(preferences);

        return (ComponentData[])provider.GetData();
    }

    void when_providing()
    {
        context["component"] = () =>
        {
            Type type = null;
            ComponentData data = null;

            before = () =>
            {
                type = typeof(MyNamespaceComponent);
                data = getData<MyNamespaceComponent>();
            };

            it["get data"] = () => { data.ShouldNotBeNull(); };

            it["gets full type name"] = () =>
            {
                data.GetTypeName().GetType().ShouldBe(typeof(string));
                data.GetTypeName().ShouldBe("My.Namespace.MyNamespaceComponent");
            };

            it["gets namespace"] = () =>
            {
                data.GetNamespace().GetType().ShouldBe(typeof(string));
                data.GetNamespace().ShouldBe("My.Namespace");
            };

            it["gets component name"] = () =>
            {
                data.GetComponentName().GetType().ShouldBe(typeof(string));
                data.GetComponentName().ShouldBe("MyNamespace");
            };

            it["gets contexts"] = () =>
            {
                var contextNames = data.GetContextNames();
                contextNames.GetType().ShouldBe(typeof(string[]));
                contextNames.Length.ShouldBe(2);
                contextNames[0].ShouldBe("Test");
                contextNames[1].ShouldBe("Test2");
            };

            it["sets first context as default when component has no context"] = () =>
            {
                var contextNames = getData<NoContextComponent>().GetContextNames();
                contextNames.Length.ShouldBe(1);
                contextNames[0].ShouldBe("Game");
            };

            it["gets unique"] = () =>
            {
                data.IsUnique().GetType().ShouldBe(typeof(bool));
                data.IsUnique().ShouldBeFalse();

                getData<UniqueStandardComponent>().IsUnique().ShouldBeTrue();
            };

            it["gets member data"] = () =>
            {
                data.GetMemberData().GetType().ShouldBe(typeof(MemberData[]));
                data.GetMemberData().Length.ShouldBe(1);
                data.GetMemberData()[0].type.ShouldBe("string");
            };

            it["gets generate component"] = () =>
            {
                data.ShouldGenerateComponent().GetType().ShouldBe(typeof(bool));
                data.ShouldGenerateComponent().ShouldBeFalse();
                data.ContainsKey(ShouldGenerateComponentComponentDataExtension.COMPONENT_OBJECT_TYPE).ShouldBeFalse();
            };

            it["gets generate index"] = () =>
            {
                data.ShouldGenerateIndex().GetType().ShouldBe(typeof(bool));
                data.ShouldGenerateIndex().ShouldBeTrue();

                getData<DontGenerateIndexComponent>().ShouldGenerateIndex().ShouldBeFalse();
            };

            it["gets generate methods"] = () =>
            {
                data.ShouldGenerateMethods().GetType().ShouldBe(typeof(bool));
                data.ShouldGenerateMethods().ShouldBeTrue();

                getData<DontGenerateMethodsComponent>().ShouldGenerateMethods().ShouldBeFalse();
            };

            it["gets flag prefix"] = () =>
            {
                data.GetFlagPrefix().GetType().ShouldBe(typeof(string));
                data.GetFlagPrefix().ShouldBe("is");

                getData<CustomPrefixFlagComponent>().GetFlagPrefix().ShouldBe("My");
            };

            it["gets is no event"] = () =>
            {
                data.IsEvent().GetType().ShouldBe(typeof(bool));
                data.IsEvent().ShouldBeFalse();
            };

            it["gets event"] = () => { getData<StandardEventComponent>().IsEvent().ShouldBeTrue(); };

            it["gets multiple events"] = () =>
            {
                var d = getData<MultipleEventsStandardEventComponent>();
                d.IsEvent().ShouldBeTrue();
                var eventData = d.GetEventData();
                eventData.Length.ShouldBe(2);

                eventData[0].eventTarget.ShouldBe(EventTarget.Any);
                eventData[0].eventType.ShouldBe(EventType.Added);
                eventData[0].priority.ShouldBe(1);

                eventData[1].eventTarget.ShouldBe(EventTarget.Self);
                eventData[1].eventType.ShouldBe(EventType.Removed);
                eventData[1].priority.ShouldBe(2);
            };

            it["gets event target"] = () =>
            {
                getData<StandardEventComponent>().GetEventData()[0].eventTarget.GetType().ShouldBe(typeof(EventTarget));
                getData<StandardEventComponent>().GetEventData()[0].eventTarget.ShouldBe(EventTarget.Any);
                getData<StandardEntityEventComponent>().GetEventData()[0].eventTarget.ShouldBe(EventTarget.Self);
            };

            it["gets event type"] = () =>
            {
                getData<StandardEventComponent>().GetEventData()[0].eventType.GetType().ShouldBe(typeof(EventType));
                getData<StandardEventComponent>().GetEventData()[0].eventType.ShouldBe(EventType.Added);
                getData<StandardEntityEventComponent>().GetEventData()[0].eventType.ShouldBe(EventType.Removed);
            };

            it["gets event priority"] = () =>
            {
                getData<StandardEventComponent>().GetEventData()[0].priority.GetType().ShouldBe(typeof(int));
                getData<StandardEntityEventComponent>().GetEventData()[0].priority.ShouldBe(1);
            };

            it["creates data for event listeners"] = () =>
            {
                var d = getMultipleData<StandardEventComponent>();
                d.Length.ShouldBe(2);
                d[1].IsEvent().ShouldBeFalse();
                d[1].GetTypeName().ShouldBe("AnyStandardEventListenerComponent");
                d[1].GetMemberData().Length.ShouldBe(1);
                d[1].GetMemberData()[0].name.ShouldBe("Value");
                d[1].GetMemberData()[0].type.ShouldBe("System.Collections.Generic.List<IAnyStandardEventListener>");
            };

            it["creates data for unique event listeners"] = () =>
            {
                var d = getMultipleData<UniqueEventComponent>();
                d.Length.ShouldBe(2);
                d[1].IsEvent().ShouldBeFalse();
                d[1].IsUnique().ShouldBeFalse();
            };

            it["creates data for event listeners with multiple contexts"] = () =>
            {
                var d = getMultipleData<MultipleContextStandardEventComponent>();
                d.Length.ShouldBe(3);
                d[1].IsEvent().ShouldBeFalse();
                d[1].GetTypeName().ShouldBe("AnyMultipleContextStandardEventListenerComponent");
                d[1].GetMemberData().Length.ShouldBe(1);
                d[1].GetMemberData()[0].name.ShouldBe("Value");
                d[1].GetMemberData()[0].type.ShouldBe("System.Collections.Generic.List<IAnyMultipleContextStandardEventListener>");

                d[2].IsEvent().ShouldBeFalse();
                d[2].GetTypeName().ShouldBe("AnyMultipleContextStandardEventListenerComponent");
                d[2].GetMemberData().Length.ShouldBe(1);
                d[2].GetMemberData()[0].name.ShouldBe("Value");
                d[2].GetMemberData()[0].type.ShouldBe("System.Collections.Generic.List<IAnyMultipleContextStandardEventListener>");
            };
        };

        xcontext["non component"] = () =>
        {
            Type type = null;
            ComponentData data = null;

            before = () =>
            {
                type = typeof(ClassToGenerate);
                data = getData<ClassToGenerate>();
            };

            it["get data"] = () => { data.ShouldNotBeNull(); };

            it["gets full type name"] = () =>
            {
                // Not the type, but the component that should be generated
                // See: no namespace
                data.GetTypeName().ShouldBe("ClassToGenerateComponent");
            };

            it["gets contexts"] = () =>
            {
                var contextNames = data.GetContextNames();
                contextNames.Length.ShouldBe(2);
                contextNames[0].ShouldBe("Test");
                contextNames[1].ShouldBe("Test2");
            };

            it["gets unique"] = () => { data.IsUnique().ShouldBeFalse(); };

            it["gets member data"] = () =>
            {
                data.GetMemberData().Length.ShouldBe(1);
                data.GetMemberData()[0].type.ShouldBe(type.ToCompilableString());
            };

            it["gets generate component"] = () =>
            {
                data.ShouldGenerateComponent().GetType().ShouldBe(typeof(bool));
                data.ShouldGenerateComponent().ShouldBeTrue();
                data.GetObjectTypeName().ShouldBe(typeof(ClassToGenerate).ToCompilableString());
            };

            it["gets generate index"] = () => { data.ShouldGenerateIndex().ShouldBeTrue(); };

            it["gets generate methods"] = () => { data.ShouldGenerateMethods().ShouldBeTrue(); };

            it["gets flag prefix"] = () => { data.GetFlagPrefix().ShouldBe("is"); };

            it["gets is no event"] = () => { data.IsEvent().ShouldBeFalse(); };

            it["gets event"] = () =>
            {
                getData<EventToGenerate>().GetEventData().Length.ShouldBe(1);
                var eventData = getData<EventToGenerate>().GetEventData()[0];
                eventData.eventTarget.ShouldBe(EventTarget.Any);
                eventData.eventType.ShouldBe(EventType.Added);
                eventData.priority.ShouldBe(0);
            };

            it["creates data for event listeners"] = () =>
            {
                var d = getMultipleData<EventToGenerate>();
                d.Length.ShouldBe(3);
                d[1].IsEvent().ShouldBeFalse();
                d[1].ShouldGenerateComponent().ShouldBeFalse();
                d[1].GetTypeName().ShouldBe("TestAnyEventToGenerateListenerComponent");
                d[1].GetMemberData().Length.ShouldBe(1);
                d[1].GetMemberData()[0].name.ShouldBe("value");
                d[1].GetMemberData()[0].type.ShouldBe("System.Collections.Generic.List<ITestAnyEventToGenerateListener>");

                d[2].IsEvent().ShouldBeFalse();
                d[2].ShouldGenerateComponent().ShouldBeFalse();
                d[2].GetTypeName().ShouldBe("Test2AnyEventToGenerateListenerComponent");
                d[2].GetMemberData().Length.ShouldBe(1);
                d[2].GetMemberData()[0].name.ShouldBe("value");
                d[2].GetMemberData()[0].type.ShouldBe("System.Collections.Generic.List<ITest2AnyEventToGenerateListener>");
            };
        };

        context["multiple types"] = () =>
        {
            it["creates data for each type"] = () =>
            {
                var types = new[] {typeof(NameAgeComponent), typeof(Test2ContextComponent)};
                var provider = new ComponentDataProvider(types);
                provider.Configure(new TestPreferences(
                    "Entitas.CodeGeneration.Plugins.Contexts = Game, GameState"
                ));
                var data = provider.GetData();
                data.Length.ShouldBe(types.Length);
            };

            it["ignores duplicates from non components"] = () =>
            {
                var types = new[] {typeof(ClassToGenerate), typeof(ClassToGenerateComponent)};
                var provider = new ComponentDataProvider(types);
                provider.Configure(new TestPreferences(
                    "Entitas.CodeGeneration.Plugins.Contexts = Game, GameState"
                ));
                var data = provider.GetData();
                data.Length.ShouldBe(1);
            };
        };

        xcontext["multiple custom component names"] = () =>
        {
            Type type = null;
            ComponentData data1 = null;
            ComponentData data2 = null;

            before = () =>
            {
                type = typeof(CustomName);
                var data = getMultipleData<CustomName>();
                data1 = data[0];
                data2 = data[1];
            };

            it["creates data for each custom component name"] = () =>
            {
                data1.GetObjectTypeName().ShouldBe(type.ToCompilableString());
                data2.GetObjectTypeName().ShouldBe(type.ToCompilableString());

                data1.GetTypeName().ShouldBe("NewCustomNameComponent1Component");
                data2.GetTypeName().ShouldBe("NewCustomNameComponent2Component");
            };
        };

        context["configure"] = () =>
        {
            Type type = null;
            ComponentData data = null;

            before = () =>
            {
                var preferences = new TestPreferences(
                    "Entitas.CodeGeneration.Plugins.Contexts = ConfiguredContext" + "\n"
                );

                type = typeof(NoContextComponent);
                data = getData<NoContextComponent>(preferences);
            };

            it["gets default context"] = () =>
            {
                var contextNames = data.GetContextNames();
                contextNames.Length.ShouldBe(1);
                contextNames[0].ShouldBe("ConfiguredContext");
            };
        };
    }
}
