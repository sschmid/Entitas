using Entitas.CodeGenerator;
using NSpec;
using Entitas;

class describe_TypeReflectionProvider : nspec {
    void when_providing() {

        context["pool names"] = () => {
            it["finds no pool names"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(object) });
                provider.poolNames.should_be_empty();
            };

            it["finds no pool names"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(PoolAttribute) });
                provider.poolNames.should_be_empty();
            };

            it["finds pool names"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(TestPoolAttribute) });
                provider.poolNames.Length.should_be(1);
                provider.poolNames[0].should_be("Test");
            };
        };

        context["component infos"] = () => {

            it["finds no components"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(object) });
                provider.componentInfos.should_be_empty();
            };

            it["finds no components"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(IComponent) });
                provider.componentInfos.should_be_empty();
            };

            it["finds singleton component"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(ComponentA) });
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.type.should_be("ComponentA");
                info.fieldInfos.should_be_empty();
                info.pools.should_be_empty();
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };

            it["finds singleton component with pools"] = () => {
                var provider = new TypeReflectionProvider(new [] { typeof(CComponent) });
                provider.componentInfos.Length.should_be(1);
                var info = provider.componentInfos[0];

                info.type.should_be("CComponent");
                info.fieldInfos.should_be_empty();
                info.pools.Length.should_be(3);
                info.isSingleEntity.should_be_false();
                info.singleComponentPrefix.should_be("is");
                info.generateMethods.should_be(true);
                info.generateIndex.should_be(true);
                info.isSingletonComponent.should_be(true);
            };
        };
    }
}

