using Entitas.CodeGenerator;
using NSpec;
using Entitas;
using My.Namespace;

class describe_TypeReflectionProvider : nspec {
    void when_providing() {

        context["pool names"] = () => {
            it["has no pool names if empty"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(object) }, new string[0]);
                provider.poolNames.should_be_empty();
            };

            it["has pool names if set"] = () => {
                var provider = new TypeReflectionProvider(
                                   new [] { typeof(TestPoolAttribute) },
                                   new [] { "Pool1", "Pool2" }
                               );
                provider.poolNames.Length.should_be(2);
                provider.poolNames[0].should_be("Pool1");
                provider.poolNames[1].should_be("Pool2");
            };
        };

        context["component infos"] = () => {

            it["finds no components if there are no types which implement IComponent"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(object) }, new string[0]);
                provider.componentInfos.should_be_empty();
            };

            it["finds no components and ignores IComponent itself"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(IComponent) }, new string[0]);
                provider.componentInfos.should_be_empty();
            };

            it["finds no components and ignores interfaces"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(AnotherComponentInterface) }, new string[0]);
                provider.componentInfos.should_be_empty();
            };

            it["creates component info from found component"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(ComponentA) }, new string[0]);
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("ComponentA");
                info.typeName.should_be("ComponentA");
                info.memberInfos.should_be_empty();
                info.pools.should_be_empty();
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["creates component info from found component with namespace"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(NamespaceComponent) }, new string[0]);
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("My.Namespace.NamespaceComponent");
                info.typeName.should_be("NamespaceComponent");
                info.memberInfos.should_be_empty();
                info.pools.should_be_empty();
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["detects PoolAttribure"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(CComponent) }, new string[0]);
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
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["detects SingleEntityAttribute "] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(AnimatingComponent) }, new string[0]);
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("AnimatingComponent");
                info.typeName.should_be("AnimatingComponent");
                info.memberInfos.should_be_empty();
                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_true();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["detects DontGenerateAttribute "] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(DontGenerateComponent) }, new string[0]);
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("DontGenerateComponent");
                info.typeName.should_be("DontGenerateComponent");
                info.memberInfos.should_be_empty();
                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(false);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["detects DontGenerateAttribute and don't generate index"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(DontGenerateIndexComponent) }, new string[0]);
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("DontGenerateIndexComponent");
                info.typeName.should_be("DontGenerateIndexComponent");
                info.memberInfos.should_be_empty();
                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(false);
                info.generateIndex.should_be(false);
                info.isSingletonComponent.should_be(true);
            };

            it["detects CustomPrefixAttribute"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(CustomPrefixComponent) }, new string[0]);
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("CustomPrefixComponent");
                info.typeName.should_be("CustomPrefixComponent");
                info.memberInfos.should_be_empty();
                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_true();
                info.singleComponentPrefix.should_be("My");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["sets fields"] = () => {
                var type = typeof(ComponentWithFieldsAndProperties);
                var provider = new TypeReflectionProvider(new [] { type }, new string[0]);
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be(type.FullName);
                info.typeName.should_be(type.FullName);
                info.memberInfos.Length.should_be(2);

                info.memberInfos[0].fullTypeName.should_be("string");
                info.memberInfos[0].name.should_be("publicField");

                info.memberInfos[1].fullTypeName.should_be("string");
                info.memberInfos[1].name.should_be("publicProperty");

                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(false);
            };

            it["gets multiple component infos"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(ComponentA), typeof(ComponentB) }, new string[0]);
                provider.componentInfos.Length.should_be(2);
                provider.componentInfos[0].fullTypeName.should_be("ComponentA");
                provider.componentInfos[1].fullTypeName.should_be("ComponentB");
            };
        };
    }
}

