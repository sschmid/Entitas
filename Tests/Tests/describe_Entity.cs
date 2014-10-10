using Entitas;
using NSpec;

class describe_Entity : nspec {
    readonly ComponentA _componentA = new ComponentA();
    readonly ComponentB _componentB = new ComponentB();

    void when_created() {

        Entity e = null;
        before = () => {
            e = new Entity(CP.NumComponents);
        };

        it["has component of type when component of that type was added"] = () => {
            addComponentA(e);
            hasComponentA(e).should_be_true();
        };

        it["doesn't have component of type when no component of that type was added"] = () => {
            hasComponentA(e).should_be_false();
        };

        it["doesn't have components of types when no components of these types were added"] = () => {
            e.HasComponents(new [] { CP.ComponentA }).should_be_false();
        };

        it["doesn't have components of types when not all components of these types were added"] = () => {
            addComponentA(e);
            hasComponentsAB(e).should_be_false();
        };

        it["has components of types when all components of these types were added"] = () => {
            addComponentA(e);
            addComponentB(e);
            hasComponentsAB(e).should_be_true();
        };

        it["doesn't have any components of types when no components of these types were added"] = () => {
            e.HasAnyComponent(new [] { CP.ComponentA }).should_be_false();
        };

        it["has any components of types when any component of these types was added"] = () => {
            addComponentA(e);
            e.HasAnyComponent(new [] {
                CP.ComponentA,
                CP.ComponentB
            }).should_be_true();
        };

        it["removes a component of type"] = () => {
            addComponentA(e);
            removeComponentA(e);
            hasComponentA(e).should_be_false();
        };

        it["gets a component of type"] = () => {
            addComponentA(e);
            getComponentA(e).should_be_same(_componentA);
        };
    
        it["replaces existing component"] = () => {
            addComponentA(e);
            var newComponentA = new ComponentA();
            replaceComponentA(e, newComponentA);
            getComponentA(e).should_be_same(newComponentA);
        };

        it["replacing a non existing component adds component"] = () => {
            var newComponentA = new ComponentA();
            replaceComponentA(e, newComponentA);
            getComponentA(e).should_be_same(newComponentA);
        };

        it["gets empty array of components when no components were added"] = () => {
            e.GetComponents().should_be_empty();
        };

        it["gets empty array of component indices when no components were added"] = () => {
            e.GetComponentIndices().should_be_empty();
        };

        it["gets all components"] = () => {
            addComponentA(e);
            addComponentB(e);
            var allComponents = e.GetComponents();
            allComponents.should_contain(_componentA);
            allComponents.should_contain(_componentB);
            allComponents.Length.should_be(2);
        };

        it["gets all component indices"] = () => {
            addComponentA(e);
            addComponentB(e);
            var allComponentTypes = e.GetComponentIndices();
            allComponentTypes.should_contain(CP.ComponentA);
            allComponentTypes.should_contain(CP.ComponentB);
            allComponentTypes.Length.should_be(2);
        };

        it["removes all components"] = () => {
            addComponentA(e);
            addComponentB(e);
            e.RemoveAllComponents();
            hasComponentA(e).should_be_false();
            hasComponentB(e).should_be_false();
            e.GetComponents().should_be_empty();
            e.GetComponentIndices().should_be_empty();
        };

        it["can ToString"] = () => {
            addComponentA(e);
            addComponentB(e);
            e.ToString().should_be("Entity(ComponentA, ComponentB)");
        };

        context["events"] = () => {
            Entity eventEntity = null;
            int eventType = CP.ComponentC;
            IComponent eventComponent = null;

            before = () => {
                eventEntity = null;
                eventType = CP.ComponentC;
                eventComponent = null;
            };

            it["dispatches OnComponentAdded when adding a component"] = () => {
                e.OnComponentWillBeRemoved += (entity, type, component) => TestHelper.Fail();
                e.OnComponentRemoved += (entity, type, component) => TestHelper.Fail();
                e.OnComponentAdded += (entity, type, component) => {
                    eventEntity = entity;
                    eventType = type;
                    eventComponent = component;
                };
                addComponentA(e);

                eventEntity.should_be_same(e);
                eventType.should_be(CP.ComponentA);
                eventComponent.should_be_same(_componentA);
            };

            it["dispatches OnComponentRemoved when removing a component"] = () => {
                addComponentA(e);
                e.OnComponentAdded += (entity, type, component) => TestHelper.Fail();
                e.OnComponentRemoved += (entity, type, component) => {
                    eventEntity = entity;
                    eventType = type;
                    eventComponent = component;
                };
                removeComponentA(e);

                eventEntity.should_be_same(e);
                eventType.should_be(CP.ComponentA);
                eventComponent.should_be_same(_componentA);
            };

            it["dispatches OnComponentWillBeRemoved when removing a component"] = () => {
                addComponentA(e);
                e.OnComponentWillBeRemoved += (entity, type, component) => {
                    eventEntity = entity;
                    eventType = type;
                    eventComponent = component;
                };
                removeComponentA(e);

                eventEntity.should_be_same(e);
                eventType.should_be(CP.ComponentA);
                eventComponent.should_be_same(_componentA);
            };

            it["dispatches OnComponentAdded and OnComponentRemoved when replacing a component"] = () => {
                addComponentA(e);
                var newComponentA = new ComponentA();
                var didAdd = 0;
                e.OnComponentAdded += (entity, type, component) => {
                    entity.should_be_same(entity);
                    type.should_be(CP.ComponentA);
                    component.should_be_same(newComponentA);
                    didAdd++;
                };

                e.OnComponentWillBeRemoved += (entity, type, component) => TestHelper.Fail();

                var didRemove = 0;
                e.OnComponentRemoved += (entity, type, component) => {
                    entity.should_be_same(entity);
                    type.should_be(CP.ComponentA);
                    component.should_be_same(_componentA);
                    didRemove++;
                };
                replaceComponentA(e, newComponentA);
                didAdd.should_be(1);
                didRemove.should_be(1);
            };

            it["dispatches OnComponentWillBeRemoved when called manually and component exists"] = () => {
                addComponentA(e);
                e.OnComponentWillBeRemoved += (entity, type, component) => {
                    eventEntity = entity;
                    eventType = type;
                    eventComponent = component;
                };

                e.WillRemoveComponent(CP.ComponentA);

                eventEntity.should_be_same(e);
                eventType.should_be(CP.ComponentA);
                eventComponent.should_be_same(_componentA);
            };

            it["doesn't dispatch OnComponentWillBeRemoved when called manually and component is null"] = () => {
                e.OnComponentWillBeRemoved += (entity, type, component) => {
                    eventEntity = entity;
                    eventType = type;
                    eventComponent = component;
                };

                e.WillRemoveComponent(CP.ComponentA);

                eventEntity.should_be_null();
                eventType.should_be(CP.ComponentC);
                eventComponent.should_be_null();
            };

            it["dispatches OnComponentAdded when attempting to replace a component which hasn't been added"] = () => {
                e.OnComponentWillBeRemoved += (entity, type, component) => TestHelper.Fail();
                e.OnComponentRemoved += (entity, type, component) => TestHelper.Fail();
                e.OnComponentAdded += (entity, type, component) => {
                    eventEntity = entity;
                    eventType = type;
                    eventComponent = component;
                };
                var newComponentA = new ComponentA();
                replaceComponentA(e, newComponentA);

                eventEntity.should_be_same(e);
                eventType.should_be(CP.ComponentA);
                eventComponent.should_be_same(newComponentA);
            };

            it["dispatches OnComponentRemoved when removing all components"] = () => {
                var removed = 0;
                addComponentA(e);
                addComponentB(e);
                e.OnComponentRemoved += (entity, type, component) => removed++;
                e.RemoveAllComponents();
                removed.should_be(2);
            };
        };

        context["invalid operations"] = () => {
            it["throws when adding a component of the same type twice"] = expect<EntityAlreadyHasComponentException>(() => {
                addComponentA(e);
                addComponentA(e);
            });

            it["throws when attempting to remove a component of type which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                removeComponentA(e);
            });

            it["throws when attempting to get component of type which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                getComponentA(e);
            });
        };

        context["internal caching"] = () => {
            context["components"] = () => {
                it["caches components"] = () => {
                    addComponentA(e);
                    var c = e.GetComponents();
                    e.GetComponents().should_be_same(c);
                };

                it["updates cache when a new component was added"] = () => {
                    addComponentA(e);
                    var c = e.GetComponents();
                    addComponentB(e);
                    e.GetComponents().should_not_be_same(c);
                };

                it["updates cache when a component was removed"] = () => {
                    addComponentA(e);
                    addComponentB(e);
                    var c = e.GetComponents();
                    removeComponentA(e);
                    e.GetComponents().should_not_be_same(c);
                };

                it["updates cache when a component was replaced"] = () => {
                    addComponentA(e);
                    addComponentB(e);
                    var c = e.GetComponents();
                    replaceComponentA(e, new ComponentA());
                    e.GetComponents().should_not_be_same(c);
                };

                it["doesn't update cache when a component was replaced with same component"] = () => {
                    addComponentA(e);
                    addComponentB(e);
                    var c = e.GetComponents();
                    replaceComponentA(e, _componentA);
                    e.GetComponents().should_be_same(c);
                };

                it["updates cache when all components were removed"] = () => {
                    addComponentA(e);
                    addComponentB(e);
                    var c = e.GetComponents();
                    e.RemoveAllComponents();
                    e.GetComponents().should_not_be_same(c);
                };
            };

            context["component types"] = () => {
                it["caches component types"] = () => {
                    addComponentA(e);
                    var c = e.GetComponentIndices();
                    e.GetComponentIndices().should_be_same(c);
                };

                it["updates cache when a new component was added"] = () => {
                    addComponentA(e);
                    var c = e.GetComponentIndices();
                    addComponentB(e);
                    e.GetComponentIndices().should_not_be_same(c);
                };

                it["updates cache when a component was removed"] = () => {
                    addComponentA(e);
                    addComponentB(e);
                    var c = e.GetComponentIndices();
                    removeComponentA(e);
                    e.GetComponentIndices().should_not_be_same(c);
                };

                it["doesn't update cache when a component was replaced"] = () => {
                    addComponentA(e);
                    addComponentB(e);
                    var c = e.GetComponentIndices();
                    replaceComponentA(e, new ComponentA());
                    e.GetComponentIndices().should_be_same(c);
                };

                it["updates cache when adding a new component with ReplaceComponent"] = () => {
                    addComponentA(e);
                    addComponentB(e);
                    var c = e.GetComponentIndices();
                    e.ReplaceComponent(CP.ComponentC, new ComponentC());
                    e.GetComponentIndices().should_not_be_same(c);
                };

                it["updates cache when all components were removed"] = () => {
                    addComponentA(e);
                    addComponentB(e);
                    var c = e.GetComponentIndices();
                    e.RemoveAllComponents();
                    e.GetComponentIndices().should_not_be_same(c);
                };
            };
        };
    }

    void addComponentA(Entity entity) {
        entity.AddComponent(CP.ComponentA, _componentA);
    }

    void addComponentB(Entity entity) {
        entity.AddComponent(CP.ComponentB, _componentB);
    }

    bool hasComponentA(Entity entity) {
        return entity.HasComponent(CP.ComponentA);
    }

    bool hasComponentB(Entity entity) {
        return entity.HasComponent(CP.ComponentB);
    }

    bool hasComponentsAB(Entity entity) {
        return entity.HasComponents(new [] { CP.ComponentA, CP.ComponentB });
    }

    void removeComponentA(Entity entity) {
        entity.RemoveComponent(CP.ComponentA);
    }

    IComponent getComponentA(Entity entity) {
        return entity.GetComponent(CP.ComponentA);
    }

    void replaceComponentA(Entity entity, ComponentA component) {
        entity.ReplaceComponent(CP.ComponentA, component);
    }
}

