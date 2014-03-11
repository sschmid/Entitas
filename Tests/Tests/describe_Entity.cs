using Entitas;
using NSpec;

class describe_Entity : nspec {
    Entity _e;
    IComponent _componentA;
    IComponent _componentB;

    void fail() {
        true.should_be_false();
    }

    void before_each() {
        _e = new Entity();
        _componentA = new ComponentA();
        _componentB = new ComponentB();
    }

    void it_has_specified_creation_index() {
        const int i = 1;
        new Entity(i).creationIndex.should_be(i);
    }

    void when_entity_created() {
        it["has default creation index"] = () => {
            _e.creationIndex.should_be(0);
        };

        it["doesn't have component which hasn't been added"] = () => {
            _e.HasComponent(_componentA).should_be_false();
        };

        it["has added component"] = () => {
            _e.AddComponent(_componentA);
            _e.HasComponent(_componentA).should_be_true();
        };

        it["has component of type when component of that type was added"] = () => {
            _e.AddComponent(_componentA);
            _e.HasComponent(typeof(ComponentA)).should_be_true();
        };

        it["doesn't have component of type when no component of that type was added"] = () => {
            _e.HasComponent(typeof(ComponentA)).should_be_false();
        };

        it["doesn't have components of types when no components of these types were added"] = () => {
            _e.HasComponents(new [] { typeof(ComponentA) }).should_be_false();
        };

        it["doesn't have components of types when not all components of these types were added"] = () => {
            _e.AddComponent(_componentA);
            _e.HasComponents(new [] { typeof(ComponentA), typeof(ComponentB) }).should_be_false();
        };

        it["has components of types when all components of these types were added"] = () => {
            _e.AddComponent(_componentA);
            _e.AddComponent(_componentB);
            _e.HasComponents(new [] { typeof(ComponentA), typeof(ComponentB) }).should_be_true();
        };

        it["doesn't have any components of types when no components of these types were added"] = () => {
            _e.HasAnyComponent(new [] { typeof(ComponentA) }).should_be_false();
        };

        it["has any components of types when any component of these types was added"] = () => {
            _e.AddComponent(_componentA);
            _e.HasAnyComponent(new [] {
                typeof(ComponentA),
                typeof(ComponentB)
            }).should_be_true();
        };

        it["removes a component of type"] = () => {
            _e.AddComponent(_componentA);
            _e.RemoveComponent(typeof(ComponentA));
            _e.HasComponent(_componentA).should_be_false();
        };

        it["gets a component of type"] = () => {
            _e.AddComponent(_componentA);
            _e.GetComponent(typeof(ComponentA)).should_be_same(_componentA);
        };
    
        it["replaces existing component"] = () => {
            _e.AddComponent(_componentA);
            var newComponentA = new ComponentA();
            _e.ReplaceComponent(newComponentA);
            _e.GetComponent(typeof(ComponentA)).should_be_same(newComponentA);
        };

        it["replacing a non existing component just adds component"] = () => {
            var newComponentA = new ComponentA();
            _e.ReplaceComponent(newComponentA);
            _e.GetComponent(typeof(ComponentA)).should_be_same(newComponentA);
        };

        it["gets empty hashSet of components when no components were added"] = () => {
            _e.GetComponents().should_be_empty();
        };

        it["gets empty hashSet of component types when no components were added"] = () => {
            _e.GetComponentTypes().should_be_empty();
        };

        it["gets all components"] = () => {
            _e.AddComponent(_componentA);
            _e.AddComponent(_componentB);
            var allComponents = _e.GetComponents();
            allComponents.should_contain(_componentA);
            allComponents.should_contain(_componentB);
        };

        it["gets all component types"] = () => {
            _e.AddComponent(_componentA);
            _e.AddComponent(_componentB);
            var allComponentTypes = _e.GetComponentTypes();
            allComponentTypes.should_contain(typeof(ComponentA));
            allComponentTypes.should_contain(typeof(ComponentB));
        };

        it["removes all components"] = () => {
            _e.AddComponent(_componentA);
            _e.AddComponent(_componentB);
            _e.RemoveAllComponents();
            _e.HasComponent(typeof(ComponentA)).should_be_false();
            _e.HasComponent(typeof(ComponentB)).should_be_false();
            _e.GetComponents().should_be_empty();
            _e.GetComponentTypes().should_be_empty();
        };

        it["can ToString"] = () => {
            _e.AddComponent(_componentA);
            _e.AddComponent(_componentB);
            _e.ToString().should_be("Entity(ComponentA, ComponentB)");
        };

        context["events"] = () => {
            it["dispatches OnComponentAdded when adding a component"] = () => {
                Entity eventEntity = null;
                IComponent eventComponent = null;
                _e.OnComponentAdded += (entity, component) => {
                    eventEntity = entity;
                    eventComponent = component;
                };
                _e.AddComponent(_componentA);
                eventEntity.should_be_same(_e);
                eventComponent.should_be_same(_componentA);
            };

            it["dispatches OnComponentRemoved when removing a component"] = () => {
                Entity eventEntity = null;
                IComponent eventComponent = null;
                _e.AddComponent(_componentA);
                _e.OnComponentRemoved += (entity, component) => {
                    eventEntity = entity;
                    eventComponent = component;
                };
                _e.RemoveComponent(typeof(ComponentA));
                eventEntity.should_be_same(_e);
                eventComponent.should_be_same(_componentA);
            };

            it["dispatches OnComponentReplaced when replacing a component"] = () => {
                Entity eventEntity = null;
                IComponent eventComponent = null;
                _e.AddComponent(_componentA);
                _e.OnComponentReplaced += (entity, component) => {
                    eventEntity = entity;
                    eventComponent = component;
                };
                var newComponentA = new ComponentA();
                _e.ReplaceComponent(newComponentA);
                eventEntity.should_be_same(_e);
                eventComponent.should_be_same(newComponentA);
            };

            it["dispatches OnComponentAdded when attempting to replace a component which hasn't been added"] = () => {
                Entity eventEntity = null;
                IComponent eventComponent = null;
                _e.OnComponentReplaced += (entity, component) => fail();
                _e.OnComponentAdded += (entity, component) => {
                    eventEntity = entity;
                    eventComponent = component;
                };
                var newComponentA = new ComponentA();
                _e.ReplaceComponent(newComponentA);
                eventEntity.should_be_same(_e);
                eventComponent.should_be_same(newComponentA);
            };

            it["doesn't dispatch OnComponentAdded when replacing a component"] = () => {
                _e.AddComponent(_componentA);
                _e.OnComponentAdded += (entity, component) => fail();
                _e.ReplaceComponent(new ComponentA());
            };

            it["doesn't dispatch OnComponentRemoved when replacing a component"] = () => {
                _e.AddComponent(_componentA);
                _e.OnComponentRemoved += (entity, component) => fail();
                _e.ReplaceComponent(new ComponentA());
            };

            it["dispatches OnComponentRemoved when removing all components"] = () => {
                var dispatched = 0;
                _e.AddComponent(_componentA);
                _e.AddComponent(_componentB);
                _e.OnComponentRemoved += (entity, component) => dispatched++;
                _e.RemoveAllComponents();
                dispatched.should_be(2);
            };
        };

        context["invalid operations"] = () => {
            it["throws when adding a component of the same type twice"] = expect<EntityAlreadyHasComponentException>(() => {
                _e.AddComponent(new ComponentA());
                _e.AddComponent(new ComponentA());
            });

            it["throws when attempting to remove a component of type which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                _e.RemoveComponent(typeof(ComponentA));
            });

            it["throws when attempting to get component of type which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                _e.GetComponent(typeof(ComponentA));
            });
        };

        context["internal caching"] = () => {
            context["components"] = () => {
                it["caches components"] = () => {
                    _e.AddComponent(new ComponentA());
                    var c = _e.GetComponents();
                    _e.GetComponents().should_be_same(c);
                };

                it["updates cache when a new component was added"] = () => {
                    _e.AddComponent(new ComponentA());
                    var c = _e.GetComponents();
                    _e.AddComponent(new ComponentB());
                    _e.GetComponents().should_not_be(c);
                };

                it["updates cache when a component was removed"] = () => {
                    _e.AddComponent(new ComponentA());
                    _e.AddComponent(new ComponentB());
                    var c = _e.GetComponents();
                    _e.RemoveComponent(typeof(ComponentA));
                    _e.GetComponents().should_not_be(c);
                };

                it["updates cache when a component was replaced"] = () => {
                    _e.AddComponent(new ComponentA());
                    _e.AddComponent(new ComponentB());
                    var c = _e.GetComponents();
                    _e.ReplaceComponent(new ComponentA());
                    _e.GetComponents().should_not_be(c);
                };

                it["updates cache when all components were removed"] = () => {
                    _e.AddComponent(new ComponentA());
                    _e.AddComponent(new ComponentB());
                    var c = _e.GetComponents();
                    _e.RemoveAllComponents();
                    _e.GetComponents().should_not_be(c);
                };
            };

            context["component types"] = () => {
                it["caches component types"] = () => {
                    _e.AddComponent(new ComponentA());
                    var c = _e.GetComponentTypes();
                    _e.GetComponentTypes().should_be_same(c);
                };

                it["updates cache when a new component was added"] = () => {
                    _e.AddComponent(new ComponentA());
                    var c = _e.GetComponentTypes();
                    _e.AddComponent(new ComponentB());
                    _e.GetComponentTypes().should_not_be(c);
                };

                it["updates cache when a component was removed"] = () => {
                    _e.AddComponent(new ComponentA());
                    _e.AddComponent(new ComponentB());
                    var c = _e.GetComponentTypes();
                    _e.RemoveComponent(typeof(ComponentA));
                    _e.GetComponentTypes().should_not_be(c);
                };

                it["doesn't update cache when a component was replaced"] = () => {
                    _e.AddComponent(new ComponentA());
                    _e.AddComponent(new ComponentB());
                    var c = _e.GetComponentTypes();
                    _e.ReplaceComponent(new ComponentA());
                    _e.GetComponentTypes().should_be(c);
                };

                it["updates cache when adding a new component with ReplaceComponent"] = () => {
                    _e.AddComponent(new ComponentA());
                    _e.AddComponent(new ComponentB());
                    var c = _e.GetComponentTypes();
                    _e.ReplaceComponent(new ComponentC());
                    _e.GetComponentTypes().should_not_be(c);
                };

                it["updates cache when all components were removed"] = () => {
                    _e.AddComponent(new ComponentA());
                    _e.AddComponent(new ComponentB());
                    var c = _e.GetComponentTypes();
                    _e.RemoveAllComponents();
                    _e.GetComponentTypes().should_not_be(c);
                };
            };
        };
    }
}

