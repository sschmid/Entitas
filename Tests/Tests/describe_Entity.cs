using Entitas;
using NSpec;

class describe_Entity : nspec {
    Entity _entity;
    ComponentA _componentA;
    ComponentB _componentB;

    void before_each() {
        _entity = new Entity(CP.NumComponents);
        _componentA = new ComponentA();
        _componentB = new ComponentB();
    }

    void it_has_specified_creation_index() {
        const int i = 1;
        new Entity(0, i).creationIndex.should_be(i);
    }

    void when_created() {
        it["has default creation index"] = () => {
            _entity.creationIndex.should_be(0);
        };

        it["has component of type when component of that type was added"] = () => {
            addComponentA(_entity);
            hasComponentA(_entity).should_be_true();
        };

        it["doesn't have component of type when no component of that type was added"] = () => {
            hasComponentA(_entity).should_be_false();
        };

        it["doesn't have components of types when no components of these types were added"] = () => {
            _entity.HasComponents(new [] { CP.ComponentA }).should_be_false();
        };

        it["doesn't have components of types when not all components of these types were added"] = () => {
            addComponentA(_entity);
            hasComponentsAB(_entity).should_be_false();
        };

        it["has components of types when all components of these types were added"] = () => {
            addComponentA(_entity);
            addComponentB(_entity);
            hasComponentsAB(_entity).should_be_true();
        };

        it["doesn't have any components of types when no components of these types were added"] = () => {
            _entity.HasAnyComponent(new [] { CP.ComponentA }).should_be_false();
        };

        it["has any components of types when any component of these types was added"] = () => {
            addComponentA(_entity);
            _entity.HasAnyComponent(new [] {
                CP.ComponentA,
                CP.ComponentB
            }).should_be_true();
        };

        it["removes a component of type"] = () => {
            addComponentA(_entity);
            removeComponentA(_entity);
            hasComponentA(_entity).should_be_false();
        };

        it["gets a component of type"] = () => {
            addComponentA(_entity);
            getComponentA(_entity).should_be_same(_componentA);
        };
    
        it["replaces existing component"] = () => {
            addComponentA(_entity);
            var newComponentA = new ComponentA();
            replaceComponentA(_entity, newComponentA);
            getComponentA(_entity).should_be_same(newComponentA);
        };

        it["replacing a non existing component adds component"] = () => {
            var newComponentA = new ComponentA();
            replaceComponentA(_entity, newComponentA);
            getComponentA(_entity).should_be_same(newComponentA);
        };

        it["gets empty array of components when no components were added"] = () => {
            _entity.GetComponents().should_be_empty();
        };

        it["gets empty array of component types when no components were added"] = () => {
            _entity.GetComponentIndices().should_be_empty();
        };

        it["gets all components"] = () => {
            addComponentA(_entity);
            addComponentB(_entity);
            var allComponents = _entity.GetComponents();
            allComponents.should_contain(_componentA);
            allComponents.should_contain(_componentB);
            allComponents.Length.should_be(2);
        };

        it["gets all component types"] = () => {
            addComponentA(_entity);
            addComponentB(_entity);
            var allComponentTypes = _entity.GetComponentIndices();
            allComponentTypes.should_contain(CP.ComponentA);
            allComponentTypes.should_contain(CP.ComponentB);
            allComponentTypes.Length.should_be(2);
        };

        it["removes all components"] = () => {
            addComponentA(_entity);
            addComponentB(_entity);
            _entity.RemoveAllComponents();
            hasComponentA(_entity).should_be_false();
            hasComponentB(_entity).should_be_false();
            _entity.GetComponents().should_be_empty();
            _entity.GetComponentIndices().should_be_empty();
        };

        it["can ToString"] = () => {
            addComponentA(_entity);
            addComponentB(_entity);
            _entity.ToString().should_be("Entity(ComponentA, ComponentB)");
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
                _entity.OnComponentReplaced += (entity, type, component) => TestHelper.Fail();
                _entity.OnComponentRemoved += (entity, type, component) => TestHelper.Fail();
                _entity.OnComponentAdded += (entity, type, component) => {
                    eventEntity = entity;
                    eventType = type;
                    eventComponent = component;
                };
                addComponentA(_entity);

                eventEntity.should_be_same(_entity);
                eventType.should_be(CP.ComponentA);
                eventComponent.should_be_same(_componentA);
            };

            it["dispatches OnComponentRemoved when removing a component"] = () => {
                addComponentA(_entity);
                _entity.OnComponentAdded += (entity, type, component) => TestHelper.Fail();
                _entity.OnComponentReplaced += (entity, type, component) => TestHelper.Fail();
                _entity.OnComponentRemoved += (entity, type, component) => {
                    eventEntity = entity;
                    eventType = type;
                    eventComponent = component;
                };
                removeComponentA(_entity);

                eventEntity.should_be_same(_entity);
                eventType.should_be(CP.ComponentA);
                eventComponent.should_be_same(_componentA);
            };

            it["dispatches OnComponentReplaced when replacing a component"] = () => {
                _entity.AddComponent(CP.ComponentA, _componentA);
                _entity.OnComponentAdded += (entity, type, component) => TestHelper.Fail();
                _entity.OnComponentRemoved += (entity, type, component) => TestHelper.Fail();
                _entity.OnComponentReplaced += (entity, type, component) => {
                    eventEntity = entity;
                    eventType = type;
                    eventComponent = component;
                };
                var newComponentA = new ComponentA();
                replaceComponentA(_entity, newComponentA);

                eventEntity.should_be_same(_entity);
                eventType.should_be(CP.ComponentA);
                eventComponent.should_be_same(newComponentA);
            };

            it["dispatches OnComponentAdded when attempting to replace a component which hasn't been added"] = () => {
                _entity.OnComponentRemoved += (entity, type, component) => TestHelper.Fail();
                _entity.OnComponentReplaced += (entity, type, component) => TestHelper.Fail();
                _entity.OnComponentAdded += (entity, type, component) => {
                    eventEntity = entity;
                    eventType = type;
                    eventComponent = component;
                };
                var newComponentA = new ComponentA();
                replaceComponentA(_entity, newComponentA);

                eventEntity.should_be_same(_entity);
                eventType.should_be(CP.ComponentA);
                eventComponent.should_be_same(newComponentA);
            };

            it["dispatches OnComponentRemoved when removing all components"] = () => {
                var dispatched = 0;
                addComponentA(_entity);
                addComponentB(_entity);
                _entity.OnComponentRemoved += (entity, type, component) => dispatched++;
                _entity.RemoveAllComponents();
                dispatched.should_be(2);
            };
        };

        context["invalid operations"] = () => {
            it["throws when adding a component of the same type twice"] = expect<EntityAlreadyHasComponentException>(() => {
                addComponentA(_entity);
                _entity.AddComponent(CP.ComponentA, new ComponentA());
            });

            it["throws when attempting to remove a component of type which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                removeComponentA(_entity);
            });

            it["throws when attempting to get component of type which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                getComponentA(_entity);
            });
        };

        context["internal caching"] = () => {
            context["components"] = () => {
                it["caches components"] = () => {
                    addComponentA(_entity);
                    var c = _entity.GetComponents();
                    _entity.GetComponents().should_be_same(c);
                };

                it["updates cache when a new component was added"] = () => {
                    addComponentA(_entity);
                    var c = _entity.GetComponents();
                    addComponentB(_entity);
                    _entity.GetComponents().should_not_be_same(c);
                };

                it["updates cache when a component was removed"] = () => {
                    addComponentA(_entity);
                    addComponentB(_entity);
                    var c = _entity.GetComponents();
                    removeComponentA(_entity);
                    _entity.GetComponents().should_not_be_same(c);
                };

                it["updates cache when a component was replaced"] = () => {
                    addComponentA(_entity);
                    addComponentB(_entity);
                    var c = _entity.GetComponents();
                    replaceComponentA(_entity, new ComponentA());
                    _entity.GetComponents().should_not_be_same(c);
                };

                it["doesn't update cache when a component was replaced with same component"] = () => {
                    addComponentA(_entity);
                    addComponentB(_entity);
                    var c = _entity.GetComponents();
                    replaceComponentA(_entity, _componentA);
                    _entity.GetComponents().should_be_same(c);
                };

                it["updates cache when all components were removed"] = () => {
                    addComponentA(_entity);
                    addComponentB(_entity);
                    var c = _entity.GetComponents();
                    _entity.RemoveAllComponents();
                    _entity.GetComponents().should_not_be_same(c);
                };
            };

            context["component types"] = () => {
                it["caches component types"] = () => {
                    addComponentA(_entity);
                    var c = _entity.GetComponentIndices();
                    _entity.GetComponentIndices().should_be_same(c);
                };

                it["updates cache when a new component was added"] = () => {
                    addComponentA(_entity);
                    var c = _entity.GetComponentIndices();
                    addComponentB(_entity);
                    _entity.GetComponentIndices().should_not_be_same(c);
                };

                it["updates cache when a component was removed"] = () => {
                    addComponentA(_entity);
                    addComponentB(_entity);
                    var c = _entity.GetComponentIndices();
                    removeComponentA(_entity);
                    _entity.GetComponentIndices().should_not_be_same(c);
                };

                it["doesn't update cache when a component was replaced"] = () => {
                    addComponentA(_entity);
                    addComponentB(_entity);
                    var c = _entity.GetComponentIndices();
                    replaceComponentA(_entity, new ComponentA());
                    _entity.GetComponentIndices().should_be_same(c);
                };

                it["updates cache when adding a new component with ReplaceComponent"] = () => {
                    addComponentA(_entity);
                    addComponentB(_entity);
                    var c = _entity.GetComponentIndices();
                    _entity.ReplaceComponent(CP.ComponentC, new ComponentC());
                    _entity.GetComponentIndices().should_not_be_same(c);
                };

                it["updates cache when all components were removed"] = () => {
                    addComponentA(_entity);
                    addComponentB(_entity);
                    var c = _entity.GetComponentIndices();
                    _entity.RemoveAllComponents();
                    _entity.GetComponentIndices().should_not_be_same(c);
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

    void replaceComponentA(Entity entity, ComponentA componentA) {
        entity.ReplaceComponent(CP.ComponentA, componentA);
    }
}

