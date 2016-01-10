using NSpec;
using Entitas;

class describe_PoolMetaData : nspec {

    void when_created() {

        it["has values"] = () => {

            var componentNames = new [] { "Health", "Position", "View" };
            const string poolName = "My Pool";

            var data = new PoolMetaData(poolName, componentNames);

            data.poolName.should_be(poolName);
            data.componentNames.should_be_same(componentNames);
        };
    }
}

