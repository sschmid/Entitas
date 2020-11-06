using System.Collections.Generic;
using Entitas;
using My.Namespace;
using NSpec;
using Shouldly;

class describe_Entity : nspec {

    readonly int[] _indicesA = { CID.ComponentA };
    readonly int[] _indicesAB = { CID.ComponentA, CID.ComponentB };

    void assertHasComponentA(TestEntity e, IComponent componentA = null) {
        if (componentA == null) {
            componentA = Component.A;
        }

        e.GetComponentA().ShouldBeSameAs(componentA);

        var components = e.GetComponents();
        components.Length.ShouldBe(1);
        components.ShouldContain(componentA);

        var indices = e.GetComponentIndices();
        indices.Length.ShouldBe(1);
        indices.ShouldContain(CID.ComponentA);

        e.HasComponentA().ShouldBeTrue();
        e.HasComponents(_indicesA).ShouldBeTrue();
        e.HasAnyComponent(_indicesA).ShouldBeTrue();
    }

    void assertHasNotComponentA(TestEntity e) {
        var components = e.GetComponents();
        components.Length.ShouldBe(0);

        var indices = e.GetComponentIndices();
        indices.Length.ShouldBe(0);

        e.HasComponentA().ShouldBeFalse();
        e.HasComponents(_indicesA).ShouldBeFalse();
        e.HasAnyComponent(_indicesA).ShouldBeFalse();
    }

    void when_created() {

        TestEntity e = null;
        before = () => {
            e = this.CreateEntity();
        };

        context["initial state"] = () => {

            it["has default ContextInfo"] = () => {
                e.contextInfo.name.ShouldBe("No Context");
                e.contextInfo.componentNames.Length.ShouldBe(CID.TotalComponents);
                e.contextInfo.componentTypes.ShouldBeNull();
                for (int i = 0; i < e.contextInfo.componentNames.Length; i++) {
                    e.contextInfo.componentNames[i].ShouldBe(i.ToString());
                }
            };

            it["initializes"] = () => {
                var contextInfo = new ContextInfo(null, null, null);
                var componentPools = new Stack<IComponent>[42];
                e = new TestEntity();
                e.Initialize(1, 2, componentPools, contextInfo);

                e.isEnabled.ShouldBeTrue();
                e.creationIndex.ShouldBe(1);
                e.totalComponents.ShouldBe(2);
                e.componentPools.ShouldBeSameAs(componentPools);
                e.contextInfo.ShouldBeSameAs(contextInfo);
            };

            it["reactivates after being desroyed"] = () => {
                var contextInfo = new ContextInfo(null, null, null);
                var componentPools = new Stack<IComponent>[42];
                e = new TestEntity();
                e.Initialize(1, 2, componentPools, contextInfo);

                e.InternalDestroy();

                e.Reactivate(42);

                e.isEnabled.ShouldBeTrue();
                e.creationIndex.ShouldBe(42);
                e.totalComponents.ShouldBe(2);
                e.componentPools.ShouldBeSameAs(componentPools);
                e.contextInfo.ShouldBeSameAs(contextInfo);
            };

            it["throws when attempting to get component at index which hasn't been added"] = expect<EntityDoesNotHaveComponentException>(() => {
                e.GetComponentA();
            });

            it["gets total components count when empty"] = () => {
                e.totalComponents.ShouldBe(CID.TotalComponents);
            };

            it["gets empty array of components when no components were added"] = () => {
                e.GetComponents().ShouldBeEmpty();
            };

            it["gets empty array of component indices when no components were added"] = () => {
                e.GetComponentIndices().ShouldBeEmpty();
            };

            it["doesn't have component at index when no component was added"] = () => {
                e.HasComponentA().ShouldBeFalse();
            };

            it["doesn't have components at indices when no components were added"] = () => {
                e.HasComponents(_indicesA).ShouldBeFalse();
            };

            it["doesn't have any components at indices when no components were added"] = () => {
                e.HasAnyComponent(_indicesA).ShouldBeFalse();
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
                e.HasComponents(_indicesAB).ShouldBeFalse();
            };

            it["has any components at indices when any component was added"] = () => {
                e.HasAnyComponent(_indicesAB).ShouldBeTrue();
            };

            context["when adding another component"] = () => {

                before = () => {
                    e.AddComponentB();
                };

                it["gets all components"] = () => {
                    var components = e.GetComponents();
                    components.Length.ShouldBe(2);
                    components.ShouldContain(Component.A);
                    components.ShouldContain(Component.B);
                };

                it["gets all component indices"] = () => {
                    var componentIndices = e.GetComponentIndices();
                    componentIndices.Length.ShouldBe(2);
                    componentIndices.ShouldContain(CID.ComponentA);
                    componentIndices.ShouldContain(CID.ComponentB);
                };

                it["has other component"] = () => {
                    e.HasComponentB().ShouldBeTrue();
                };

                it["has components at indices when all components were added"] = () => {
                    e.HasComponents(_indicesAB).ShouldBeTrue();
                };

                it["removes all components"] = () => {
                    e.RemoveAllComponents();
                    e.HasComponentA().ShouldBeFalse();
                    e.HasComponentB().ShouldBeFalse();
                    e.GetComponents().ShouldBeEmpty();
                    e.GetComponentIndices().ShouldBeEmpty();
                };

                it["can ToString and doesn't removes *Component suffix"] = () => {
                    e.AddComponent(0, new StandardComponent());
                    e.Retain(this);
                    e.ToString().ShouldBe("Entity_0(StandardComponent, ComponentA, ComponentB)");
                };

                it["uses component.ToString()"] = () => {
                    e.AddComponent(0, new NameAgeComponent { name = "Max", age = 42 });
                    e.ToString().ShouldBe("Entity_0(NameAge(Max, 42), ComponentA, ComponentB)");
                };

                it["uses full component name with namespace if ToString is not implemented"] = () => {
                    e.AddComponent(0, new MyNamespaceComponent());
                    e.ToString().ShouldBe("Entity_0(My.Namespace.MyNamespaceComponent, ComponentA, ComponentB)");
                };
            };
        };

        context["componentPool"] = () => {

            it["gets component context"] = () => {
                var componentPool = e.GetComponentPool(CID.ComponentA);
                componentPool.Count.ShouldBe(0);
            };

            it["gets same component context instance"] = () => {
                e.GetComponentPool(CID.ComponentA).ShouldBeSameAs(e.GetComponentPool(CID.ComponentA));
            };

            it["pushes component to componentPool when removed"] = () => {
                e.AddComponentA();
                var component = e.GetComponentA();
                e.RemoveComponentA();

                var componentPool = e.GetComponentPool(CID.ComponentA);
                componentPool.Count.ShouldBe(1);
                componentPool.Pop().ShouldBeSameAs(component);
            };

            it["creates new component when componentPool is empty"] = () => {
                var type = typeof(NameAgeComponent);
                var component = e.CreateComponent(1, type);
                component.GetType().ShouldBe(type);

                var nameAgeComponent = ((NameAgeComponent)component);
                nameAgeComponent.name.ShouldBeNull();
                nameAgeComponent.age.ShouldBe(0);
            };

            it["gets pooled component when componentPool is not empty"] = () => {
                var component = new NameAgeComponent();
                e.AddComponent(1, component);

                e.RemoveComponent(1);

                var newComponent = (NameAgeComponent)e.CreateComponent(1, typeof(NameAgeComponent));
                newComponent.ShouldBeSameAs(component);
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
                    entity.ShouldBeSameAs(e);
                    index.ShouldBe(CID.ComponentA);
                    component.ShouldBeSameAs(Component.A);
                };
                e.OnComponentRemoved += delegate { this.Fail(); };
                e.OnComponentReplaced += delegate { this.Fail(); };

                e.AddComponentA();
                didDispatch.ShouldBe(1);
            };

            it["dispatches OnComponentRemoved when removing a component"] = () => {
                e.AddComponentA();

                e.OnComponentRemoved += (entity, index, component) => {
                    didDispatch += 1;
                    entity.ShouldBeSameAs(e);
                    index.ShouldBe(CID.ComponentA);
                    component.ShouldBeSameAs(Component.A);
                };
                e.OnComponentAdded += delegate { this.Fail(); };
                e.OnComponentReplaced += delegate { this.Fail(); };

                e.RemoveComponentA();
                didDispatch.ShouldBe(1);
            };

            it["dispatches OnComponentRemoved before pushing component to context"] = () => {
                e.AddComponentA();

                e.OnComponentRemoved += (entity, index, component) => {
                    var newComponent = entity.CreateComponent(index, component.GetType());
                    component.ShouldNotBeSameAs(newComponent);
                };

                e.RemoveComponentA();
            };

            it["dispatches OnComponentReplaced when replacing a component"] = () => {
                e.AddComponentA();
                var newComponentA = new ComponentA();

                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => {
                    didDispatch += 1;
                    entity.ShouldBeSameAs(e);
                    index.ShouldBe(CID.ComponentA);
                    previousComponent.ShouldBeSameAs(Component.A);
                    newComponent.ShouldBeSameAs(newComponentA);
                };
                e.OnComponentAdded += delegate { this.Fail(); };
                e.OnComponentRemoved += delegate { this.Fail(); };

                e.ReplaceComponentA(newComponentA);
                didDispatch.ShouldBe(1);
            };

            it["provides previous and new component OnComponentReplaced when replacing with different component"] = () => {
                var prevComp = new ComponentA();
                var newComp = new ComponentA();

                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => {
                    didDispatch += 1;
                    entity.ShouldBeSameAs(e);
                    previousComponent.ShouldBeSameAs(prevComp);
                    newComponent.ShouldBeSameAs(newComp);
                };

                e.AddComponent(CID.ComponentA, prevComp);
                e.ReplaceComponent(CID.ComponentA, newComp);
                didDispatch.ShouldBe(1);
            };

            it["provides previous and new component OnComponentReplaced when replacing with same component"] = () => {
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => {
                    didDispatch += 1;
                    entity.ShouldBeSameAs(e);
                    previousComponent.ShouldBeSameAs(Component.A);
                    newComponent.ShouldBeSameAs(Component.A);
                };

                e.AddComponentA();
                e.ReplaceComponentA(Component.A);
                didDispatch.ShouldBe(1);
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
                    entity.ShouldBeSameAs(e);
                    index.ShouldBe(CID.ComponentA);
                    component.ShouldBeSameAs(newComponentA);
                };
                e.OnComponentReplaced += delegate { this.Fail(); };
                e.OnComponentRemoved += delegate { this.Fail(); };

                e.ReplaceComponentA(newComponentA);
                didDispatch.ShouldBe(1);
            };

            it["dispatches OnComponentRemoved when replacing a component with null"] = () => {
                e.AddComponentA();

                e.OnComponentRemoved += (entity, index, component) => {
                    didDispatch += 1;
                    component.ShouldBeSameAs(Component.A);
                };
                e.OnComponentAdded += delegate { this.Fail(); };
                e.OnComponentReplaced += delegate { this.Fail(); };

                e.ReplaceComponentA(null);
                didDispatch.ShouldBe(1);
            };

            it["dispatches OnComponentRemoved when removing all components"] = () => {
                e.AddComponentA();
                e.AddComponentB();
                e.OnComponentRemoved += (entity, index, component) => didDispatch += 1;
                e.RemoveAllComponents();
                didDispatch.ShouldBe(2);
            };

            it["dispatches OnDestroy when calling Destroy"] = () => {
                var didDestroy = 0;
                e.OnDestroyEntity += entity => didDestroy += 1;
                e.Destroy();
                didDestroy.ShouldBe(1);
            };
        };

        context["reference counting"] = () => {

            it["retains entity"] = () => {
                e.retainCount.ShouldBe(0);
                e.Retain(this);
                e.retainCount.ShouldBe(1);

                var safeAerc = e.aerc as SafeAERC;
                if (safeAerc != null) {
                    safeAerc.owners.ShouldContain(this);
                }
            };

            it["releases entity"] = () => {
                e.Retain(this);
                e.Release(this);
                e.retainCount.ShouldBe(0);

                var safeAerc = e.aerc as SafeAERC;
                if (safeAerc != null) {
                    safeAerc.owners.ShouldNotContain(this);
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
                        entity.ShouldBeSameAs(e);
                    };

                    e.Retain(this);
                    e.Release(this);

                    didDispatch.ShouldBe(1);
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
                    e.GetComponents().ShouldBeSameAs(cache);
                };

                it["updates cache when a new component was added"] = () => {
                    e.AddComponentB();
                    e.GetComponents().ShouldNotBeSameAs(cache);
                };

                it["updates cache when a component was removed"] = () => {
                    e.RemoveComponentA();
                    e.GetComponents().ShouldNotBeSameAs(cache);
                };

                it["updates cache when a component was replaced"] = () => {
                    e.ReplaceComponentA(new ComponentA());
                    e.GetComponents().ShouldNotBeSameAs(cache);
                };

                it["doesn't update cache when a component was replaced with same component"] = () => {
                    e.ReplaceComponentA(Component.A);
                    e.GetComponents().ShouldBeSameAs(cache);
                };

                it["updates cache when all components were removed"] = () => {
                    e.RemoveAllComponents();
                    e.GetComponents().ShouldNotBeSameAs(cache);
                };
            };

            context["component indices"] = () => {

                int[] cache = null;

                before = () => {
                    e.AddComponentA();
                    cache = e.GetComponentIndices();
                };

                it["caches component indices"] = () => {
                    e.GetComponentIndices().ShouldBeSameAs(cache);
                };

                it["updates cache when a new component was added"] = () => {
                    e.AddComponentB();
                    e.GetComponentIndices().ShouldNotBeSameAs(cache);
                };

                it["updates cache when a component was removed"] = () => {
                    e.RemoveComponentA();
                    e.GetComponentIndices().ShouldNotBeSameAs(cache);
                };

                it["doesn't update cache when a component was replaced"] = () => {
                    e.ReplaceComponentA(new ComponentA());
                    e.GetComponentIndices().ShouldBeSameAs(cache);
                };

                it["updates cache when adding a new component with ReplaceComponent"] = () => {
                    e.ReplaceComponentC(Component.C);
                    e.GetComponentIndices().ShouldNotBeSameAs(cache);
                };

                it["updates cache when all components were removed"] = () => {
                    e.RemoveAllComponents();
                    e.GetComponentIndices().ShouldNotBeSameAs(cache);
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
                        e.ToString().ShouldBeSameAs(cache);
                    };

                    it["updates cache when a new component was added"] = () => {
                        e.AddComponentB();
                        e.ToString().ShouldNotBeSameAs(cache);
                    };

                    it["updates cache when a component was removed"] = () => {
                        e.RemoveComponentA();
                        e.ToString().ShouldNotBeSameAs(cache);
                    };

                    it["doesn't update cache when a component was replaced"] = () => {
                        e.ReplaceComponentA(new ComponentA());
                        e.ToString().ShouldBeSameAs(cache);
                    };

                    it["updates cache when all components were removed"] = () => {
                        e.RemoveAllComponents();
                        e.ToString().ShouldNotBeSameAs(cache);
                    };

                    it["doesn't update cache when entity gets retained"] = () => {
                        e.Retain(this);
                        e.ToString().ShouldBeSameAs(cache);
                    };

                    it["doesn't update cache when entity gets released"] = () => {
                        e.Retain(this);
                        e.Retain(new object());
                        cache = e.ToString();
                        e.Release(this);
                        e.ToString().ShouldBeSameAs(cache);
                    };

                    it["released entity doesn't have updated cache"] = () => {
                        e.Retain(this);
                        cache = e.ToString();
                        e.OnEntityReleased += entity => {
                            e.ToString().ShouldBeSameAs(cache);
                        };
                        e.Release(this);
                    };

                    it["updates cache when RemoveAllComponents is called, even if entity has no components"] = () => {
                        cache = e.ToString();
                        e.RemoveAllComponents();
                        e.ToString().ShouldNotBeSameAs(cache);
                    };
                };
            };
        };
    }
}
