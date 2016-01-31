using Entitas.CodeGenerator;
using NSpec;
using Entitas;
using My.Namespace;

class describe_TypeReflectionProvider : nspec {
    void when_providing() {

        context["pool names"] = () => {
            it["finds no pool names if there are no types which subclass PoolAttribute"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(object) });
                provider.poolNames.should_be_empty();
            };

            it["finds no pool names and ignores PoolAttribute itself"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(PoolAttribute) });
                provider.poolNames.should_be_empty();
            };

            it["finds pool names if there are types which inherit from PoolAttribute"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(TestPoolAttribute) });
                provider.poolNames.Length.should_be(1);
                provider.poolNames[0].should_be("Test");
            };
        };

        context["component infos"] = () => {

            it["finds no components if there are no types which implement IComponent"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(object) });
                provider.componentInfos.should_be_empty();
            };

            it["finds no components and ignores IComponent itself"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(IComponent) });
                provider.componentInfos.should_be_empty();
            };

            it["creates component info from found component"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(ComponentA) });
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("ComponentA");
                info.typeName.should_be("ComponentA");
                info.fieldInfos.should_be_empty();
                info.pools.should_be_empty();
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["creates component info from found component with namespace"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(NamespaceComponent) });
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("My.Namespace.NamespaceComponent");
                info.typeName.should_be("NamespaceComponent");
                info.fieldInfos.should_be_empty();
                info.pools.should_be_empty();
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["detects PoolAttribure"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(CComponent) });
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("CComponent");
                info.typeName.should_be("CComponent");
                info.fieldInfos.should_be_empty();
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
                var provider = new TypeReflectionProvider(new [] { typeof(AnimatingComponent) });
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("AnimatingComponent");
                info.typeName.should_be("AnimatingComponent");
                info.fieldInfos.should_be_empty();
                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_true();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["detects DontGenerateAttribute "] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(DontGenerateComponent) });
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("DontGenerateComponent");
                info.typeName.should_be("DontGenerateComponent");
                info.fieldInfos.should_be_empty();
                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(false);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["detects DontGenerateAttribute and don't generate index"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(DontGenerateIndexComponent) });
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("DontGenerateIndexComponent");
                info.typeName.should_be("DontGenerateIndexComponent");
                info.fieldInfos.should_be_empty();
                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(false);
                info.generateIndex.should_be(false);
                info.isSingletonComponent.should_be(true);
            };

            it["detects CustomPrefixAttribute"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(CustomPrefixComponent) });
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("CustomPrefixComponent");
                info.typeName.should_be("CustomPrefixComponent");
                info.fieldInfos.should_be_empty();
                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_true();
                info.singleComponentPrefix.should_be("My");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["sets fields"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(PersonComponent) });
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.fullTypeName.should_be("PersonComponent");
                info.typeName.should_be("PersonComponent");
                info.fieldInfos.Length.should_be(2);

                info.fieldInfos[0].type.should_be("int");
                info.fieldInfos[0].name.should_be("age");

                info.fieldInfos[1].type.should_be("string");
                info.fieldInfos[1].name.should_be("name");

                info.pools.Length.should_be(0);
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(false);
            };

            it["gets multiple component infos"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(ComponentA), typeof(ComponentB) });
                provider.componentInfos.Length.should_be(2);
                provider.componentInfos[0].fullTypeName.should_be("ComponentA");
                provider.componentInfos[1].fullTypeName.should_be("ComponentB");
            };
        };
    }
}

