using Entitas;
using NSpec;

class describe_Context : nspec {

    void when_created() {

        IContext<TestEntity> ctx = null;

        before = () => {
            ctx = new MyTestContext();
        };

        it["increments creationIndex"] = () => {
            ctx.CreateEntity().creationIndex.should_be(0);
            ctx.CreateEntity().creationIndex.should_be(1);
        };

        it["starts with given creationIndex"] = () => {
            new MyTestContext(CID.TotalComponents, 42, null).CreateEntity().creationIndex.should_be(42);
        };

        it["has no entities when no entities were created"] = () => {
            ctx.GetEntities().should_be_empty();
        };

        it["gets total entity count"] = () => {
            ctx.count.should_be(0);
        };

        it["creates entity"] = () => {
            var e = ctx.CreateEntity();
            e.should_not_be_null();
            e.GetType().should_be(typeof(TestEntity));

            e.totalComponents.should_be(ctx.totalComponents);
            e.isEnabled.should_be_true();
        };

        it["has default ContextInfo"] = () => {
            ctx.contextInfo.name.should_be("Unnamed Context");
            ctx.contextInfo.componentNames.Length.should_be(CID.TotalComponents);
            for(int i = 0; i < ctx.contextInfo.componentNames.Length; i++) {
                ctx.contextInfo.componentNames[i].should_be("Index " + i);
            }
        };

        it["creates component pools"] = () => {
            ctx.componentPools.should_not_be_null();
            ctx.componentPools.Length.should_be(CID.TotalComponents);
        };

        it["creates entity with component pools"] = () => {
            var e = ctx.CreateEntity();
            e.componentPools.should_be_same(ctx.componentPools);
        };

        it["throws when destroying an entity which the context doesn't contain"] = expect<ContextDoesNotContainEntityException>(() => {
            var e = ctx.CreateEntity();
            ctx.DestroyEntity(e);
            ctx.DestroyEntity(e);
        });

        it["can ToString"] = () => {
            ctx.ToString().should_be("Unnamed Context");
        };

        context["when ContextInfo set"] = () => {

            ContextInfo contextInfo = null;

            before = () => {
                var componentNames = new[] { "Health", "Position", "View" };
                var componentTypes = new[] { typeof(ComponentA), typeof(ComponentB), typeof(ComponentC) };
                contextInfo = new ContextInfo("My Context", componentNames, componentTypes);
                ctx = new MyTestContext(componentNames.Length, 0, contextInfo);
            };

            it["has custom ContextInfo"] = () => {
                ctx.contextInfo.should_be_same(contextInfo);
            };

            it["creates entity with same ContextInfo"] = () => {
                ctx.CreateEntity().contextInfo.should_be_same(contextInfo);
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
                ctx.count.should_be(1);
            };

            it["has entities that were created with CreateEntity()"] = () => {
                ctx.HasEntity(e).should_be_true();
            };

            it["doesn't have entities that were not created with CreateEntity()"] = () => {
                ctx.HasEntity(this.CreateEntity()).should_be_false();
            };

            it["returns all created entities"] = () => {
                var e2 = ctx.CreateEntity();
                var entities = ctx.GetEntities();
                entities.Length.should_be(2);
                entities.should_contain(e);
                entities.should_contain(e2);
            };

            it["destroys entity and removes it"] = () => {
                ctx.DestroyEntity(e);
                ctx.HasEntity(e).should_be_false();
                ctx.count.should_be(0);
                ctx.GetEntities().should_be_empty();
            };

            it["destroys an entity and removes all its components"] = () => {
                ctx.DestroyEntity(e);
                e.GetComponents().should_be_empty();
            };

            it["destroys all entities"] = () => {
                ctx.CreateEntity();
                ctx.DestroyAllEntities();
                ctx.HasEntity(e).should_be_false();
                ctx.count.should_be(0);
                ctx.GetEntities().should_be_empty();
                e.GetComponents().should_be_empty();
            };

            it["ensures same deterministic order when getting entities after destroying all entities"] = () => {

                // This is a Unity specific problem. Run Unity Test Tools with in the Entitas.Unity project

                const int numEntities = 10;

                for(int i = 0; i < numEntities; i++) {
                    ctx.CreateEntity();
                }

                var order1 = new int[numEntities];
                var entities1 = ctx.GetEntities();
                for(int i = 0; i < numEntities; i++) {
                    order1[i] = entities1[i].creationIndex;
                }

                ctx.DestroyAllEntities();
                ctx.ResetCreationIndex();

                for(int i = 0; i < numEntities; i++) {
                    ctx.CreateEntity();
                }

                var order2 = new int[numEntities];
                var entities2 = ctx.GetEntities();
                for(int i = 0; i < numEntities; i++) {
                    order2[i] = entities2[i].creationIndex;
                }

                for(int i = 0; i < numEntities; i++) {
                    var index1 = order1[i];
                    var index2 = order2[i];
                    index1.should_be(index2);
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
                ctx.GetEntities().should_be_same(entities);
            };

            it["updates entities cache when creating an entity"] = () => {
                var entities = ctx.GetEntities();
                ctx.CreateEntity();
                ctx.GetEntities().should_not_be_same(entities);
            };

            it["updates entities cache when destroying an entity"] = () => {
                var e = ctx.CreateEntity();
                var entities = ctx.GetEntities();
                ctx.DestroyEntity(e);
                ctx.GetEntities().should_not_be_same(entities);
            };
        };

        context["events"] = () => {

            var didDispatch = 0;

            before = () => {
                didDispatch = 0;
            };

            it["dispatches OnEntityCreated when creating a new entity"] = () => {
                IEntity eventEntity = null;
                ctx.OnEntityCreated += (p, entity) => {
                    didDispatch += 1;
                    eventEntity = entity;
                    p.should_be_same(p);
                };

                var e = ctx.CreateEntity();
                didDispatch.should_be(1);
                eventEntity.should_be_same(e);
            };

            it["dispatches OnEntityWillBeDestroyed when destroying an entity"] = () => {
                var e = ctx.CreateEntity();
                e.AddComponentA();
                ctx.OnEntityWillBeDestroyed += (c, entity) => {
                    didDispatch += 1;
                    c.should_be_same(ctx);
                    entity.should_be_same(e);
                    entity.HasComponentA().should_be_true();
                    entity.isEnabled.should_be_true();

                    ((IContext<TestEntity>)c).GetEntities().Length.should_be(0);
                };
                ctx.GetEntities();
                ctx.DestroyEntity(e);
                didDispatch.should_be(1);
            };

            it["dispatches OnEntityDestroyed when destroying an entity"] = () => {
                var e = ctx.CreateEntity();
                ctx.OnEntityDestroyed += (p, entity) => {
                    didDispatch += 1;
                    p.should_be_same(ctx);
                    entity.should_be_same(e);
                    entity.HasComponentA().should_be_false();
                    entity.isEnabled.should_be_false();
                };
                ctx.DestroyEntity(e);
                didDispatch.should_be(1);
            };

            it["entity is released after OnEntityDestroyed"] = () => {
                var e = ctx.CreateEntity();
                ctx.OnEntityDestroyed += (p, entity) => {
                    didDispatch += 1;
                    entity.retainCount.should_be(1);
                    var newEntity = ctx.CreateEntity();
                    newEntity.should_not_be_null();
                    newEntity.should_not_be_same(entity);
                };
                ctx.DestroyEntity(e);
                var reusedEntity = ctx.CreateEntity();
                reusedEntity.should_be_same(e);
                didDispatch.should_be(1);
            };

            it["throws if entity is released before it is destroyed"] = expect<EntityIsNotDestroyedException>(() => {
                var e = ctx.CreateEntity();
                e.Release(ctx);
            });

            it["dispatches OnGroupCreated when creating a new group"] = () => {
                IGroup eventGroup = null;
                ctx.OnGroupCreated += (p, g) => {
                    didDispatch += 1;
                    p.should_be_same(ctx);
                    eventGroup = g;
                };
                var group = ctx.GetGroup(Matcher<TestEntity>.AllOf(0));
                didDispatch.should_be(1);
                eventGroup.should_be_same(group);
            };

            it["doesn't dispatch OnGroupCreated when group alredy exists"] = () => {
                ctx.GetGroup(Matcher<TestEntity>.AllOf(0));
                ctx.OnGroupCreated += delegate { this.Fail(); };
                ctx.GetGroup(Matcher<TestEntity>.AllOf(0));
            };

            it["dispatches OnGroupCleared when clearing groups"] = () => {
                IGroup eventGroup = null;
                ctx.OnGroupCleared += (p, g) => {
                    didDispatch += 1;
                    p.should_be_same(ctx);
                    eventGroup = g;
                };
                ctx.GetGroup(Matcher<TestEntity>.AllOf(0));
                var group2 = ctx.GetGroup(Matcher<TestEntity>.AllOf(1));
                ctx.ClearGroups();

                didDispatch.should_be(2);
                eventGroup.should_be_same(group2);
            };

            it["removes all external delegates when destroying an entity"] = () => {
                var e = ctx.CreateEntity();
                e.OnComponentAdded += delegate { this.Fail(); };
                e.OnComponentRemoved += delegate { this.Fail(); };
                e.OnComponentReplaced += delegate { this.Fail(); };
                ctx.DestroyEntity(e);
                var e2 = ctx.CreateEntity();
                e2.should_be_same(e);
                e2.AddComponentA();
                e2.ReplaceComponentA(Component.A);
                e2.RemoveComponentA();
            };

            it["will not remove external delegates for OnEntityReleased"] = () => {
                var e = ctx.CreateEntity();
                var didRelease = 0;
                e.OnEntityReleased += entity => didRelease += 1;
                ctx.DestroyEntity(e);
                didRelease.should_be(1);
            };

            it["removes all external delegates from OnEntityReleased when after being dispatched"] = () => {
                var e = ctx.CreateEntity();
                var didRelease = 0;
                e.OnEntityReleased += entity => didRelease += 1;
                ctx.DestroyEntity(e);
                e.Retain(this);
                e.Release(this);
                didRelease.should_be(1);
            };

            it["removes all external delegates from OnEntityReleased after being dispatched (when delayed release)"] = () => {
                var e = ctx.CreateEntity();
                var didRelease = 0;
                e.OnEntityReleased += entity => didRelease += 1;
                e.Retain(this);
                ctx.DestroyEntity(e);
                didRelease.should_be(0);
                e.Release(this);
                didRelease.should_be(1);

                e.Retain(this);
                e.Release(this);
                didRelease.should_be(1);
            };
        };

        context["entity pool"] = () => {

            it["gets entity from object pool"] = () => {
                var e = ctx.CreateEntity();
                e.should_not_be_null();
                e.GetType().should_be(typeof(TestEntity));
            };

            it["destroys entity when pushing back to object pool"] = () => {
                var e = ctx.CreateEntity();
                e.AddComponentA();
                ctx.DestroyEntity(e);
                e.HasComponent(CID.ComponentA).should_be_false();
            };

            it["returns pushed entity"] = () => {
                var e = ctx.CreateEntity();
                e.AddComponentA();
                ctx.DestroyEntity(e);
                var entity = ctx.CreateEntity();
                entity.HasComponent(CID.ComponentA).should_be_false();
                entity.should_be_same(e);
            };

            it["only returns released entities"] = () => {
                var e1 = ctx.CreateEntity();
                e1.Retain(this);
                ctx.DestroyEntity(e1);
                var e2 = ctx.CreateEntity();
                e2.should_not_be_same(e1);
                e1.Release(this);
                var e3 = ctx.CreateEntity();
                e3.should_be_same(e1);
            };

            it["returns new entity"] = () => {
                var e1 = ctx.CreateEntity();
                e1.AddComponentA();
                ctx.DestroyEntity(e1);
                ctx.CreateEntity();
                var e2 = ctx.CreateEntity();
                e2.HasComponent(CID.ComponentA).should_be_false();
                e2.should_not_be_same(e1);
            };

            it["sets up entity from pool"] = () => {
                var e = ctx.CreateEntity();
                var creationIndex = e.creationIndex;
                ctx.DestroyEntity(e);
                var g = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));

                e = ctx.CreateEntity();
                e.creationIndex.should_be(creationIndex + 1);
                e.isEnabled.should_be_true();

                e.AddComponentA();
                g.GetEntities().should_contain(e);
            };

            context["when entity gets destroyed"] = () => {

                TestEntity e = null;

                before = () => {
                    e = ctx.CreateEntity();
                    e.AddComponentA();
                    ctx.DestroyEntity(e);
                };

                it["throws when adding component"] = expect<EntityIsNotEnabledException>(() => e.AddComponentA());
                it["throws when removing component"] = expect<EntityIsNotEnabledException>(() => e.RemoveComponentA());
                it["throws when replacing component"] = expect<EntityIsNotEnabledException>(() => e.ReplaceComponentA(new ComponentA()));
                it["throws when replacing component with null"] = expect<EntityIsNotEnabledException>(() => e.ReplaceComponentA(null));
            };
        };

        context["groups"] = () => {

            it["gets empty group for matcher when no entities were created"] = () => {
                var g = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
                g.should_not_be_null();
                g.GetEntities().should_be_empty();
            };

            context["when entities created"] = () => {

                TestEntity eAB1 = null;
                TestEntity eAB2 = null;
                TestEntity eA = null;

                IMatcher<TestEntity> matcherAB = Matcher<TestEntity>.AllOf(new[] {
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
                    g.Length.should_be(2);
                    g.should_contain(eAB1);
                    g.should_contain(eAB2);
                };

                it["gets cached group"] = () => {
                    ctx.GetGroup(matcherAB).should_be_same(ctx.GetGroup(matcherAB));
                };

                it["cached group contains newly created matching entity"] = () => {
                    var g = ctx.GetGroup(matcherAB);
                    eA.AddComponentB();
                    g.GetEntities().should_contain(eA);
                };

                it["cached group doesn't contain entity which are not matching anymore"] = () => {
                    var g = ctx.GetGroup(matcherAB);
                    eAB1.RemoveComponentA();
                    g.GetEntities().should_not_contain(eAB1);
                };

                it["removes destroyed entity"] = () => {
                    var g = ctx.GetGroup(matcherAB);
                    ctx.DestroyEntity(eAB1);
                    g.GetEntities().should_not_contain(eAB1);
                };

                it["group dispatches OnEntityRemoved and OnEntityAdded when replacing components"] = () => {
                    var g = ctx.GetGroup(matcherAB);
                    var didDispatchRemoved = 0;
                    var didDispatchAdded = 0;
                    var componentA = new ComponentA();
                    g.OnEntityRemoved += (group, entity, index, component) => {
                        group.should_be_same(g);
                        entity.should_be_same(eAB1);
                        index.should_be(CID.ComponentA);
                        component.should_be_same(Component.A);
                        didDispatchRemoved++;
                    };
                    g.OnEntityAdded += (group, entity, index, component) => {
                        group.should_be_same(g);
                        entity.should_be_same(eAB1);
                        index.should_be(CID.ComponentA);
                        component.should_be_same(componentA);
                        didDispatchAdded++;
                    };
                    eAB1.ReplaceComponentA(componentA);

                    didDispatchRemoved.should_be(1);
                    didDispatchAdded.should_be(1);
                };

                it["group dispatches OnEntityUpdated with previous and current component when replacing a component"] = () => {
                    var updated = 0;
                    var prevComp = eA.GetComponent(CID.ComponentA);
                    var newComp = new ComponentA();
                    var g = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
                    g.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => {
                        updated += 1;
                        group.should_be_same(g);
                        entity.should_be_same(eA);
                        index.should_be(CID.ComponentA);
                        previousComponent.should_be_same(prevComp);
                        newComponent.should_be_same(newComp);
                    };

                    eA.ReplaceComponent(CID.ComponentA, newComp);

                    updated.should_be(1);
                };

                it["group with matcher NoneOf doesn't dispatch OnEntityAdded when destroying entity"] = () => {
                    var e = ctx.CreateEntity()
                                .AddComponentA()
                                .AddComponentB();
                    var matcher = Matcher<TestEntity>.AllOf(CID.ComponentB).NoneOf(CID.ComponentA);
                    var g = ctx.GetGroup(matcher);
                    g.OnEntityAdded += delegate { this.Fail(); };
                    ctx.DestroyEntity(e);
                };

                context["event timing"] = () => {

                    before = () => {
                        ctx = new MyTestContext();
                    };

                    it["dispatches group.OnEntityAdded events after all groups are updated"] = () => {
                        var groupA = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB));
                        var groupB = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentB));

                        groupA.OnEntityAdded += delegate {
                            groupB.count.should_be(1);
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
                            groupAB.count.should_be(0);
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
                var entityIndex = new PrimaryEntityIndex<TestEntity, string>(ctx.GetGroup(Matcher<TestEntity>.AllOf(componentIndex)), null);
                ctx.AddEntityIndex(componentIndex.ToString(), entityIndex);
                ctx.GetEntityIndex(componentIndex.ToString()).should_be_same(entityIndex);
            };

            it["throws when adding an EntityIndex with same name"] = expect<ContextEntityIndexDoesAlreadyExistException>(() => {
                const int componentIndex = 1;
                var entityIndex = new PrimaryEntityIndex<TestEntity, string>(ctx.GetGroup(Matcher<TestEntity>.AllOf(componentIndex)), null);
                ctx.AddEntityIndex(componentIndex.ToString(), entityIndex);
                ctx.AddEntityIndex(componentIndex.ToString(), entityIndex);
            });
        };

        context["reset"] = () => {

            context["groups"] = () => {

                it["resets and removes groups from context"] = () => {

                    var m = Matcher<TestEntity>.AllOf(CID.ComponentA);
                    var groupsCreated = 0;
                    IGroup createdGroup = null;
                    ctx.OnGroupCreated += (p, g) => {
                        groupsCreated += 1;
                        createdGroup = g;
                    };

                    var initialGroup = ctx.GetGroup(m);

                    ctx.ClearGroups();

                    ctx.GetGroup(m);

                    ctx.CreateEntity().AddComponentA();

                    groupsCreated.should_be(2);
                    createdGroup.should_not_be_same(initialGroup);

                    initialGroup.count.should_be(0);
                    createdGroup.count.should_be(1);
                };

                it["removes all event handlers from groups"] = () => {
                    var m = Matcher<TestEntity>.AllOf(CID.ComponentA);
                    var group = ctx.GetGroup(m);

                    group.OnEntityAdded += delegate { this.Fail(); };

                    ctx.ClearGroups();

                    var e = ctx.CreateEntity();
                    e.AddComponentA();
                    group.HandleEntity(e, CID.ComponentA, Component.A);
                };

                it["releases entities in groups"] = () => {
                    var m = Matcher<TestEntity>.AllOf(CID.ComponentA);
                    ctx.GetGroup(m);
                    var entity = ctx.CreateEntity();
                    entity.AddComponentA();

                    ctx.ClearGroups();

                    entity.retainCount.should_be(1);
                };
            };

            context["context"] = () => {

                it["resets creation index"] = () => {
                    ctx.CreateEntity();

                    ctx.ResetCreationIndex();

                    ctx.CreateEntity().creationIndex.should_be(0);
                };


                context["removes all event handlers"] = () => {

                    it["removes OnEntityCreated"] = () => {
                        ctx.OnEntityCreated += delegate { this.Fail(); };
                        ctx.Reset();

                        ctx.CreateEntity();
                    };

                    it["removes OnEntityWillBeDestroyed"] = () => {
                        ctx.OnEntityWillBeDestroyed += delegate { this.Fail(); };
                        ctx.Reset();

                        ctx.DestroyEntity(ctx.CreateEntity());
                    };

                    it["removes OnEntityDestroyed"] = () => {
                        ctx.OnEntityDestroyed += delegate { this.Fail(); };
                        ctx.Reset();

                        ctx.DestroyEntity(ctx.CreateEntity());
                    };

                    it["removes OnGroupCreated"] = () => {
                        ctx.OnGroupCreated += delegate { this.Fail(); };
                        ctx.Reset();

                        ctx.GetGroup(Matcher<TestEntity>.AllOf(0));
                    };

                    it["removes OnGroupCleared"] = () => {
                        ctx.OnGroupCleared += delegate { this.Fail(); };
                        ctx.Reset();
                        ctx.GetGroup(Matcher<TestEntity>.AllOf(0));

                        ctx.ClearGroups();
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
                    ctx.componentPools[CID.ComponentA].Count.should_be(1);
                    ctx.componentPools[CID.ComponentB].Count.should_be(1);

                    ctx.ClearComponentPools();

                    ctx.componentPools[CID.ComponentA].Count.should_be(0);
                    ctx.componentPools[CID.ComponentB].Count.should_be(0);
                };

                it["clears a specific component pool"] = () => {
                    ctx.ClearComponentPool(CID.ComponentB);

                    ctx.componentPools[CID.ComponentA].Count.should_be(1);
                    ctx.componentPools[CID.ComponentB].Count.should_be(0);
                };

                it["only clears existing component pool"] = () => {
                    ctx.ClearComponentPool(CID.ComponentC);
                };
            };

            context["EntityIndex"] = () => {

                PrimaryEntityIndex<TestEntity, string> entityIndex = null;

                before = () => {
                    entityIndex = new PrimaryEntityIndex<TestEntity, string>(ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA)),
                        (e, c) => ((NameAgeComponent)(c)).name);
                    ctx.AddEntityIndex(CID.ComponentA.ToString(), entityIndex);
                };

                it["deactivates EntityIndex"] = () => {
                    var nameAgeComponent = new NameAgeComponent();
                    nameAgeComponent.name = "Max";

                    ctx.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
                    entityIndex.HasEntity("Max").should_be_true();

                    ctx.DeactivateAndRemoveEntityIndices();

                    entityIndex.HasEntity("Max").should_be_false();
                };

                it["removes EntityIndex"] = expect<ContextEntityIndexDoesNotExistException>(() => {
                    ctx.DeactivateAndRemoveEntityIndices();
                    ctx.GetEntityIndex(CID.ComponentA.ToString());
                });
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

                didExecute.should_be(3);
            };
        };
    }
}
