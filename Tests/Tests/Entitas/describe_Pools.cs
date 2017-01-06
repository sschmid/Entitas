using Entitas;
using NSpec;

class describe_Pools : nspec {

    void when_getting_instance() {

        it["instance is not null"] = () => {
            Contexts.sharedInstance.should_not_be_null();
        };

        it["resturn same shared instance"] = () => {
            Contexts.sharedInstance.should_be_same(Contexts.sharedInstance);
        };

        it["sets new shared instance"] = () => {
            var pools = new Contexts();
            Contexts.sharedInstance = pools;
            Contexts.sharedInstance.should_be_same(pools);
        };
    }
}
