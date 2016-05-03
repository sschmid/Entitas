using System;
using Entitas;
using Entitas.CodeGenerator;
using My.Namespace;
using NSpec;

class describe_TypeReflectionProvider : nspec {

    TypeReflectionProvider createProviderWithTypes(params Type[] types) {
        return new TypeReflectionProvider(types, new string[0], new string[0]);
    }

    TypeReflectionProvider createProviderWithPoolName(params string[] poolNames) {
        return new TypeReflectionProvider(new [] { typeof(object) }, poolNames, new string[0]);
    }

    TypeReflectionProvider createProviderWithBlueprintNames(params string[] blueprintNames) {
        return new TypeReflectionProvider(new [] { typeof(object) }, new string[0], blueprintNames);
    }

    void when_providing() {

        context["pool names"] = () => {
            it["has no pool names if empty"] = () => {
                var provider = createProviderWithPoolName();
                provider.poolNames.should_be_empty();
            };

            it["has pool names if set"] = () => {
                var provider = createProviderWithPoolName("Pool1", "Pool2");
                provider.poolNames.Length.should_be(2);
                provider.poolNames[0].should_be("Pool1");
                provider.poolNames[1].should_be("Pool2");
            };
        };

        context["component infos"] = () => {

            context["when type doesn't implement IComponent"] = () => {
                it["creates a component for a class"] = () => {
                    var provider = createProviderWithTypes(typeof(SomeClass));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("SomeClassComponent");
                    info.typeName.should_be("SomeClassComponent");

                    info.memberInfos.Count.should_be(1);

                    info.memberInfos[0].type.should_be(typeof(SomeClass));
                    info.memberInfos[0].name.should_be("value");

                    info.pools.should_contain("SomePool");
                    info.isSingleEntity.should_be_false();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(true);
                    info.generateMethods.should_be(true);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(false);
                };

                it["creates a component for a class in namespace"] = () => {
                    var provider = createProviderWithTypes(typeof(SomeNamespace.SomeOtherClass));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("SomeOtherClassComponent");
                    info.typeName.should_be("SomeOtherClassComponent");

                    info.memberInfos.Count.should_be(1);

                    info.memberInfos[0].type.should_be(typeof(SomeNamespace.SomeOtherClass));
                    info.memberInfos[0].name.should_be("value");

                    info.pools.should_contain("SomePool");
                    info.isSingleEntity.should_be_false();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(true);
                    info.generateMethods.should_be(true);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(false);
                };

                it["doesn't create a component for a generic class"] = () => {
                    var provider = createProviderWithTypes(typeof(SomeGenericClass<int>));
                    provider.componentInfos.should_be_empty();
                };

                it["creates a component for an interface"] = () => {
                    var provider = createProviderWithTypes(typeof(ISomeInterface));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("ISomeInterfaceComponent");
                    info.typeName.should_be("ISomeInterfaceComponent");

                    info.memberInfos.Count.should_be(1);

                    info.memberInfos[0].type.should_be(typeof(ISomeInterface));
                    info.memberInfos[0].name.should_be("value");

                    info.pools.should_contain("SomePool");
                    info.isSingleEntity.should_be_false();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(true);
                    info.generateMethods.should_be(true);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(false);
                };

                it["creates a component with custom name"] = () => {
                    var provider = createProviderWithTypes(typeof(CustomName));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("SomeName");
                    info.typeName.should_be("SomeName");

                    info.memberInfos.Count.should_be(1);

                    info.memberInfos[0].type.should_be(typeof(CustomName));
                    info.memberInfos[0].name.should_be("value");

                    info.pools.should_contain("SomePool");
                    info.isSingleEntity.should_be_false();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(true);
                    info.generateMethods.should_be(true);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(false);
                };
            };

            context["when type implements IComponent"] = () => {
                it["finds no components and ignores IComponent itself"] = () => {
                    var provider = createProviderWithTypes(typeof(IComponent));
                    provider.componentInfos.should_be_empty();
                };

                it["finds no components and ignores interfaces"] = () => {
                    var provider = createProviderWithTypes(typeof(AnotherComponentInterface));
                    provider.componentInfos.should_be_empty();
                };

                it["finds no components and ignores abstract classes"] = () => {
                    var provider = createProviderWithTypes(typeof(AbstractComponent));
                    provider.componentInfos.should_be_empty();
                };

                it["creates component info from found component"] = () => {
                    var provider = createProviderWithTypes(typeof(ComponentA));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("ComponentA");
                    info.typeName.should_be("ComponentA");
                    info.memberInfos.should_be_empty();
                    info.pools.should_be_empty();
                    info.isSingleEntity.should_be_false();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(false);
                    info.generateMethods.should_be(true);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(true);
                };

                it["creates component info from found component with namespace"] = () => {
                    var provider = createProviderWithTypes(typeof(NamespaceComponent));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("My.Namespace.NamespaceComponent");
                    info.typeName.should_be("NamespaceComponent");
                    info.memberInfos.should_be_empty();
                    info.pools.should_be_empty();
                    info.isSingleEntity.should_be_false();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(false);
                    info.generateMethods.should_be(true);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(true);
                };

                it["detects PoolAttribure"] = () => {
                    var provider = createProviderWithTypes(typeof(CComponent));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("CComponent");
                    info.typeName.should_be("CComponent");
                    info.memberInfos.should_be_empty();
                    info.pools.Length.should_be(3);

                    info.pools.should_contain("PoolA");
                    info.pools.should_contain("PoolB");
                    info.pools.should_contain("PoolC");

                    info.isSingleEntity.should_be_false();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(false);
                    info.generateMethods.should_be(true);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(true);
                };

                it["detects SingleEntityAttribute "] = () => {
                    var provider = createProviderWithTypes(typeof(AnimatingComponent));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("AnimatingComponent");
                    info.typeName.should_be("AnimatingComponent");
                    info.memberInfos.should_be_empty();
                    info.pools.Length.should_be(0);
                    info.isSingleEntity.should_be_true();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(false);
                    info.generateMethods.should_be(true);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(true);
                };

                it["detects DontGenerateAttribute "] = () => {
                    var provider = createProviderWithTypes(typeof(DontGenerateComponent));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("DontGenerateComponent");
                    info.typeName.should_be("DontGenerateComponent");
                    info.memberInfos.should_be_empty();
                    info.pools.Length.should_be(0);
                    info.isSingleEntity.should_be_false();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(false);
                    info.generateMethods.should_be(false);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(true);
                };

                it["detects DontGenerateAttribute and don't generate index"] = () => {
                    var provider = createProviderWithTypes(typeof(DontGenerateIndexComponent));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("DontGenerateIndexComponent");
                    info.typeName.should_be("DontGenerateIndexComponent");
                    info.memberInfos.should_be_empty();
                    info.pools.Length.should_be(0);
                    info.isSingleEntity.should_be_false();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(false);
                    info.generateMethods.should_be(false);
                    info.generateIndex.should_be(false);
                    info.isSingletonComponent.should_be(true);
                };

                it["detects CustomPrefixAttribute"] = () => {
                    var provider = createProviderWithTypes(typeof(CustomPrefixComponent));
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be("CustomPrefixComponent");
                    info.typeName.should_be("CustomPrefixComponent");
                    info.memberInfos.should_be_empty();
                    info.pools.Length.should_be(0);
                    info.isSingleEntity.should_be_true();
                    info.singleComponentPrefix.should_be("My");
                    info.generateComponent.should_be(false);
                    info.generateMethods.should_be(true);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(true);
                };

                it["sets fields"] = () => {
                    var type = typeof(ComponentWithFieldsAndProperties);
                    var provider = createProviderWithTypes(type);
                    provider.componentInfos.Length.should_be(1);
                    var info = provider.componentInfos[0];

                    info.fullTypeName.should_be(type.FullName);
                    info.typeName.should_be(type.FullName);
                    info.memberInfos.Count.should_be(2);

                    info.memberInfos[0].type.should_be(typeof(string));
                    info.memberInfos[0].name.should_be("publicField");

                    info.memberInfos[1].type.should_be(typeof(string));
                    info.memberInfos[1].name.should_be("publicProperty");

                    info.pools.Length.should_be(0);
                    info.isSingleEntity.should_be_false();
                    info.singleComponentPrefix.should_be("is");
                    info.generateComponent.should_be(false);
                    info.generateMethods.should_be(true);
                    info.generateIndex.should_be(true);
                    info.isSingletonComponent.should_be(false);
                };

                it["gets multiple component infos"] = () => {
                    var provider = createProviderWithTypes(typeof(ComponentA), typeof(ComponentB));
                    provider.componentInfos.Length.should_be(2);
                    provider.componentInfos[0].fullTypeName.should_be("ComponentA");
                    provider.componentInfos[1].fullTypeName.should_be("ComponentB");
                };
            };
        };

        context["blueprint names"] = () => {
            it["has no blueprint names if empty"] = () => {
                var provider = createProviderWithBlueprintNames();
                provider.blueprintNames.should_be_empty();
            };

            it["has blueprint names if set"] = () => {
                var provider = createProviderWithBlueprintNames("My Blueprint1", "My Blueprint2");
                provider.blueprintNames.Length.should_be(2);
                provider.blueprintNames[0].should_be("My Blueprint1");
                provider.blueprintNames[1].should_be("My Blueprint2");
            };
        };
    }
}

