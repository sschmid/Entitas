using System;
using Entitas.CodeGenerator;
using NSpec;
using My.Namespace;
using Entitas;

class describe_ComponentDataProvider : nspec {

    ComponentData getData<T>() {
        var provider = new ComponentDataProvider(new Type[] { typeof(T) });
        return (ComponentData)provider.GetData()[0];
    }

    void when_providing() {

        context["component"] = () => {

            Type[] types = null;
            ComponentData[] data = null;
            ComponentData d = null;

            before = () => {
                types = new [] { typeof(MyNamespaceComponent) };
                var provider = new ComponentDataProvider(types);
                data = (ComponentData[])provider.GetData();
                d = data[0];
            };

            it["get data"] = () => {
                data.Length.should_be(1);
            };

            it["gets component name"] = () => {
                d.GetComponentName().GetType().should_be(typeof(string));
                d.GetComponentName().should_be("MyNamespace");

                d.GetFullComponentName().GetType().should_be(typeof(string));
                d.GetFullComponentName().should_be("MyNamespaceComponent");
            };

            it["gets full type name"] = () => {
                d.GetFullTypeName().GetType().should_be(typeof(string));
                d.GetFullTypeName().should_be(types[0].ToCompilableString());
            };

            it["gets contexts"] = () => {
                d.GetContextNames().GetType().should_be(typeof(string[]));
                d.GetContextNames().Length.should_be(2);
                d.GetContextNames()[0].should_be("Test");
                d.GetContextNames()[1].should_be("Test2");
            };

            it["sets first context as default when component has no context"] = () => {
                var contextNames = getData<NoContextComponent>().GetContextNames();
                contextNames.Length.should_be(1);
                contextNames[0].should_be("Game");
            };

            it["gets unique"] = () => {
                d.IsUnique().GetType().should_be(typeof(bool));
                d.IsUnique().should_be_false();

                getData<UniqueStandardComponent>().IsUnique().should_be_true();
            };

            it["gets member data"] = () => {
                d.GetMemberData().GetType().should_be(typeof(MemberData[]));
                d.GetMemberData().Length.should_be(1);
            };

            it["gets generate component"] = () => {
                d.ShouldGenerateComponent().GetType().should_be(typeof(bool));
                d.ShouldGenerateComponent().should_be_false();
                d.ContainsKey(ShouldGenerateComponentComponentDataExtension.COMPONENT_OBJECT_TYPE).should_be_false();
            };

            it["gets generate index"] = () => {
                d.ShouldGenerateIndex().GetType().should_be(typeof(bool));
                d.ShouldGenerateIndex().should_be_true();

                getData<DontGenerateIndexComponent>().ShouldGenerateIndex().should_be_false();
            };

            it["gets generate methods"] = () => {
                d.ShouldGenerateMethods().GetType().should_be(typeof(bool));
                d.ShouldGenerateMethods().should_be_true();

                getData<DontGenerateMethodsComponent>().ShouldGenerateMethods().should_be_false();
            };

            it["gets unique prefix"] = () => {
                d.GetUniqueComponentPrefix().GetType().should_be(typeof(string));
                d.GetUniqueComponentPrefix().should_be("is");

                getData<CustomPrefixFlagComponent>().GetUniqueComponentPrefix().should_be("My");
            };
        };

        context["non component"] = () => {

            Type[] types = null;
            ComponentData[] data = null;
            ComponentData d = null;

            before = () => {
                types = new [] { typeof(ClassToGenerate) };
                var provider = new ComponentDataProvider(types);
                data = (ComponentData[])provider.GetData();
                d = data[0];
            };

            it["get data"] = () => {
                data.Length.should_be(1);
            };

            it["gets component name"] = () => {
                d.GetComponentName().GetType().should_be(typeof(string));

                // Not the type, but the component that should be generated
                // See: no namespace
                d.GetComponentName().should_be("ClassToGenerate");

                d.GetFullComponentName().GetType().should_be(typeof(string));

                // Not the type, but the component that should be generated
                // See: no namespace
                d.GetFullComponentName().should_be("ClassToGenerateComponent");
            };

            it["gets full type name"] = () => {
                d.GetFullTypeName().GetType().should_be(typeof(string));

                // Not the type, but the component that should be generated
                // See: no namespace
                d.GetFullTypeName().should_be("ClassToGenerateComponent");
            };

            it["gets contexts"] = () => {
                d.GetContextNames().GetType().should_be(typeof(string[]));
                d.GetContextNames().Length.should_be(2);
                d.GetContextNames()[0].should_be("Test");
                d.GetContextNames()[1].should_be("Test2");
            };

            it["gets unique"] = () => {
                d.IsUnique().GetType().should_be(typeof(bool));
                d.IsUnique().should_be_false();
            };

            it["gets member data"] = () => {
                d.GetMemberData().Length.should_be(1);
                d.GetMemberData()[0].type.should_be(typeof(ClassToGenerate).ToCompilableString());
            };

            it["gets generate component"] = () => {
                d.ShouldGenerateComponent().GetType().should_be(typeof(bool));
                d.ShouldGenerateComponent().should_be_true();
                d.GetObjectType().should_be(typeof(ClassToGenerate).ToCompilableString());
            };

            it["gets generate index"] = () => {
                d.ShouldGenerateIndex().GetType().should_be(typeof(bool));
                d.ShouldGenerateIndex().should_be_true();
            };

            it["gets generate methods"] = () => {
                d.ShouldGenerateMethods().GetType().should_be(typeof(bool));
                d.ShouldGenerateMethods().should_be_true();
            };

            it["gets unique prefix"] = () => {
                d.GetUniqueComponentPrefix().GetType().should_be(typeof(string));
                d.GetUniqueComponentPrefix().should_be("is");
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

            Type[] types = null;
            ComponentData[] data = null;
            ComponentData d1 = null;
            ComponentData d2 = null;

            before = () => {
                types = new [] { typeof(CustomName) };
                var provider = new ComponentDataProvider(types);
                data = (ComponentData[])provider.GetData();
                d1 = data[0];
                d2 = data[1];
            };

            it["get data"] = () => {
                data.Length.should_be(2);
            };

            it["creates data for each custom component name"] = () => {
                d1.GetComponentName().should_be("NewCustomNameComponent1");
                d2.GetComponentName().should_be("NewCustomNameComponent2");

                d1.GetObjectType().should_be(types[0].ToCompilableString());
                d2.GetObjectType().should_be(types[0].ToCompilableString());

                d1.GetFullTypeName().should_be("NewCustomNameComponent1Component");
                d2.GetFullTypeName().should_be("NewCustomNameComponent2Component");

                d1.GetComponentName().should_be("NewCustomNameComponent1");
                d2.GetComponentName().should_be("NewCustomNameComponent2");

                d1.GetFullComponentName().should_be("NewCustomNameComponent1Component");
                d2.GetFullComponentName().should_be("NewCustomNameComponent2Component");
            };
        };
    }
}
