using System;
using Entitas.CodeGeneration.Plugins;
using Entitas.Utils;
using My.Namespace;
using NSpec;

class describe_ComponentDataProvider : nspec {

    ComponentData getData<T>() {
        return getMultipleData<T>()[0];
    }

    ComponentData[] getMultipleData<T>() {
        var provider = new ComponentDataProvider(new Type[] { typeof(T) });
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

            it["get data"] = () => {
                data.should_not_be_null();
            };

            it["gets full type name"] = () => {
                data.GetFullTypeName().GetType().should_be(typeof(string));
                data.GetFullTypeName().should_be(type.ToCompilableString());
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

            it["gets unique prefix"] = () => {
                data.GetUniqueComponentPrefix().GetType().should_be(typeof(string));
                data.GetUniqueComponentPrefix().should_be("is");

                getData<CustomPrefixFlagComponent>().GetUniqueComponentPrefix().should_be("My");
            };
        };

        context["non component"] = () => {

            Type type = null;
            ComponentData data = null;

            before = () => {
                type = typeof(ClassToGenerate);
                data = getData<ClassToGenerate>();
            };

            it["get data"] = () => {
                data.should_not_be_null();
            };

            it["gets full type name"] = () => {
                // Not the type, but the component that should be generated
                // See: no namespace
                data.GetFullTypeName().should_be("ClassToGenerateComponent");
            };

            it["gets contexts"] = () => {
                var contextNames = data.GetContextNames();
                contextNames.Length.should_be(2);
                contextNames[0].should_be("Test");
                contextNames[1].should_be("Test2");
            };

            it["gets unique"] = () => {
                data.IsUnique().should_be_false();
            };

            it["gets member data"] = () => {
                data.GetMemberData().Length.should_be(1);
                data.GetMemberData()[0].type.should_be(type.ToCompilableString());
            };

            it["gets generate component"] = () => {
                data.ShouldGenerateComponent().GetType().should_be(typeof(bool));
                data.ShouldGenerateComponent().should_be_true();
                data.GetObjectType().should_be(typeof(ClassToGenerate).ToCompilableString());
            };

            it["gets generate index"] = () => {
                data.ShouldGenerateIndex().should_be_true();
            };

            it["gets generate methods"] = () => {
                data.ShouldGenerateMethods().should_be_true();
            };

            it["gets unique prefix"] = () => {
                data.GetUniqueComponentPrefix().should_be("is");
            };
        };

        context["multiple types"] = () => {

            it["creates data for each type"] = () => {
                var types = new [] { typeof(NameAgeComponent), typeof(Test2ContextComponent) };
                var provider = new ComponentDataProvider(types);
                var data = provider.GetData();
                data.Length.should_be(types.Length);
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
                data1.GetObjectType().should_be(type.ToCompilableString());
                data2.GetObjectType().should_be(type.ToCompilableString());

                data1.GetFullTypeName().should_be("NewCustomNameComponent1Component");
                data2.GetFullTypeName().should_be("NewCustomNameComponent2Component");
            };
        };
    }
}
