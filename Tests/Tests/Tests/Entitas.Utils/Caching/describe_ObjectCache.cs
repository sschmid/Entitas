using Entitas.Utils;
using NSpec;

class describe_ObjectCache : nspec {

    void when_caching() {

        ObjectCache cache = null;

        before = () => {
            cache = new ObjectCache();
        };

        it["creates new object pool when requested"] = () => {
            cache.GetObjectPool<NameAgeComponent>().should_not_be_null();
        };

        it["returns same object pool already created"] = () => {
            cache.GetObjectPool<NameAgeComponent>().should_be_same(cache.GetObjectPool<NameAgeComponent>());
        };

        it["returns new instance"] = () => {
            var component = cache.Get<NameAgeComponent>();
            component.should_not_be_null();
        };

        it["returns pooled instance"] = () => {
            var component = cache.Get<NameAgeComponent>();
            cache.Push(component);
            cache.Get<NameAgeComponent>().should_be_same(component);
        };

        it["returns custom pushed instance"] = () => {
            var component = new NameAgeComponent();
            cache.Push(component);
            cache.Get<NameAgeComponent>().should_be_same(component);
        };

        it["registers custom object pool"] = () => {
            var objectPool = new ObjectPool<NameAgeComponent>(
                () => new NameAgeComponent { name = "Max" },
                c => c.name = null
            );

            cache.RegisterCustomObjectPool(objectPool);

            cache.GetObjectPool<NameAgeComponent>().should_be_same(objectPool);

            var component = cache.Get<NameAgeComponent>();
            component.name.should_be("Max");

            cache.Push(component);
            component.name.should_be_null();
        };

        it["resets"] = () => {
            var component = cache.Get<NameAgeComponent>();
            cache.Push(component);
            cache.Reset();
            cache.Get<NameAgeComponent>().should_not_be_same(component);
        };
    }
}
