using Entitas;
using NSpec;
using Shouldly;

class describe_PublicMemberInfoEntityExtension : nspec {

    void when_extending() {

        context["when copying components"] = () => {

            IContext<TestEntity> ctx = null;
            TestEntity entity = null;
            TestEntity target = null;
            NameAgeComponent nameAge = null;

            before = () => {
                ctx = new MyTestContext();
                entity = ctx.CreateEntity();
                target = ctx.CreateEntity();
                nameAge = new NameAgeComponent { name = "Max", age = 42 };
            };

            it["doesn't change entity if original doesn't have any components"] = () => {
                entity.CopyTo(target);

                entity.creationIndex.ShouldBe(0);
                target.creationIndex.ShouldBe(1);

                target.GetComponents().Length.ShouldBe(0);
            };

            it["adds copies of all components to target entity"] = () => {
                entity.AddComponentA();
                entity.AddComponent(CID.ComponentB, nameAge);

                entity.CopyTo(target);

                target.GetComponents().Length.ShouldBe(2);
                target.HasComponentA().ShouldBeTrue();
                target.HasComponentB().ShouldBeTrue();
                target.GetComponentA().ShouldNotBeSameAs(Component.A);
                target.GetComponent(CID.ComponentB).ShouldNotBeSameAs(nameAge);

                var clonedComponent = (NameAgeComponent)target.GetComponent(CID.ComponentB);

                clonedComponent.name.ShouldBe(nameAge.name);
                clonedComponent.age.ShouldBe(nameAge.age);
            };

            it["throws when target already has a component at index"] = base.expect<EntityAlreadyHasComponentException>(() => {
                entity.AddComponentA();
                entity.AddComponent(CID.ComponentB, nameAge);
                var component = new NameAgeComponent();
                target.AddComponent(CID.ComponentB, component);

                entity.CopyTo(target);
            });

            it["replaces existing components when overwrite is set"] = () => {
                entity.AddComponentA();
                entity.AddComponent(CID.ComponentB, nameAge);
                var component = new NameAgeComponent();
                target.AddComponent(CID.ComponentB, component);

                entity.CopyTo(target, true);

                var copy = target.GetComponent(CID.ComponentB);
                copy.ShouldNotBeSameAs(nameAge);
                copy.ShouldNotBeSameAs(component);
                ((NameAgeComponent)copy).name.ShouldBe(nameAge.name);
                ((NameAgeComponent)copy).age.ShouldBe(nameAge.age);
            };

            it["only adds copies of specified components to target entity"] = () => {
                entity.AddComponentA();
                entity.AddComponentB();
                entity.AddComponentC();

                entity.CopyTo(target, false, CID.ComponentB, CID.ComponentC);

                target.GetComponents().Length.ShouldBe(2);
                target.HasComponentB().ShouldBeTrue();
                target.HasComponentC().ShouldBeTrue();
            };

            it["uses component pool"] = () => {
                entity.AddComponentA();

                var component = new ComponentA();
                target.GetComponentPool(CID.ComponentA).Push(component);

                entity.CopyTo(target);

                target.GetComponentA().ShouldBeSameAs(component);
            };
        };
    }
}
