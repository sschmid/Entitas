using System.Collections.Generic;
using Entitas;
using My.Namespace;
using NSpec;

class describe_Entity : nspec {

    readonly int[] _indicesA = { CID.ComponentA };
    readonly int[] _indicesAB = { CID.ComponentA, CID.ComponentB };

    void assertHasComponentA(TestEntity e, IComponent componentA = null) {
        if (componentA == null) {
            componentA = Component.A;
        }

        e.GetComponentA().should_be_same(componentA);

        var components = e.GetComponents();
        components.Length.should_be(1);
        components.should_contain(componentA);

        var indices = e.GetComponentIndices();
        indices.Length.should_be(1);
        indices.should_contain(CID.ComponentA);

        e.HasComponentA().should_be_true();
        e.HasComponents(_indicesA).should_be_true();
        e.HasAnyComponent(_indicesA).should_be_true();
    }

    void assertHasNotComponentA(TestEntity e) {
        var components = e.GetComponents();
        components.Length.should_be(0);

        var indices = e.GetComponentIndices();
        indices.Length.should_be(0);

        e.HasComponentA().should_be_false();
        e.HasComponents(_indicesA).should_be_false();
        e.HasAnyComponent(_indicesA).should_be_false();
    }

    void when_created() {

        TestEntity e = null;
        before = () => {
            e = this.CreateEntity();
        };

        context["initial state"] = () => {

            it["has default ContextInfo"] = () => {
                e.contextInfo.name.should_be("No Context");
                e.contextInfo.componentNames.Length.should_be(CID.TotalComponents);
                e.contextInfo.componentTypes.should_be_null();
                for (int i = 0; i < e.contextInfo.componentNames.Length; i++) {
                    e.contextInfo.componentNames[i].should_be(i.ToString());
                }
            };

            it["initializes"] = () => {
                var contextInfo = new ContextInfo(null, null, null);
                var componentPools = new Stack<IComponent>[42];
                e = new TestEntity();
                e.Initialize(1, 2, componentPools, contextInfo);

                e.isEnabled.should_be_true();
                e.creationIndex.should_be(1);
                e.totalComponents.should_be(2);
                e.componentPools.should_be_same(componentPools);
                e.contextInfo.should_be_same(contextInfo);
            };

            it["reactivates after being desroyed"] = () => {
                var contextInfo = new ContextInfo(null, null, null);
                var componentPools = new Stack<IComponent>[42];
                e = new TestEntity();
                e.Initialize(1, 2, componentPools, contextInfo);

                e.InternalDestroy();

                e.Reactivate(42);

                e.isEnabled.should_be_true();
                e.creationIndex.should_be(42);
                e.totalComponents.should_be(2);
                e.componentPools.should_be_same(componentPools);
                e.contextInfo.should_be_same(contextInfo);
            };

            it["throws when attempting to get component at index which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                e.GetComponentA();
            });

            it["gets total components count when empty"] = () => {
                e.totalComponents.should_be(CID.TotalComponents);
            };

            it["gets empty array of components when no components were added"] = () => {
                e.GetComponents().should_be_empty();
            };

            it["gets empty array of component indices when no components were added"] = () => {
                e.GetComponentIndices().should_be_empty();
            };

            it["doesn't have component at index when no component was added"] = () => {
                e.HasComponentA().should_be_false();
            };

            it["doesn't have components at indices when no components were added"] = () => {
                e.HasComponents(_indicesA).should_be_false();
            };

            it["doesn't have any components at indices when no components were added"] = () => {
                e.HasAnyComponent(_indicesA).should_be_false();
            };

            it["adds a component"] = () => {
                e.AddComponentA();
                assertHasComponentA(e);
            };

            it["throws when attempting to remove a component at index which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                e.RemoveComponentA();
            });

            it["replacing a non existing component adds component"] = () => {
                e.ReplaceComponentA(Component.A);
                assertHasComponentA(e);
            };
        };

        context["when component added"] = () => {

            before = () => {
                e.AddComponentA();
            };

            it["throws when adding a component at the same index twice"] = expect<EntityAlreadyHasComponentException>(() => {
                e.AddComponentA();
            });

            it["removes a component at index"] = () => {
                e.RemoveComponentA();
                assertHasNotComponentA(e);
            };

            it["replaces existing component"] = () => {
                var newComponentA = new ComponentA();
                e.ReplaceComponentA(newComponentA);
                assertHasComponentA(e, newComponentA);
            };

            it["doesn't have components at indices when not all components were added"] = () => {
                e.HasComponents(_indicesAB).should_be_false();
            };

            it["has any components at indices when any component was added"] = () => {
                e.HasAnyComponent(_indicesAB).should_be_true();
            };

            context["when adding another component"] = () => {

                before = () => {
                    e.AddComponentB();
                };

                it["gets all components"] = () => {
                    var components = e.GetComponents();
                    components.Length.should_be(2);
                    components.should_contain(Component.A);
                    components.should_contain(Component.B);
                };

                it["gets all component indices"] = () => {
                    var componentIndices = e.GetComponentIndices();
                    componentIndices.Length.should_be(2);
                    componentIndices.should_contain(CID.ComponentA);
                    componentIndices.should_contain(CID.ComponentB);
                };

                it["has other component"] = () => {
                    e.HasComponentB().should_be_true();
                };

                it["has components at indices when all components were added"] = () => {
                    e.HasComponents(_indicesAB).should_be_true();
                };

                it["removes all components"] = () => {
                    e.RemoveAllComponents();
                    e.HasComponentA().should_be_false();
                    e.HasComponentB().should_be_false();
                    e.GetComponents().should_be_empty();
                    e.GetComponentIndices().should_be_empty();
                };

                it["can ToString and doesn't removes *Component suffix"] = () => {
                    e.AddComponent(0, new StandardComponent());
                    e.Retain(this);
                    e.ToString().should_be("Entity_0(StandardComponent, ComponentA, ComponentB)");
                };

                it["uses component.ToString()"] = () => {
                    e.AddComponent(0, new NameAgeComponent { name = "Max", age = 42 });
                    e.ToString().should_be("Entity_0(NameAge(Max, 42), ComponentA, ComponentB)");
                };

                it["uses full component name with namespace if ToString is not implemented"] = () => {
                    e.AddComponent(0, new MyNamespaceComponent());
                    e.ToString().should_be("Entity_0(My.Namespace.MyNamespaceComponent, ComponentA, ComponentB)");
                };
            };
        };

        context["componentPool"] = () => {

            it["gets component context"] = () => {
                var componentPool = e.GetComponentPool(CID.ComponentA);
                componentPool.Count.should_be(0);
            };

            it["gets same component context instance"] = () => {
                e.GetComponentPool(CID.ComponentA).should_be_same(e.GetComponentPool(CID.ComponentA));
            };

            it["pushes component to componentPool when removed"] = () => {
                e.AddComponentA();
                var component = e.GetComponentA();
                e.RemoveComponentA();

                var componentPool = e.GetComponentPool(CID.ComponentA);
                componentPool.Count.should_be(1);
                componentPool.Pop().should_be_same(component);
            };

            it["creates new component when componentPool is empty"] = () => {
                var type = typeof(NameAgeComponent);
                var component = e.CreateComponent(1, type);
                component.GetType().should_be(type);

                var nameAgeComponent = ((NameAgeComponent)component);
                nameAgeComponent.name.should_be_null();
                nameAgeComponent.age.should_be(0);
            };

            it["gets pooled component when componentPool is not empty"] = () => {
                var component = new NameAgeComponent();
                e.AddComponent(1, component);

                e.RemoveComponent(1);

                var newComponent = (NameAgeComponent)e.CreateComponent(1, typeof(NameAgeComponent));
                newComponent.should_be_same(component);
            };
        };

        context["events"] = () => {

            int didDispatch = 0;

            before = () => {
                didDispatch = 0;
            };

            it["dispatches OnComponentAdded when adding a component"] = () => {
                e.OnComponentAdded += (entity, index, component) => {
                    didDispatch += 1;
                    entity.should_be_same(e);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(Component.A);
                };
                e.OnComponentRemoved += delegate { this.Fail(); };
                e.OnComponentReplaced += delegate { this.Fail(); };

                e.AddComponentA();
                didDispatch.should_be(1);
            };

            it["dispatches OnComponentRemoved when removing a component"] = () => {
                e.AddComponentA();

                e.OnComponentRemoved += (entity, index, component) => {
                    didDispatch += 1;
                    entity.should_be_same(e);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(Component.A);
                };
                e.OnComponentAdded += delegate { this.Fail(); };
                e.OnComponentReplaced += delegate { this.Fail(); };

                e.RemoveComponentA();
                didDispatch.should_be(1);
            };

            it["dispatches OnComponentRemoved before pushing component to context"] = () => {
                e.AddComponentA();

                e.OnComponentRemoved += (entity, index, component) => {
                    var newComponent = entity.CreateComponent(index, component.GetType());
                    component.should_not_be_same(newComponent);
                };

                e.RemoveComponentA();
            };

            it["dispatches OnComponentReplaced when replacing a component"] = () => {
                e.AddComponentA();
                var newComponentA = new ComponentA();

                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => {
                    didDispatch += 1;
                    entity.should_be_same(e);
                    index.should_be(CID.ComponentA);
                    previousComponent.should_be_same(Component.A);
                    newComponent.should_be_same(newComponentA);
                };
                e.OnComponentAdded += delegate { this.Fail(); };
                e.OnComponentRemoved += delegate { this.Fail(); };

                e.ReplaceComponentA(newComponentA);
                didDispatch.should_be(1);
            };

            it["provides previous and new component OnComponentReplaced when replacing with different component"] = () => {
                var prevComp = new ComponentA();
                var newComp = new ComponentA();

                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => {
                    didDispatch += 1;
                    entity.should_be_same(e);
                    previousComponent.should_be_same(prevComp);
                    newComponent.should_be_same(newComp);
                };

                e.AddComponent(CID.ComponentA, prevComp);
                e.ReplaceComponent(CID.ComponentA, newComp);
                didDispatch.should_be(1);
            };

            it["provides previous and new component OnComponentReplaced when replacing with same component"] = () => {
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => {
                    didDispatch += 1;
                    entity.should_be_same(e);
                    previousComponent.should_be_same(Component.A);
                    newComponent.should_be_same(Component.A);
                };

                e.AddComponentA();
                e.ReplaceComponentA(Component.A);
                didDispatch.should_be(1);
            };

            it["doesn't dispatch anything when replacing a non existing component with null"] = () => {
                e.OnComponentAdded += delegate { this.Fail(); };
                e.OnComponentReplaced += delegate { this.Fail(); };
                e.OnComponentRemoved += delegate { this.Fail(); };

                e.ReplaceComponentA(null);
            };

            it["dispatches OnComponentAdded when attempting to replace a component which hasn't been added"] = () => {
                var newComponentA = new ComponentA();

                e.OnComponentAdded += (entity, index, component) => {
                    didDispatch += 1;
                    entity.should_be_same(e);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(newComponentA);
                };
                e.OnComponentReplaced += delegate { this.Fail(); };
                e.OnComponentRemoved += delegate { this.Fail(); };

                e.ReplaceComponentA(newComponentA);
                didDispatch.should_be(1);
            };

            it["dispatches OnComponentRemoved when replacing a component with null"] = () => {
                e.AddComponentA();

                e.OnComponentRemoved += (entity, index, component) => {
                    didDispatch += 1;
                    component.should_be_same(Component.A);
                };
                e.OnComponentAdded += delegate { this.Fail(); };
                e.OnComponentReplaced += delegate { this.Fail(); };

                e.ReplaceComponentA(null);
                didDispatch.should_be(1);
            };

            it["dispatches OnComponentRemoved when removing all components"] = () => {
                e.AddComponentA();
                e.AddComponentB();
                e.OnComponentRemoved += (entity, index, component) => didDispatch += 1;
                e.RemoveAllComponents();
                didDispatch.should_be(2);
            };

            it["dispatches OnDestroy when calling Destroy"] = () => {
                var didDestroy = 0;
                e.OnDestroyEntity += entity => didDestroy += 1;
                e.Destroy();
                didDestroy.should_be(1);
            };
        };

        context["reference counting"] = () => {

            it["retains entity"] = () => {
                e.retainCount.should_be(0);
                e.Retain(this);
                e.retainCount.should_be(1);

                var safeAerc = e.aerc as SafeAERC;
                if (safeAerc != null) {
                    safeAerc.owners.should_contain(this);
                }
            };

            it["releases entity"] = () => {
                e.Retain(this);
                e.Release(this);
                e.retainCount.should_be(0);

                var safeAerc = e.aerc as SafeAERC;
                if (safeAerc != null) {
                    safeAerc.owners.should_not_contain(this);
                }
            };

            it["throws when releasing more than it has been retained"] = expect<EntityIsNotRetainedByOwnerException>(() => {
                e.Retain(this);
                e.Release(this);
                e.Release(this);
            });

            it["throws when retaining twice with same owner"] = expect<EntityIsAlreadyRetainedByOwnerException>(() => {
                var owner1 = new object();
                e.Retain(owner1);
                e.Retain(owner1);
            });

            it["throws when releasing with unknown owner"] = expect<EntityIsNotRetainedByOwnerException>(() => {
                var owner = new object();
                var unknownOwner = new object();
                e.Retain(owner);
                e.Release(unknownOwner);
            });

            it["throws when releasing with owner which doesn't retain entity anymore"] = expect<EntityIsNotRetainedByOwnerException>(() => {
                var owner1 = new object();
                var owner2 = new object();
                e.Retain(owner1);
                e.Retain(owner2);
                e.Release(owner2);
                e.Release(owner2);
            });

            context["events"] = () => {

                it["doesn't dispatch OnEntityReleased when retaining"] = () => {
                    e.OnEntityReleased += delegate { this.Fail(); };
                    e.Retain(this);
                };

                it["dispatches OnEntityReleased when retain and release"] = () => {
                    var didDispatch = 0;

                    e.OnEntityReleased += entity => {
                        didDispatch += 1;
                        entity.should_be_same(e);
                    };

                    e.Retain(this);
                    e.Release(this);

                    didDispatch.should_be(1);
                };
            };
        };

        context["internal caching"] = () => {

            context["components"] = () => {

                IComponent[] cache = null;

                before = () => {
                    e.AddComponentA();
                    cache = e.GetComponents();
                };

                it["caches components"] = () => {
                    e.GetComponents().should_be_same(cache);
                };

                it["updates cache when a new component was added"] = () => {
                    e.AddComponentB();
                    e.GetComponents().should_not_be_same(cache);
                };

                it["updates cache when a component was removed"] = () => {
                    e.RemoveComponentA();
                    e.GetComponents().should_not_be_same(cache);
                };

                it["updates cache when a component was replaced"] = () => {
                    e.ReplaceComponentA(new ComponentA());
                    e.GetComponents().should_not_be_same(cache);
                };

                it["doesn't update cache when a component was replaced with same component"] = () => {
                    e.ReplaceComponentA(Component.A);
                    e.GetComponents().should_be_same(cache);
                };

                it["updates cache when all components were removed"] = () => {
                    e.RemoveAllComponents();
                    e.GetComponents().should_not_be_same(cache);
                };
            };

            context["component indices"] = () => {

                int[] cache = null;

                before = () => {
                    e.AddComponentA();
                    cache = e.GetComponentIndices();
                };

                it["caches component indices"] = () => {
                    e.GetComponentIndices().should_be_same(cache);
                };

                it["updates cache when a new component was added"] = () => {
                    e.AddComponentB();
                    e.GetComponentIndices().should_not_be_same(cache);
                };

                it["updates cache when a component was removed"] = () => {
                    e.RemoveComponentA();
                    e.GetComponentIndices().should_not_be_same(cache);
                };

                it["doesn't update cache when a component was replaced"] = () => {
                    e.ReplaceComponentA(new ComponentA());
                    e.GetComponentIndices().should_be_same(cache);
                };

                it["updates cache when adding a new component with ReplaceComponent"] = () => {
                    e.ReplaceComponentC(Component.C);
                    e.GetComponentIndices().should_not_be_same(cache);
                };

                it["updates cache when all components were removed"] = () => {
                    e.RemoveAllComponents();
                    e.GetComponentIndices().should_not_be_same(cache);
                };
            };

            context["ToString"] = () => {

                context["when component was added"] = () => {

                    string cache = null;

                    before = () => {
                        e.AddComponentA();
                        cache = e.ToString();
                    };

                    it["caches entity description"] = () => {
                        e.ToString().should_be_same(cache);
                    };

                    it["updates cache when a new component was added"] = () => {
                        e.AddComponentB();
                        e.ToString().should_not_be_same(cache);
                    };

                    it["updates cache when a component was removed"] = () => {
                        e.RemoveComponentA();
                        e.ToString().should_not_be_same(cache);
                    };

                    it["doesn't update cache when a component was replaced"] = () => {
                        e.ReplaceComponentA(new ComponentA());
                        e.ToString().should_be_same(cache);
                    };

                    it["updates cache when all components were removed"] = () => {
                        e.RemoveAllComponents();
                        e.ToString().should_not_be_same(cache);
                    };

                    it["doesn't update cache when entity gets retained"] = () => {
                        e.Retain(this);
                        e.ToString().should_be_same(cache);
                    };

                    it["doesn't update cache when entity gets released"] = () => {
                        e.Retain(this);
                        e.Retain(new object());
                        cache = e.ToString();
                        e.Release(this);
                        e.ToString().should_be_same(cache);
                    };

                    it["released entity doesn't have updated cache"] = () => {
                        e.Retain(this);
                        cache = e.ToString();
                        e.OnEntityReleased += entity => {
                            e.ToString().should_be_same(cache);
                        };
                        e.Release(this);
                    };

                    it["updates cache when RemoveAllComponents is called, even if entity has no components"] = () => {
                        cache = e.ToString();
                        e.RemoveAllComponents();
                        e.ToString().should_not_be_same(cache);
                    };
                };
            };
        };
    }
}
