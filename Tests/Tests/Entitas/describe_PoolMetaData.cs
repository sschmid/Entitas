using NSpec;
using Entitas;

class describe_PoolMetaData : nspec {

    void when_created() {

        it["has values"] = () => {

            var componentNames = new [] { "Health", "Position", "View" };
            var componentTypes = new [] { typeof(ComponentA), typeof(ComponentB), typeof(ComponentC) };
            const string poolName = "My Pool";

            var data = new PoolMetaData(poolName, componentNames, componentTypes);

            data.poolName.should_be(poolName);
            data.componentNames.should_be_same(componentNames);
            data.componentTypes.should_be_same(componentTypes);
        };
    }
}

