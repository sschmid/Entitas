using Entitas;
using NSpec;

class describe_Pool : nspec {
    void when_created() {

        Pool pool = null;
        before = () => {
            pool = new Pool(CID.NumComponents);
        };

        it["increments creationIndex"] = () => {
            pool.CreateEntity().creationIndex.should_be(0);
            pool.CreateEntity().creationIndex.should_be(1);
        };

        it["starts with given creationIndex"] = () => {
            new Pool(CID.NumComponents, 42, null).CreateEntity().creationIndex.should_be(42);
        };

        it["has no entities when no entities were created"] = () => {
            pool.GetEntities().should_be_empty();
        };

        it["gets total entity count"] = () => {
            pool.count.should_be(0);
        };

        it["creates entity"] = () => {
            var e = pool.CreateEntity();
            e.should_not_be_null();
            e.GetType().should_be(typeof(Entity));
        };

        it["has default PoolMetaData"] = () => {
            pool.metaData.poolName.should_be("Unnamed Pool");
            pool.metaData.componentNames.Length.should_be(CID.NumComponents);
            for (int i = 0; i < pool.metaData.componentNames.Length; i++) {
                pool.metaData.componentNames[i].should_be("Index " + i);
            }
        };

        it["creates component pools"] = () => {
            pool.componentPools.should_not_be_null();
            pool.componentPools.Length.should_be(CID.NumComponents);
        };

        it["creates entity with component pools"] = () => {
            var e = pool.CreateEntity();
            e.componentPools.should_be_same(pool.componentPools);
        };

        it["can ToString"] = () => {
            pool.ToString().should_be("Unnamed Pool");
        };

        context["when PoolMetaData set"] = () => {

            PoolMetaData metaData = null;
            before = () => {
                var componentNames = new [] { "Health", "Position", "View" };
                metaData = new PoolMetaData("My Pool", componentNames);
                pool = new Pool(componentNames.Length, 0, metaData);
            };

            it["has custom PoolMetaData"] = () => {
                pool.metaData.should_be_same(metaData);
            };

            it["creates entity with PoolMetaData"] = () => {
                pool.CreateEntity().poolMetaData.should_be_same(metaData);
            };

            it["throws when componentNames is not same length as totalComponents"] = expect<PoolMetaDataException>(() => {
                new Pool(metaData.componentNames.Length + 1, 0, metaData);
            });
        };

        context["when entity created"] = () => {

            Entity e = null;
            before = () => {
                e = pool.CreateEntity();
                e.AddComponentA();
            };

            it["gets total entity count"] = () => {
                pool.count.should_be(1);
            };

            it["has entities that were created with CreateEntity()"] = () => {
                pool.HasEntity(e).should_be_true();
            };

            it["doesn't have entities that were not created with CreateEntity()"] = () => {
                pool.HasEntity(this.CreateEntity()).should_be_false();
            };

            it["returns all created entities"] = () => {
                var e2 = pool.CreateEntity();
                var entities = pool.GetEntities();
                entities.Length.should_be(2);
                entities.should_contain(e);
                entities.should_contain(e2);
            };

            it["destroys entity and removes it"] = () => {
                pool.DestroyEntity(e);
                pool.HasEntity(e).should_be_false();
                pool.count.should_be(0);
                pool.GetEntities().should_be_empty();
            };

            it["destroys an entity and removes all its components"] = () => {
                pool.DestroyEntity(e);
                e.GetComponents().should_be_empty();
            };

            it["destroys all entities"] = () => {
                pool.CreateEntity();
                pool.DestroyAllEntities();
                pool.HasEntity(e).should_be_false();
                pool.count.should_be(0);
                pool.GetEntities().should_be_empty();
                e.GetComponents().should_be_empty();
            };

            it["ensures same deterministic order when getting entities after destroying all entities"] = () => {

                // This is a Unity specific problem. Run Unity Test Tools with in the Entitas.Unity project

                const int numEntities = 10;
                for (int i = 0; i < numEntities; i++) {
                    pool.CreateEntity();
                }

                var order1 = new int[numEntities];
                var entities1 = pool.GetEntities();
                for (int i = 0; i < numEntities; i++) {
                    order1[i] = entities1[i].creationIndex;
                }

                pool.DestroyAllEntities();
                pool.ResetCreationIndex();

                for (int i = 0; i < numEntities; i++) {
                    pool.CreateEntity();
                }

                var order2 = new int[numEntities];
                var entities2 = pool.GetEntities();
                for (int i = 0; i < numEntities; i++) {
                    order2[i] = entities2[i].creationIndex;
                }

                for (int i = 0; i < numEntities; i++) {
                    var index1 = order1[i];
                    var index2 = order2[i];
                    index1.should_be(index2);
                }
            };

            it["throws when destroying all entities and there are stille entities retained"] = expect<PoolStillHasRetainedEntitiesException>(() => {
                pool.CreateEntity().Retain(new object());
                pool.DestroyAllEntities();
            });
        };

        context["internal caching"] = () => {
            it["caches entities"] = () => {
                var entities = pool.GetEntities();
                pool.GetEntities().should_be_same(entities);
            };

            it["updates entities cache when creating an entity"] = () => {
                var entities = pool.GetEntities();
                pool.CreateEntity();
                pool.GetEntities().should_not_be_same(entities);
            };

            it["updates entities cache when destroying an entity"] = () => {
                var e = pool.CreateEntity();
                var entities = pool.GetEntities();
                pool.DestroyEntity(e);
                pool.GetEntities().should_not_be_same(entities);
            };
        };

        context["events"] = () => {

            var didDispatch = 0;
            before = () => {
                didDispatch = 0;
            };

            it["dispatches OnEntityCreated when creating a new entity"] = () => {
                Entity eventEntity = null;
                pool.OnEntityCreated += (p, entity) => {
                    didDispatch += 1;
                    eventEntity = entity;
                    p.should_be_same(p);
                };

                var e = pool.CreateEntity();
                didDispatch.should_be(1);
                eventEntity.should_be_same(e);
            };

            it["dispatches OnEntityWillBeDestroyed when destroying an entity"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                pool.OnEntityWillBeDestroyed += (p, entity) => {
                    didDispatch += 1;
                    p.should_be_same(pool);
                    entity.should_be_same(e);
                    entity.HasComponentA().should_be_true();
                    entity.IsEnabled().should_be_true();
                };
                pool.DestroyEntity(e);
                didDispatch.should_be(1);
            };

            it["dispatches OnEntityDestroyed when destroying an entity"] = () => {
                var e = pool.CreateEntity();
                pool.OnEntityDestroyed += (p, entity) => {
                    didDispatch += 1;
                    p.should_be_same(pool);
                    entity.should_be_same(e);
                    entity.HasComponentA().should_be_false();
                    entity.IsEnabled().should_be_false();
                };
                pool.DestroyEntity(e);
                didDispatch.should_be(1);
            };

            it["Entity is released after OnEntityDestroyed"] = () => {
                var e = pool.CreateEntity();
                pool.OnEntityDestroyed += (p, entity) => {
                    didDispatch += 1;
                    p.should_be_same(pool);
                    entity.should_be_same(e);
                    entity.retainCount.should_be(1);
                    var newEntity = pool.CreateEntity();
                    newEntity.should_not_be_null();
                    newEntity.should_not_be_same(entity);
                };
                pool.DestroyEntity(e);
                var reusedEntity = pool.CreateEntity();
                reusedEntity.should_be_same(e);
                didDispatch.should_be(1);
            };

            it["throws if entity is released before it is destroyed"] = expect<EntityIsNotDestroyedException>(() => {
                var e = pool.CreateEntity();
                e.Release(pool);
            });

            it["dispatches OnGroupCreated when creating a new group"] = () => {
                Group eventGroup = null;
                pool.OnGroupCreated += (p, g) => {
                    didDispatch += 1;
                    p.should_be_same(pool);
                    eventGroup = g;
                };
                var group = pool.GetGroup(Matcher.AllOf(0));
                didDispatch.should_be(1);
                eventGroup.should_be_same(group);
            };

            it["doesn't dispatch OnGroupCreated when group alredy exists"] = () => {
                pool.GetGroup(Matcher.AllOf(0));
                pool.OnGroupCreated += (p, g) => this.Fail();
                pool.GetGroup(Matcher.AllOf(0));
            };

            it["dispatches OnGroupCleared when clearing groups"] = () => {
                Group eventGroup = null;
                pool.OnGroupCleared += (p, g) => {
                    didDispatch += 1;
                    p.should_be_same(pool);
                    eventGroup = g;
                };
                pool.GetGroup(Matcher.AllOf(0));
                var group2 = pool.GetGroup(Matcher.AllOf(1));
                pool.ClearGroups();

                didDispatch.should_be(2);
                eventGroup.should_be_same(group2);
            };

            it["removes all external delegates when destroying an entity"] = () => {
                var e = pool.CreateEntity();
                e.OnComponentAdded += (entity, index, component) => this.Fail();
                e.OnComponentRemoved += (entity, index, component) => this.Fail();
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => this.Fail();
                pool.DestroyEntity(e);
                var e2 = pool.CreateEntity();
                e2.should_be_same(e);
                e2.AddComponentA();
                e2.ReplaceComponentA(Component.A);
                e2.RemoveComponentA();
            };
        };

        context["entity pool"] = () => {

            it["gets entity from object pool"] = () => {
                var e = pool.CreateEntity();
                e.should_not_be_null();
                e.GetType().should_be(typeof(Entity));
            };

            it["destroys entity when pushing back to object pool"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                pool.DestroyEntity(e);
                e.HasComponent(CID.ComponentA).should_be_false();
            };

            it["returns pushed entity"] = () => {
                var e = pool.CreateEntity();
                e.AddComponentA();
                pool.DestroyEntity(e);
                var entity = pool.CreateEntity();
                entity.HasComponent(CID.ComponentA).should_be_false();
                entity.should_be_same(e);
            };

            it["only returns released entities"] = () => {
                var e1 = pool.CreateEntity();
                e1.Retain(this);
                pool.DestroyEntity(e1);
                var e2 = pool.CreateEntity();
                e2.should_not_be_same(e1);
                e1.Release(this);
                var e3 = pool.CreateEntity();
                e3.should_be_same(e1);
            };

            it["returns new entity"] = () => {
                var e1 = pool.CreateEntity();
                e1.AddComponentA();
                pool.DestroyEntity(e1);
                pool.CreateEntity();
                var e2 = pool.CreateEntity();
                e2.HasComponent(CID.ComponentA).should_be_false();
                e2.should_not_be_same(e1);
            };

            it["sets up entity from pool"] = () => {
                pool.DestroyEntity(pool.CreateEntity());
                var g = pool.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
                var e = pool.CreateEntity();
                e.AddComponentA();
                g.GetEntities().should_contain(e);
            };

            context["when entity gets destroyed"] = () => {
                Entity e = null;
                before = () => {
                    e = pool.CreateEntity();
                    e.AddComponentA();
                    pool.DestroyEntity(e);
                };

                it["throws when adding component"] = expect<EntityIsNotEnabledException>(() => e.AddComponentA());
                it["throws when removing component"] = expect<EntityIsNotEnabledException>(() => e.RemoveComponentA());
                it["throws when replacing component"] = expect<EntityIsNotEnabledException>(() => e.ReplaceComponentA(new ComponentA()));
                it["throws when replacing component with null"] = expect<EntityIsNotEnabledException>(() => e.ReplaceComponentA(null));
            };
        };

        context["groups"] = () => {

            it["gets empty group for matcher when no entities were created"] = () => {
                var g = pool.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
                g.should_not_be_null();
                g.GetEntities().should_be_empty();
            };

            context["when entities created"] = () => {
                Entity eAB1 = null;
                Entity eAB2 = null;
                Entity eA = null;

                IMatcher matcherAB = Matcher.AllOf(new [] {
                    CID.ComponentA,
                    CID.ComponentB
                });

                before = () => {
                    eAB1 = pool.CreateEntity();
                    eAB1.AddComponentA();
                    eAB1.AddComponentB();
                    eAB2 = pool.CreateEntity();
                    eAB2.AddComponentA();
                    eAB2.AddComponentB();
                    eA = pool.CreateEntity();
                    eA.AddComponentA();
                };

                it["gets group with matching entities"] = () => {
                    var g = pool.GetGroup(matcherAB).GetEntities();
                    g.Length.should_be(2);
                    g.should_contain(eAB1);
                    g.should_contain(eAB2);
                };

                it["gets cached group"] = () => {
                    pool.GetGroup(matcherAB).should_be_same(pool.GetGroup(matcherAB));
                };

                it["cached group contains newly created matching entity"] = () => {
                    var g = pool.GetGroup(matcherAB);
                    eA.AddComponentB();
                    g.GetEntities().should_contain(eA);
                };

                it["cached group doesn't contain entity which are not matching anymore"] = () => {
                    var g = pool.GetGroup(matcherAB);
                    eAB1.RemoveComponentA();
                    g.GetEntities().should_not_contain(eAB1);
                };

                it["removes destroyed entity"] = () => {
                    var g = pool.GetGroup(matcherAB);
                    pool.DestroyEntity(eAB1);
                    g.GetEntities().should_not_contain(eAB1);
                };

                it["throws when destroying an entity the pool doesn't contain"] = expect<PoolDoesNotContainEntityException>(() => {
                    var e = pool.CreateEntity();
                    pool.DestroyEntity(e);
                    pool.DestroyEntity(e);
                });

                it["group dispatches OnEntityRemoved and OnEntityAdded when replacing components"] = () => {
                    var g = pool.GetGroup(matcherAB);
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
                    var g = pool.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
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

                context["event timing"] = () => {

                    before = () => {
                        pool = new Pool(CID.NumComponents);
                    };

                    it["dispatches group.OnEntityAdded events after all groups are updated"] = () => {
                        var groupA = pool.GetGroup(Matcher.AllOf(CID.ComponentA, CID.ComponentB));
                        var groupB = pool.GetGroup(Matcher.AllOf(CID.ComponentB));

                        groupA.OnEntityAdded += delegate {
                            groupB.count.should_be(1);
                        };

                        var entity = pool.CreateEntity();
                        entity.AddComponentA();
                        entity.AddComponentB();
                    };

                    it["dispatches group.OnEntityRemoved events after all groups are updated"] = () => {
                        pool = new Pool(CID.NumComponents);
                        var groupB = pool.GetGroup(Matcher.AllOf(CID.ComponentB));
                        var groupA = pool.GetGroup(Matcher.AllOf(CID.ComponentA, CID.ComponentB));

                        groupB.OnEntityRemoved += delegate {
                            groupA.count.should_be(0);
                        };

                        var entity = pool.CreateEntity();
                        entity.AddComponentA();
                        entity.AddComponentB();

                        entity.RemoveComponentB();
                    };
                };
            };
        };

        context["reset"] = () => {

            context["groups"] = () => {

                it["resets and removes groups from pool"] = () => {
                    var m = Matcher.AllOf(CID.ComponentA);
                    var groupsCreated = 0;
                    Group createdGroup = null;
                    pool.OnGroupCreated += (p, g) => {
                        groupsCreated += 1;
                        createdGroup = g;
                    };

                    var initialGroup = pool.GetGroup(m);

                    pool.ClearGroups();

                    pool.GetGroup(m);

                    pool.CreateEntity().AddComponentA();

                    groupsCreated.should_be(2);
                    createdGroup.should_not_be_same(initialGroup);

                    initialGroup.count.should_be(0);
                    createdGroup.count.should_be(1);
                };

                it["removes all event handlers from groups"] = () => {
                    var m = Matcher.AllOf(CID.ComponentA);
                    var group = pool.GetGroup(m);

                    group.OnEntityAdded += (g, entity, index, component) => this.Fail();

                    pool.ClearGroups();

                    var e = pool.CreateEntity();
                    e.AddComponentA();
                    group.HandleEntity(e, CID.ComponentA, Component.A);
                };

                it["releases entities in groups"] = () => {
                    var m = Matcher.AllOf(CID.ComponentA);
                    pool.GetGroup(m);
                    var entity = pool.CreateEntity();
                    entity.AddComponentA();

                    pool.ClearGroups();

                    entity.retainCount.should_be(1);
                };
            };

            context["pool"] = () => {

                it["resets creation index"] = () => {
                    pool.CreateEntity();

                    pool.ResetCreationIndex();

                    pool.CreateEntity().creationIndex.should_be(0);
                };
            };

            context["component pools"] = () => {

                before = () => {
                    var entity = pool.CreateEntity();
                    entity.AddComponentA();
                    entity.AddComponentB();
                    entity.RemoveComponentA();
                    entity.RemoveComponentB();
                };

                it["clears all component pools"] = () => {
                    pool.componentPools[CID.ComponentA].Count.should_be(1);
                    pool.componentPools[CID.ComponentB].Count.should_be(1);

                    pool.ClearComponentPools();

                    pool.componentPools[CID.ComponentA].Count.should_be(0);
                    pool.componentPools[CID.ComponentB].Count.should_be(0);
                };

                it["clears a specific component pool"] = () => {
                    pool.ClearComponentPool(CID.ComponentB);

                    pool.componentPools[CID.ComponentA].Count.should_be(1);
                    pool.componentPools[CID.ComponentB].Count.should_be(0);
                };

                it["only clears existing component pool"] = () => {
                    pool.ClearComponentPool(CID.ComponentC);
                };
            };
        };
    }
}
