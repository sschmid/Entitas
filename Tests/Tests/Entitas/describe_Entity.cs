using Entitas;
using NSpec;

class describe_Entity : nspec {

    void when_created() {

        Entity e = null;
        before = () => {
            e = this.CreateEntity();
        };

        it["has component of type when component of that type was added"] = () => {
            e.AddComponentA();
            e.HasComponentA().should_be_true();
        };

        it["returns entity when adding a component"] = () => {
            e.AddComponent(0, null).should_be_same(e);
        };

        it["doesn't have component of type when no component of that type was added"] = () => {
            e.HasComponentA().should_be_false();
        };

        it["doesn't have components of types when no components of these types were added"] = () => {
            e.HasComponents(new [] { CID.ComponentA }).should_be_false();
        };

        it["doesn't have components of types when not all components of these types were added"] = () => {
            e.AddComponentA();
            e.HasComponents(new [] { CID.ComponentA, CID.ComponentB }).should_be_false();
        };

        it["has components of types when all components of these types were added"] = () => {
            e.AddComponentA();
            e.AddComponentB();
            e.HasComponents(new [] { CID.ComponentA, CID.ComponentB }).should_be_true();
        };

        it["doesn't have any components of types when no components of these types were added"] = () => {
            e.HasAnyComponent(new [] { CID.ComponentA }).should_be_false();
        };

        it["has any components of types when any component of these types was added"] = () => {
            e.AddComponentA();
            e.HasAnyComponent(new [] {
                CID.ComponentA,
                CID.ComponentB
            }).should_be_true();
        };

        it["removes a component of type"] = () => {
            e.AddComponentA();
            e.RemoveComponentA();
            e.HasComponentA().should_be_false();
        };

        it["returns entity when removing a component"] = () => {
            e.AddComponentA();
            e.RemoveComponent(1).should_be_same(e);
        };

        it["gets a component of type"] = () => {
            e.AddComponentA();
            e.GetComponentA().should_be_same(Component.A);
        };
    
        it["replaces existing component"] = () => {
            e.AddComponentA();
            var newComponentA = new ComponentA();
            e.ReplaceComponentA(newComponentA);
            e.GetComponentA().should_be_same(newComponentA);
        };

        it["returns entity when replacing a component"] = () => {
            e.ReplaceComponent(1, null).should_be_same(e);
        };

        it["replacing a non existing component adds component"] = () => {
            var newComponentA = new ComponentA();
            e.ReplaceComponentA(newComponentA);
            e.GetComponentA().should_be_same(newComponentA);
        };

        it["gets empty array of components when no components were added"] = () => {
            e.GetComponents().should_be_empty();
        };

        it["gets empty array of component indices when no components were added"] = () => {
            e.GetComponentIndices().should_be_empty();
        };

        it["gets all components"] = () => {
            e.AddComponentA();
            e.AddComponentB();
            var allComponents = e.GetComponents();
            allComponents.should_contain(Component.A);
            allComponents.should_contain(Component.B);
            allComponents.Length.should_be(2);
        };

        it["gets all component indices"] = () => {
            e.AddComponentA();
            e.AddComponentB();
            var allComponentIndices = e.GetComponentIndices();
            allComponentIndices.should_contain(CID.ComponentA);
            allComponentIndices.should_contain(CID.ComponentB);
            allComponentIndices.Length.should_be(2);
        };

        it["removes all components"] = () => {
            e.AddComponentA();
            e.AddComponentB();
            e.RemoveAllComponents();
            e.HasComponentA().should_be_false();
            e.HasComponentB().should_be_false();
            e.GetComponents().should_be_empty();
            e.GetComponentIndices().should_be_empty();
        };

        it["can ToString"] = () => {
            e.AddComponentA();
            e.AddComponentB();
            e.ToString().should_be("Entity_0(ComponentA, ComponentB)");
        };

        context["events"] = () => {
            Entity eventEntity = null;
            int eventIndex = CID.None;
            IComponent eventComponent = null;

            before = () => {
                eventEntity = null;
                eventIndex = CID.None;
                eventComponent = null;
            };

            it["dispatches OnComponentAdded when adding a component"] = () => {
                e.OnComponentAdded += (entity, index, component) => {
                    eventEntity = entity;
                    eventIndex = index;
                    eventComponent = component;
                };
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => this.Fail();
                e.OnComponentRemoved += (entity, index, component) => this.Fail();

                e.AddComponentA();
                eventEntity.should_be_same(e);
                eventIndex.should_be(CID.ComponentA);
                eventComponent.should_be_same(Component.A);
            };

            it["dispatches OnComponentRemoved when removing a component"] = () => {
                e.AddComponentA();
                e.OnComponentAdded += (entity, index, component) => this.Fail();
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => this.Fail();
                e.OnComponentRemoved += (entity, index, component) => {
                    eventEntity = entity;
                    eventIndex = index;
                    eventComponent = component;
                };

                e.RemoveComponentA();
                eventEntity.should_be_same(e);
                eventIndex.should_be(CID.ComponentA);
                eventComponent.should_be_same(Component.A);
            };

            it["dispatches OnComponentReplaced when replacing a component"] = () => {
                e.AddComponentA();
                var newComponentA = new ComponentA();
                var didReplace = 0;
                e.OnComponentAdded += (entity, index, component) => this.Fail();
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => {
                    entity.should_be_same(entity);
                    index.should_be(CID.ComponentA);
                    newComponent.should_be_same(newComponentA);
                    didReplace++;
                };
                e.OnComponentRemoved += (entity, index, component) => this.Fail();
                
                e.ReplaceComponentA(newComponentA);
                didReplace.should_be(1);
            };

            it["doesn't dispatch anything when replacing a non existing component with null"] = () => {
                e.OnComponentAdded += (entity, index, component) => this.Fail();
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => this.Fail();
                e.OnComponentRemoved += (entity, index, component) => this.Fail();
                
                e.ReplaceComponentA(null);
            };

            it["dispatches OnComponentAdded when attempting to replace a component which hasn't been added"] = () => {
                e.OnComponentAdded += (entity, index, component) => {
                    eventEntity = entity;
                    eventIndex = index;
                    eventComponent = component;
                };
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => this.Fail();
                e.OnComponentRemoved += (entity, index, component) => this.Fail();

                var newComponentA = new ComponentA();
                e.ReplaceComponentA(newComponentA);
                eventEntity.should_be_same(e);
                eventIndex.should_be(CID.ComponentA);
                eventComponent.should_be_same(newComponentA);
            };

            it["dispatches OnComponentRemoved when removing all components"] = () => {
                var removed = 0;
                e.AddComponentA();
                e.AddComponentB();
                e.OnComponentRemoved += (entity, index, component) => removed++;
                e.RemoveAllComponents();
                removed.should_be(2);
            };
        };

        context["invalid operations"] = () => {
            it["throws when adding a component of the same type twice"] = expect<EntityAlreadyHasComponentException>(() => {
                e.AddComponentA();
                e.AddComponentA();
            });

            it["throws when attempting to remove a component of type which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                e.RemoveComponentA();
            });

            it["throws when attempting to get component of type which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                e.GetComponentA();
            });
        };

        context["internal caching"] = () => {
            context["components"] = () => {
                it["caches components"] = () => {
                    e.AddComponentA();
                    var c = e.GetComponents();
                    e.GetComponents().should_be_same(c);
                };

                it["updates cache when a new component was added"] = () => {
                    e.AddComponentA();
                    var c = e.GetComponents();
                    e.AddComponentB();
                    e.GetComponents().should_not_be_same(c);
                };

                it["updates cache when a component was removed"] = () => {
                    e.AddComponentA();
                    e.AddComponentB();
                    var c = e.GetComponents();
                    e.RemoveComponentA();
                    e.GetComponents().should_not_be_same(c);
                };

                it["updates cache when a component was replaced"] = () => {
                    e.AddComponentA();
                    e.AddComponentB();
                    var c = e.GetComponents();
                    e.ReplaceComponentA(new ComponentA());
                    e.GetComponents().should_not_be_same(c);
                };

                it["doesn't update cache when a component was replaced with same component"] = () => {
                    e.AddComponentA();
                    e.AddComponentB();
                    var c = e.GetComponents();
                    e.ReplaceComponentA(Component.A);
                    e.GetComponents().should_be_same(c);
                };

                it["updates cache when all components were removed"] = () => {
                    e.AddComponentA();
                    e.AddComponentB();
                    var c = e.GetComponents();
                    e.RemoveAllComponents();
                    e.GetComponents().should_not_be_same(c);
                };
            };

            context["component indices"] = () => {
                it["caches component indices"] = () => {
                    e.AddComponentA();
                    var c = e.GetComponentIndices();
                    e.GetComponentIndices().should_be_same(c);
                };

                it["updates cache when a new component was added"] = () => {
                    e.AddComponentA();
                    var c = e.GetComponentIndices();
                    e.AddComponentB();
                    e.GetComponentIndices().should_not_be_same(c);
                };

                it["updates cache when a component was removed"] = () => {
                    e.AddComponentA();
                    e.AddComponentB();
                    var c = e.GetComponentIndices();
                    e.RemoveComponentA();
                    e.GetComponentIndices().should_not_be_same(c);
                };

                it["doesn't update cache when a component was replaced"] = () => {
                    e.AddComponentA();
                    e.AddComponentB();
                    var c = e.GetComponentIndices();
                    e.ReplaceComponentA(new ComponentA());
                    e.GetComponentIndices().should_be_same(c);
                };

                it["updates cache when adding a new component with ReplaceComponent"] = () => {
                    e.AddComponentA();
                    e.AddComponentB();
                    var c = e.GetComponentIndices();
                    e.ReplaceComponentC(Component.C);
                    e.GetComponentIndices().should_not_be_same(c);
                };

                it["updates cache when all components were removed"] = () => {
                    e.AddComponentA();
                    e.AddComponentB();
                    var c = e.GetComponentIndices();
                    e.RemoveAllComponents();
                    e.GetComponentIndices().should_not_be_same(c);
                };
            };
        };

        context["component pooling"] = () => {

            it["provides previous and new component OnComponentReplaced when replacing with different component"] = () => {
                const int i = 1;
                var prevComp = new ComponentA();
                var newComp = new ComponentA();
                var didReplace = 0;
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => {
                    entity.should_be_same(e);
                    previousComponent.should_be_same(prevComp);
                    newComponent.should_be_same(newComp);

                    didReplace += 1;
                };

                e.AddComponent(i, prevComp);
                e.ReplaceComponent(i, newComp);

                didReplace.should_be(1);
            };

            it["provides previous and new component OnComponentReplaced when replacing with same component"] = () => {
                const int i = 1;
                var comp = new ComponentA();
                var didReplace = 0;
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => {
                    entity.should_be_same(e);
                    previousComponent.should_be_same(comp);
                    newComponent.should_be_same(comp);

                    didReplace += 1;
                };

                e.AddComponent(i, comp);
                e.ReplaceComponent(i, comp);

                didReplace.should_be(1);
            };
        };
    }
}

