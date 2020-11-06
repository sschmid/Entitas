using Entitas;
using NSpec;
using Shouldly;

class describe_Context : nspec {

    void when_created() {

        IContext<TestEntity> ctx = null;

        before = () => {
            ctx = new MyTestContext();
        };

        it["increments creationIndex"] = () => {
            ctx.CreateEntity().creationIndex.ShouldBe(0);
            ctx.CreateEntity().creationIndex.ShouldBe(1);
        };

        it["starts with given creationIndex"] = () => {
            new MyTestContext(CID.TotalComponents, 42, null).CreateEntity().creationIndex.ShouldBe(42);
        };

        it["has no entities when no entities were created"] = () => {
            ctx.GetEntities().ShouldBeEmpty();
        };

        it["gets total entity count"] = () => {
            ctx.count.ShouldBe(0);
        };

        it["creates entity"] = () => {
            var e = ctx.CreateEntity();
            e.ShouldNotBeNull();
            e.GetType().ShouldBe(typeof(TestEntity));

            e.totalComponents.ShouldBe(ctx.totalComponents);
            e.isEnabled.ShouldBeTrue();
        };

        it["has default ContextInfo"] = () => {
            ctx.contextInfo.name.ShouldBe("Unnamed Context");
            ctx.contextInfo.componentNames.Length.ShouldBe(CID.TotalComponents);
            for (int i = 0; i < ctx.contextInfo.componentNames.Length; i++) {
                ctx.contextInfo.componentNames[i].ShouldBe("Index " + i);
            }
        };

        it["creates component pools"] = () => {
            ctx.componentPools.ShouldNotBeNull();
            ctx.componentPools.Length.ShouldBe(CID.TotalComponents);
        };

        it["creates entity with component pools"] = () => {
            var e = ctx.CreateEntity();
            e.componentPools.ShouldBeSameAs(ctx.componentPools);
        };

        it["can ToString"] = () => {
            ctx.ToString().ShouldBe("Unnamed Context");
        };

        context["when ContextInfo set"] = () => {

            ContextInfo contextInfo = null;

            before = () => {
                var componentNames = new [] { "Health", "Position", "View" };
                var componentTypes = new [] { typeof(ComponentA), typeof(ComponentB), typeof(ComponentC) };
                contextInfo = new ContextInfo("My Context", componentNames, componentTypes);
                ctx = new MyTestContext(componentNames.Length, 0, contextInfo);
            };

            it["has custom ContextInfo"] = () => {
                ctx.contextInfo.ShouldBeSameAs(contextInfo);
            };

            it["creates entity with same ContextInfo"] = () => {
                ctx.CreateEntity().contextInfo.ShouldBeSameAs(contextInfo);
            };

            it["throws when componentNames is not same length as totalComponents"] = expect<ContextInfoException>(() => {
                new MyTestContext(contextInfo.componentNames.Length + 1, 0, contextInfo);
            });
        };

        context["when entity created"] = () => {

            TestEntity e = null;

            before = () => {
                e = ctx.CreateEntity();
                e.AddComponentA();
            };

            it["gets total entity count"] = () => {
                ctx.count.ShouldBe(1);
            };

            it["has entities that were created with CreateEntity()"] = () => {
                ctx.HasEntity(e).ShouldBeTrue();
            };

            it["doesn't have entities that were not created with CreateEntity()"] = () => {
                ctx.HasEntity(this.CreateEntity()).ShouldBeFalse();
            };

            it["returns all created entities"] = () => {
                var e2 = ctx.CreateEntity();
                var entities = ctx.GetEntities();
                entities.Length.ShouldBe(2);
                entities.ShouldContain(e);
                entities.ShouldContain(e2);
            };

            it["destroys entity and removes it"] = () => {
                e.Destroy();
                ctx.HasEntity(e).ShouldBeFalse();
                ctx.count.ShouldBe(0);
                ctx.GetEntities().ShouldBeEmpty();
            };

            it["destroys an entity and removes all its components"] = () => {
                e.Destroy();
                e.GetComponents().ShouldBeEmpty();
            };

            it["removes OnDestroyEntity handler"] = () => {
                var didDestroy = 0;
                ctx.OnEntityWillBeDestroyed += delegate {
                    didDestroy += 1;
                };
                e.Destroy();
                ctx.CreateEntity().Destroy();
                didDestroy.ShouldBe(2);
            };

            it["destroys all entities"] = () => {
                ctx.CreateEntity();
                ctx.DestroyAllEntities();
                ctx.HasEntity(e).ShouldBeFalse();
                ctx.count.ShouldBe(0);
                ctx.GetEntities().ShouldBeEmpty();
                e.GetComponents().ShouldBeEmpty();
            };

            it["ensures same deterministic order when getting entities after destroying all entities"] = () => {

                // This is a Unity specific problem. Run Unity Test Tools with in the Entitas.Unity project

                const int numEntities = 10;

                for (int i = 0; i < numEntities; i++) {
                    ctx.CreateEntity();
                }

                var order1 = new int[numEntities];
                var entities1 = ctx.GetEntities();
                for (int i = 0; i < numEntities; i++) {
                    order1[i] = entities1[i].creationIndex;
                }

                ctx.DestroyAllEntities();
                ctx.ResetCreationIndex();

                for (int i = 0; i < numEntities; i++) {
                    ctx.CreateEntity();
                }

                var order2 = new int[numEntities];
                var entities2 = ctx.GetEntities();
                for (int i = 0; i < numEntities; i++) {
                    order2[i] = entities2[i].creationIndex;
                }

                for (int i = 0; i < numEntities; i++) {
                    var index1 = order1[i];
                    var index2 = order2[i];
                    index1.ShouldBe(index2);
                }
            };

            it["throws when destroying all entities and there are still entities retained"] = expect<ContextStillHasRetainedEntitiesException>(() => {
                ctx.CreateEntity().Retain(new object());
                ctx.DestroyAllEntities();
            });
        };

        context["internal caching"] = () => {

            it["caches entities"] = () => {
                var entities = ctx.GetEntities();
                ctx.GetEntities().ShouldBeSameAs(entities);
            };

            it["updates entities cache when creating an entity"] = () => {
                var entities = ctx.GetEntities();
                ctx.CreateEntity();
                ctx.GetEntities().ShouldNotBeSameAs(entities);
            };

            it["updates entities cache when destroying an entity"] = () => {
                var e = ctx.CreateEntity();
                var entities = ctx.GetEntities();
                e.Destroy();
                ctx.GetEntities().ShouldNotBeSameAs(entities);
            };
        };

        context["events"] = () => {

            var didDispatch = 0;

            before = () => {
                didDispatch = 0;
            };

            it["dispatches OnEntityCreated when creating a new entity"] = () => {
                IEntity eventEntity = null;
                ctx.OnEntityCreated += (c, entity) => {
                    didDispatch += 1;
                    eventEntity = entity;
                    c.ShouldBeSameAs(ctx);
                };

                var e = ctx.CreateEntity();
                didDispatch.ShouldBe(1);
                eventEntity.ShouldBeSameAs(e);
            };

            it["dispatches OnEntityWillBeDestroyed when destroying an entity"] = () => {
                var e = ctx.CreateEntity();
                e.AddComponentA();
                ctx.OnEntityWillBeDestroyed += (c, entity) => {
                    didDispatch += 1;
                    c.ShouldBeSameAs(ctx);
                    entity.ShouldBeSameAs(e);
                    entity.HasComponentA().ShouldBeTrue();
                    entity.isEnabled.ShouldBeTrue();

                    ((IContext<TestEntity>)c).GetEntities().Length.ShouldBe(0);
                };
                ctx.GetEntities();
                e.Destroy();
                didDispatch.ShouldBe(1);
            };

            it["dispatches OnEntityDestroyed when destroying an entity"] = () => {
                var e = ctx.CreateEntity();
                ctx.OnEntityDestroyed += (p, entity) => {
                    didDispatch += 1;
                    p.ShouldBeSameAs(ctx);
                    entity.ShouldBeSameAs(e);
                    entity.HasComponentA().ShouldBeFalse();
                    entity.isEnabled.ShouldBeFalse();
                };
                e.Destroy();
                didDispatch.ShouldBe(1);
            };

            it["entity is released after OnEntityDestroyed"] = () => {
                var e = ctx.CreateEntity();
                ctx.OnEntityDestroyed += (p, entity) => {
                    didDispatch += 1;
                    entity.retainCount.ShouldBe(1);
                    var newEntity = ctx.CreateEntity();
                    newEntity.ShouldNotBeNull();
                    newEntity.ShouldNotBeSameAs(entity);
                };
                e.Destroy();
                var reusedEntity = ctx.CreateEntity();
                reusedEntity.ShouldBeSameAs(e);
                didDispatch.ShouldBe(1);
            };

            it["throws if entity is released before it is destroyed"] = expect<EntityIsNotDestroyedException>(() => {
                var e = ctx.CreateEntity();
                e.Release(ctx);
            });

            it["dispatches OnGroupCreated when creating a new group"] = () => {
                IGroup eventGroup = null;
                ctx.OnGroupCreated += (p, g) => {
                    didDispatch += 1;
                    p.ShouldBeSameAs(ctx);
                    eventGroup = g;
                };
                var group = ctx.GetGroup(Matcher<TestEntity>.AllOf(0));
                didDispatch.ShouldBe(1);
                eventGroup.ShouldBeSameAs(group);
            };

            it["doesn't dispatch OnGroupCreated when group alredy exists"] = () => {
                ctx.GetGroup(Matcher<TestEntity>.AllOf(0));
                ctx.OnGroupCreated += delegate { this.Fail(); };
                ctx.GetGroup(Matcher<TestEntity>.AllOf(0));
            };

            it["removes all external delegates when destroying an entity"] = () => {
                var e = ctx.CreateEntity();
                e.OnComponentAdded += delegate { this.Fail(); };
                e.OnComponentRemoved += delegate { this.Fail(); };
                e.OnComponentReplaced += delegate { this.Fail(); };
                e.Destroy();
                var e2 = ctx.CreateEntity();
                e2.ShouldBeSameAs(e);
                e2.AddComponentA();
                e2.ReplaceComponentA(Component.A);
                e2.RemoveComponentA();
            };

            it["will not remove external delegates for OnEntityReleased"] = () => {
                var e = ctx.CreateEntity();
                var didRelease = 0;
                e.OnEntityReleased += entity => didRelease += 1;
                e.Destroy();
                didRelease.ShouldBe(1);
            };

            it["removes all external delegates from OnEntityReleased when after being dispatched"] = () => {
                var e = ctx.CreateEntity();
                var didRelease = 0;
                e.OnEntityReleased += entity => didRelease += 1;
                e.Destroy();
                e.Retain(this);
                e.Release(this);
                didRelease.ShouldBe(1);
            };

            it["removes all external delegates from OnEntityReleased after being dispatched (when delayed release)"] = () => {
                var e = ctx.CreateEntity();
                var didRelease = 0;
                e.OnEntityReleased += entity => didRelease += 1;
                e.Retain(this);
                e.Destroy();
                didRelease.ShouldBe(0);
                e.Release(this);
                didRelease.ShouldBe(1);

                e.Retain(this);
                e.Release(this);
                didRelease.ShouldBe(1);
            };
        };

        context["entity pool"] = () => {

            it["gets entity from object pool"] = () => {
                var e = ctx.CreateEntity();
                e.ShouldNotBeNull();
                e.GetType().ShouldBe(typeof(TestEntity));
            };

            it["destroys entity when pushing back to object pool"] = () => {
                var e = ctx.CreateEntity();
                e.AddComponentA();
                e.Destroy();
                e.HasComponent(CID.ComponentA).ShouldBeFalse();
            };

            it["returns pushed entity"] = () => {
                var e = ctx.CreateEntity();
                e.AddComponentA();
                e.Destroy();
                var entity = ctx.CreateEntity();
                entity.HasComponent(CID.ComponentA).ShouldBeFalse();
                entity.ShouldBeSameAs(e);
            };

            it["only returns released entities"] = () => {
                var e1 = ctx.CreateEntity();
                e1.Retain(this);
                e1.Destroy();
                var e2 = ctx.CreateEntity();
                e2.ShouldNotBeSameAs(e1);
                e1.Release(this);
                var e3 = ctx.CreateEntity();
                e3.ShouldBeSameAs(e1);
            };

            it["returns new entity"] = () => {
                var e1 = ctx.CreateEntity();
                e1.AddComponentA();
                e1.Destroy();
                ctx.CreateEntity();
                var e2 = ctx.CreateEntity();
                e2.HasComponent(CID.ComponentA).ShouldBeFalse();
                e2.ShouldNotBeSameAs(e1);
            };

            it["sets up entity from pool"] = () => {
                var e = ctx.CreateEntity();
                var creationIndex = e.creationIndex;
                e.Destroy();
                var g = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));

                e = ctx.CreateEntity();
                e.creationIndex.ShouldBe(creationIndex + 1);
                e.isEnabled.ShouldBeTrue();

                e.AddComponentA();
                g.GetEntities().ShouldContain(e);
            };

            context["when entity gets destroyed"] = () => {

                TestEntity e = null;

                before = () => {
                    e = ctx.CreateEntity();
                    e.AddComponentA();
                    e.Destroy();
                };

                it["throws when adding component"] = expect<EntityIsNotEnabledException>(() => e.AddComponentA());
                it["throws when removing component"] = expect<EntityIsNotEnabledException>(() => e.RemoveComponentA());
                it["throws when replacing component"] = expect<EntityIsNotEnabledException>(() => e.ReplaceComponentA(new ComponentA()));
                it["throws when replacing component with null"] = expect<EntityIsNotEnabledException>(() => e.ReplaceComponentA(null));
                it["throws when attempting to destroy again"] = expect<EntityIsNotEnabledException>(() => e.Destroy());
            };
        };

        context["groups"] = () => {

            it["gets empty group for matcher when no entities were created"] = () => {
                var g = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
                g.ShouldNotBeNull();
                g.GetEntities().ShouldBeEmpty();
            };

            context["when entities created"] = () => {

                TestEntity eAB1 = null;
                TestEntity eAB2 = null;
                TestEntity eA = null;

                IMatcher<TestEntity> matcherAB = Matcher<TestEntity>.AllOf(new [] {
                    CID.ComponentA,
                    CID.ComponentB
                });

                before = () => {
                    eAB1 = ctx.CreateEntity();
                    eAB1.AddComponentA();
                    eAB1.AddComponentB();

                    eAB2 = ctx.CreateEntity();
                    eAB2.AddComponentA();
                    eAB2.AddComponentB();

                    eA = ctx.CreateEntity();
                    eA.AddComponentA();
                };

                it["gets group with matching entities"] = () => {
                    var g = ctx.GetGroup(matcherAB).GetEntities();
                    g.Length.ShouldBe(2);
                    g.ShouldContain(eAB1);
                    g.ShouldContain(eAB2);
                };

                it["gets cached group"] = () => {
                    ctx.GetGroup(matcherAB).ShouldBeSameAs(ctx.GetGroup(matcherAB));
                };

                it["cached group contains newly created matching entity"] = () => {
                    var g = ctx.GetGroup(matcherAB);
                    eA.AddComponentB();
                    g.GetEntities().ShouldContain(eA);
                };

                it["cached group doesn't contain entity which are not matching anymore"] = () => {
                    var g = ctx.GetGroup(matcherAB);
                    eAB1.RemoveComponentA();
                    g.GetEntities().ShouldNotContain(eAB1);
                };

                it["removes destroyed entity"] = () => {
                    var g = ctx.GetGroup(matcherAB);
                    eAB1.Destroy();
                    g.GetEntities().ShouldNotContain(eAB1);
                };

                it["group dispatches OnEntityRemoved and OnEntityAdded when replacing components"] = () => {
                    var g = ctx.GetGroup(matcherAB);
                    var didDispatchRemoved = 0;
                    var didDispatchAdded = 0;
                    var componentA = new ComponentA();
                    g.OnEntityRemoved += (group, entity, index, component) => {
                        group.ShouldBeSameAs(g);
                        entity.ShouldBeSameAs(eAB1);
                        index.ShouldBe(CID.ComponentA);
                        component.ShouldBeSameAs(Component.A);
                        didDispatchRemoved++;
                    };
                    g.OnEntityAdded += (group, entity, index, component) => {
                        group.ShouldBeSameAs(g);
                        entity.ShouldBeSameAs(eAB1);
                        index.ShouldBe(CID.ComponentA);
                        component.ShouldBeSameAs(componentA);
                        didDispatchAdded++;
                    };
                    eAB1.ReplaceComponentA(componentA);

                    didDispatchRemoved.ShouldBe(1);
                    didDispatchAdded.ShouldBe(1);
                };

                it["group dispatches OnEntityUpdated with previous and current component when replacing a component"] = () => {
                    var updated = 0;
                    var prevComp = eA.GetComponent(CID.ComponentA);
                    var newComp = new ComponentA();
                    var g = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
                    g.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => {
                        updated += 1;
                        group.ShouldBeSameAs(g);
                        entity.ShouldBeSameAs(eA);
                        index.ShouldBe(CID.ComponentA);
                        previousComponent.ShouldBeSameAs(prevComp);
                        newComponent.ShouldBeSameAs(newComp);
                    };

                    eA.ReplaceComponent(CID.ComponentA, newComp);

                    updated.ShouldBe(1);
                };

                it["group with matcher NoneOf doesn't dispatch OnEntityAdded when destroying entity"] = () => {
                    var e = ctx.CreateEntity()
                                .AddComponentA()
                                .AddComponentB();
                    var matcher = Matcher<TestEntity>.AllOf(CID.ComponentB).NoneOf(CID.ComponentA);
                    var g = ctx.GetGroup(matcher);
                    g.OnEntityAdded += delegate { this.Fail(); };
                    e.Destroy();
                };

                context["event timing"] = () => {

                    before = () => {
                        ctx = new MyTestContext();
                    };

                    it["dispatches group.OnEntityAdded events after all groups are updated"] = () => {
                        var groupA = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB));
                        var groupB = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));

                        groupA.OnEntityAdded += delegate {
                            groupB.count.ShouldBe(1);
                        };

                        var entity = ctx.CreateEntity();
                        entity.AddComponentA();
                        entity.AddComponentB();
                    };

                    it["dispatches group.OnEntityRemoved events after all groups are updated"] = () => {
                        ctx = new MyTestContext();
                        var groupB = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));
                        var groupAB = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB));

                        groupB.OnEntityRemoved += delegate {
                            groupAB.count.ShouldBe(0);
                        };

                        var entity = ctx.CreateEntity();
                        entity.AddComponentA();
                        entity.AddComponentB();

                        entity.RemoveComponentB();
                    };
                };
            };
        };

        context["EntityIndex"] = () => {

            it["throws when EntityIndex for key doesn't exist"] = expect<ContextEntityIndexDoesNotExistException>(() => {
                ctx.GetEntityIndex("unknown");
            });

            it["adds and EntityIndex"] = () => {
                const int componentIndex = 1;
                var entityIndex = new PrimaryEntityIndex<TestEntity, string>("TestIndex", ctx.GetGroup(Matcher<TestEntity>.AllOf(componentIndex)), (arg1, arg2) => string.Empty);
                ctx.AddEntityIndex(entityIndex);
                ctx.GetEntityIndex(entityIndex.name).ShouldBeSameAs(entityIndex);
            };

            it["throws when adding an EntityIndex with same name"] = expect<ContextEntityIndexDoesAlreadyExistException>(() => {
                const int componentIndex = 1;
                var entityIndex = new PrimaryEntityIndex<TestEntity, string>("TestIndex", ctx.GetGroup(Matcher<TestEntity>.AllOf(componentIndex)), (arg1, arg2) => string.Empty);
                ctx.AddEntityIndex(entityIndex);
                ctx.AddEntityIndex(entityIndex);
            });
        };

        context["reset"] = () => {

            context["context"] = () => {

                it["resets creation index"] = () => {
                    ctx.CreateEntity();

                    ctx.ResetCreationIndex();

                    ctx.CreateEntity().creationIndex.ShouldBe(0);
                };

                context["removes all event handlers"] = () => {

                    it["removes OnEntityCreated"] = () => {
                        ctx.OnEntityCreated += delegate { this.Fail(); };
                        ctx.RemoveAllEventHandlers();

                        ctx.CreateEntity();
                    };

                    it["removes OnEntityWillBeDestroyed"] = () => {
                        ctx.OnEntityWillBeDestroyed += delegate { this.Fail(); };
                        ctx.RemoveAllEventHandlers();

                        ctx.CreateEntity().Destroy();
                    };

                    it["removes OnEntityDestroyed"] = () => {
                        ctx.OnEntityDestroyed += delegate { this.Fail(); };
                        ctx.RemoveAllEventHandlers();

                        ctx.CreateEntity().Destroy();
                    };

                    it["removes OnGroupCreated"] = () => {
                        ctx.OnGroupCreated += delegate { this.Fail(); };
                        ctx.RemoveAllEventHandlers();

                        ctx.GetGroup(Matcher<TestEntity>.AllOf(0));
                    };
                };
            };

            context["component pools"] = () => {

                before = () => {
                    var entity = ctx.CreateEntity();
                    entity.AddComponentA();
                    entity.AddComponentB();
                    entity.RemoveComponentA();
                    entity.RemoveComponentB();
                };

                it["clears all component pools"] = () => {
                    ctx.componentPools[CID.ComponentA].Count.ShouldBe(1);
                    ctx.componentPools[CID.ComponentB].Count.ShouldBe(1);

                    ctx.ClearComponentPools();

                    ctx.componentPools[CID.ComponentA].Count.ShouldBe(0);
                    ctx.componentPools[CID.ComponentB].Count.ShouldBe(0);
                };

                it["clears a specific component pool"] = () => {
                    ctx.ClearComponentPool(CID.ComponentB);

                    ctx.componentPools[CID.ComponentA].Count.ShouldBe(1);
                    ctx.componentPools[CID.ComponentB].Count.ShouldBe(0);
                };

                it["only clears existing component pool"] = () => {
                    ctx.ClearComponentPool(CID.ComponentC);
                };
            };
        };

        context["EntitasCache"] = () => {

            it["pops new list from list pool"] = () => {
                var groupA = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
                var groupAB = ctx.GetGroup(Matcher<TestEntity>.AnyOf(CID.ComponentA, CID.ComponentB));
                var groupABC = ctx.GetGroup(Matcher<TestEntity>.AnyOf(CID.ComponentA, CID.ComponentB, CID.ComponentC));

                var didExecute = 0;

                groupA.OnEntityAdded += (g, entity, index, component) => {
                    didExecute += 1;
                    entity.RemoveComponentA();
                };

                groupAB.OnEntityAdded += (g, entity, index, component) => {
                    didExecute += 1;
                };

                groupABC.OnEntityAdded += (g, entity, index, component) => {
                    didExecute += 1;
                };

                ctx.CreateEntity().AddComponentA();

                didExecute.ShouldBe(3);
            };
        };
    }
}
