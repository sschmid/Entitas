using Entitas;
using NSpec;

class describe_Pools : nspec {

    void when_getting_instance() {

        it["instance is not null"] = () => {
            Pools.sharedInstance.should_not_be_null();
        };

        it["resturn same shared instance"] = () => {
            Pools.sharedInstance.should_be_same(Pools.sharedInstance);
        };

        it["sets new shared instance"] = () => {
            var pools = new Pools();
            Pools.sharedInstance = pools;
            Pools.sharedInstance.should_be_same(pools);
        };
    }
}

