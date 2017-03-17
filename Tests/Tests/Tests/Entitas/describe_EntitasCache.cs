using Entitas;
using NSpec;

class describe_EntitasCache : nspec {

    void when_caching() {

        before = () => {
            EntitasCache.Reset();
        };

        it["clears IComponent list"] = () => {
            var list = EntitasCache.GetIComponentList();
            list.Add(Component.A);
            EntitasCache.PushIComponentList(list);

            EntitasCache.GetIComponentList().Count.should_be(0);
        };

        it["clears int list"] = () => {
            var list = EntitasCache.GetIntList();
            list.Add(42);
            EntitasCache.PushIntList(list);

            EntitasCache.GetIntList().Count.should_be(0);
        };

        it["clears int hashSet"] = () => {
            var hashSet = EntitasCache.GetIntHashSet();
            hashSet.Add(42);
            EntitasCache.PushIntHashSet(hashSet);

            EntitasCache.GetIntHashSet().Count.should_be(0);
        };
    }
}
