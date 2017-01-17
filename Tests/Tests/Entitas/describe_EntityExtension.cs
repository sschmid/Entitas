using NSpec;
using Entitas;

class describe_EntityExtension : nspec {

    void when_entity() {

        context["when adding ComponentSuffix"] = () => {

            it["doesn't add component suffix to string ending with ComponentSuffix"] = () => {
                const string str = "Position" + EntityExtension.COMPONENT_SUFFIX;
                str.AddComponentSuffix().should_be_same(str);
            };

            it["add ComponentSuffix to string not ending with ComponentSuffix"] = () => {
                const string str = "Position";
                str.AddComponentSuffix().should_be("Position" + EntityExtension.COMPONENT_SUFFIX);
            };
        };

        context["when removeing ComponentSuffix"] = () => {

            it["doesn't change string when not ending with ComponentSuffix"] = () => {
                const string str = "Position";
                str.RemoveComponentSuffix().should_be_same(str);
            };

            it["removes ComponentSuffix when ending with ComponentSuffix"] = () => {
                const string str = "Position" + EntityExtension.COMPONENT_SUFFIX;
                str.RemoveComponentSuffix().should_be("Position");
            };
        };

        context["when copying components"] = () => {

            IContext<TestEntity> ctx = null;
            TestEntity entity = null;
            TestEntity target = null;
            NameAgeComponent nameAge = null;

            before = () => {
                ctx = new TestContext();
                entity = ctx.CreateEntity();
                target = ctx.CreateEntity();
                nameAge = new NameAgeComponent { name = "Max", age = 42 };
            };

            it["doesn't change entity if original doesn't have any components"] = () => {
                entity.CopyTo(target);

                entity.creationIndex.should_be(0);
                target.creationIndex.should_be(1);

                target.GetComponents().Length.should_be(0);
            };

            it["adds copies of all components to target entity"] = () => {
                entity.AddComponentA();
                entity.AddComponent(CID.ComponentB, nameAge);

                entity.CopyTo(target);

                target.GetComponents().Length.should_be(2);
                target.HasComponentA().should_be_true();
                target.HasComponentB().should_be_true();
                target.GetComponentA().should_not_be_same(Component.A);
                target.GetComponent(CID.ComponentB).should_not_be_same(nameAge);

                var clonedComponent = (NameAgeComponent)target.GetComponent(CID.ComponentB);

                clonedComponent.name.should_be(nameAge.name);
                clonedComponent.age.should_be(nameAge.age);
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
                copy.should_not_be_same(nameAge);
                copy.should_not_be_same(component);
                ((NameAgeComponent)copy).name.should_be(nameAge.name);
                ((NameAgeComponent)copy).age.should_be(nameAge.age);
            };

            it["only adds copies of specified components to target entity"] = () => {
                entity.AddComponentA();
                entity.AddComponentB();
                entity.AddComponentC();

                entity.CopyTo(target, false, CID.ComponentB, CID.ComponentC);

                target.GetComponents().Length.should_be(2);
                target.HasComponentB().should_be_true();
                target.HasComponentC().should_be_true();
            };

            it["uses component pool"] = () => {
                entity.AddComponentA();

                var component = new ComponentA();
                target.GetComponentPool(CID.ComponentA).Push(component);

                entity.CopyTo(target);

                target.GetComponentA().should_be_same(component);
            };
        };
    }
}
