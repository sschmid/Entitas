using System;
using System.Collections.Generic;
using Entitas.CodeGenerator;
using Entitas.Utils;
using NSpec;
using System.Linq;
using My.Namespace;

class describe_ComponentDataProvider : nspec {

    void when_providing() {

        context["component"] = () => {

            Type[] types = null;
            ComponentData[] data = null;
            ComponentData d = null;

            before = () => {
                types = new[] { typeof(NameAgeComponent) };
                var provider = new ComponentDataProvider(types);
                data = (ComponentData[])provider.GetData();
                d = data[0];
            };

            it["get data"] = () => {
                data.Length.should_be(1);
            };

            it["gets full type name"] = () => {
                d.GetFullTypeName().GetType().should_be(typeof(string));
                d.GetFullTypeName().should_be(types[0].FullName);
            };

            it["gets member infos"] = () => {
                d.GetMemberInfos().GetType().should_be(typeof(List<PublicMemberInfo>));
                d.GetMemberInfos().Count.should_be(2);
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

                var provider = new ComponentDataProvider(new Type[] { typeof(UniqueStandardComponent) });
                data = (ComponentData[])provider.GetData();
                data[0].IsUnique().should_be_true();
            };

            it["gets unique prefix"] = () => {
                d.GetUniqueComponentPrefix().GetType().should_be(typeof(string));
                d.GetUniqueComponentPrefix().should_be("is");

                var provider = new ComponentDataProvider(new Type[] { typeof(CustomPrefixFlagComponent) });
                data = (ComponentData[])provider.GetData();
                data[0].GetUniqueComponentPrefix().should_be("My");
            };

            it["gets isComponent"] = () => {
                d.ShouldGenerateComponent().GetType().should_be(typeof(bool));
                d.ShouldGenerateComponent().should_be_false();

                var provider = new ComponentDataProvider(new Type[] { typeof(ClassToGenerate) });
                data = (ComponentData[])provider.GetData();
                data[0].ShouldGenerateComponent().should_be_true();
            };

            it["gets generate methods"] = () => {
                d.ShouldGenerateMethods().GetType().should_be(typeof(bool));
                d.ShouldGenerateMethods().should_be_true();

                var provider = new ComponentDataProvider(new Type[] { typeof(DontGenerateMethodsComponent) });
                data = (ComponentData[])provider.GetData();
                data[0].ShouldGenerateMethods().should_be_false();
            };

            it["gets generate index"] = () => {
                d.ShouldGenerateIndex().GetType().should_be(typeof(bool));
                d.ShouldGenerateIndex().should_be_true();

                var provider = new ComponentDataProvider(new Type[] { typeof(DontGenerateIndexComponent) });
                data = (ComponentData[])provider.GetData();
                data[0].ShouldGenerateIndex().should_be_false();
            };

            it["gets hide in blueprints inspector"] = () => {
                d.ShouldHideInBlueprintInspector().GetType().should_be(typeof(bool));
                d.ShouldHideInBlueprintInspector().should_be_false();

                var provider = new ComponentDataProvider(new Type[] { typeof(HideInBlueprintInspectorComponent) });
                data = (ComponentData[])provider.GetData();
                data[0].ShouldHideInBlueprintInspector().should_be_true();
            };
        };

        context["non component"] = () => {

            Type[] types = null;
            ComponentData[] data = null;
            ComponentData d = null;

            before = () => {
                types = new[] { typeof(ClassToGenerate) };
                var provider = new ComponentDataProvider(types);
                data = (ComponentData[])provider.GetData();
                d = data[0];
            };

            it["get data"] = () => {
                data.Length.should_be(1);
            };

            it["gets full type name"] = () => {
                d.GetFullTypeName().GetType().should_be(typeof(string));
                d.GetFullTypeName().should_be(types[0].FullName.AddComponentSuffix());
            };

            it["gets member infos"] = () => {
                d.GetMemberInfos().GetType().should_be(typeof(List<PublicMemberInfo>));
                d.GetMemberInfos().Count.should_be(1);
            };

            it["gets contexts"] = () => {
                d.GetContextNames().GetType().should_be(typeof(string[]));
                d.GetContextNames().Length.should_be(2);
                d.GetContextNames()[0].should_be("SomeContext");
                d.GetContextNames()[1].should_be("SomeOtherContext");
            };

            it["gets unique"] = () => {
                d.IsUnique().GetType().should_be(typeof(bool));
                d.IsUnique().should_be_false();
            };

            it["gets unique prefix"] = () => {
                d.GetUniqueComponentPrefix().GetType().should_be(typeof(string));
                d.GetUniqueComponentPrefix().should_be("is");
            };

            it["gets isComponent"] = () => {
                d.ShouldGenerateComponent().GetType().should_be(typeof(bool));
                d.ShouldGenerateComponent().should_be_true();
            };

            it["gets generate methods"] = () => {
                d.ShouldGenerateMethods().GetType().should_be(typeof(bool));
                d.ShouldGenerateMethods().should_be_true();
            };

            it["gets generate index"] = () => {
                d.ShouldGenerateIndex().GetType().should_be(typeof(bool));
                d.ShouldGenerateIndex().should_be_true();
            };

            it["gets hide in blueprints inspector"] = () => {
                d.ShouldHideInBlueprintInspector().GetType().should_be(typeof(bool));
                d.ShouldHideInBlueprintInspector().should_be_false();

                var provider = new ComponentDataProvider(new Type[] { typeof(ClassHideInBlueprintsInspector) });
                data = (ComponentData[])provider.GetData();
                data[0].ShouldHideInBlueprintInspector().should_be_true();
            };
        };

        context["multiple types"] = () => {

            it["creates data for each type"] = () => {
                var types = new[] { typeof(NameAgeComponent), typeof(Test2ContextComponent) };
                var provider = new ComponentDataProvider(types);
                var data = provider.GetData();
                data.Length.should_be(types.Length);
            };
        };
    }
}
