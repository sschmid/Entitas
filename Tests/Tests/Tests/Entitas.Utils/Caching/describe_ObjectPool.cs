using Entitas;
using NSpec;

class describe_ObjectPool : nspec {

    void when_pooling() {

        const string NAME = "Max";

        ObjectPool<NameAgeComponent> objectPool = null;

        before = () => {
            objectPool = new ObjectPool<NameAgeComponent>(
                () => new NameAgeComponent { name = NAME },
                c => { c.name = null; c.age = -1; }
            );
        };

        it["gets new instance from pool"] = () => {
            var component = objectPool.Get();
            component.name.should_be(NAME);
        };

        it["gets pooled instance"] = () => {
            var component = new NameAgeComponent();
            objectPool.Push(component);
            objectPool.Get().should_be_same(component);
        };

        it["resets pushed instance"] = () => {
            var component = new NameAgeComponent { name = NAME, age = 42 };
            objectPool.Push(component);
            component.name.should_be_null();
            component.age.should_be(-1);
        };

        it["doesn't reset when reset method is null"] = () => {
            objectPool = new ObjectPool<NameAgeComponent>(() => new NameAgeComponent { name = NAME });
            var component = new NameAgeComponent { name = NAME };
            objectPool.Push(component);
            component.name.should_be(NAME);
            objectPool.Get().should_be_same(component);
        };
    }
}
