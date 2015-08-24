using NSpec;
using Entitas;

class describe_Pool : nspec {
    Pool _pool;

    void before_each() {
        _pool = new Pool(CID.NumComponents);
    }

    void when_created() {

        it["increments creationIndex"] = () => {
            _pool.CreateEntity().creationIndex.should_be(0);
            _pool.CreateEntity().creationIndex.should_be(1);
        };

        it["starts with given creationIndex"] = () => {
            new Pool(CID.NumComponents, 42).CreateEntity().creationIndex.should_be(42);
        };

        it["has no entities when no entities were created"] = () => {
            _pool.GetEntities().should_be_empty();
        };

        it["creates entity"] = () => {
            var e = _pool.CreateEntity();
            e.should_not_be_null();
            e.GetType().should_be(typeof(Entity));
        };

        it["gets total entity count"] = () => {
            _pool.CreateEntity();
            _pool.Count.should_be(1);
        };

        it["doesn't have entities that were not created with CreateEntity()"] = () => {
            _pool.HasEntity(this.CreateEntity()).should_be_false();
        };

        it["has entities that were created with CreateEntity()"] = () => {
            _pool.HasEntity(_pool.CreateEntity()).should_be_true();
        };

        it["returns all created entities"] = () => {
            var e1 = _pool.CreateEntity();
            var e2 = _pool.CreateEntity();
            var entities = _pool.GetEntities();
            entities.should_contain(e1);
            entities.should_contain(e2);
            entities.Length.should_be(2);
        };

        it["destroys entity and removes it"] = () => {
            var e = _pool.CreateEntity();
            _pool.DestroyEntity(e);
            _pool.HasEntity(e).should_be_false();
        };

        it["destroys an entity and removes all its components"] = () => {
            var e = _pool.CreateEntity();
            e.AddComponentA();
            _pool.DestroyEntity(e);
            e.GetComponents().should_be_empty();
        };

        it["destroys all entities"] = () => {
            var e = _pool.CreateEntity();
            e.AddComponentA();
            _pool.CreateEntity();
            _pool.DestroyAllEntities();
            _pool.GetEntities().should_be_empty();
            e.GetComponents().should_be_empty();
        };

        it["caches entities"] = () => {
            _pool.CreateEntity();
            var entities1 = _pool.GetEntities();
            var entities2 = _pool.GetEntities();
            entities1.should_be_same(entities2);
            _pool.DestroyEntity(_pool.CreateEntity());
            _pool.GetEntities().should_not_be_same(entities1);
        };

        context["events"] = () => {

            it["dispatches OnEntityCreated when creating a new entity"] = () => {
                Pool eventPool = null;
                Entity eventEntity = null;
                _pool.OnEntityCreated += (pool, entity) => {
                    eventPool = pool;
                    eventEntity = entity;
                };

                var e = _pool.CreateEntity();
                eventPool.should_be_same(_pool);
                eventEntity.should_be_same(e);
            };

            it["dispatches OnEntityWillBeDestroyed when destroying a new entity"] = () => {
                var e = _pool.CreateEntity();
                e.AddComponentA();
                Pool eventPool = null;
                Entity eventEntity = null;
                _pool.OnEntityWillBeDestroyed += (pool, entity) => {
                    eventPool = pool;
                    eventEntity = entity;
                    entity.HasComponentA().should_be_true();
                };
                _pool.DestroyEntity(e);
                eventPool.should_be_same(_pool);
                eventEntity.should_be_same(e);
            };

            it["dispatches OnEntityDestroyed when destroying a new entity"] = () => {
                var e = _pool.CreateEntity();
                Pool eventPool = null;
                Entity eventEntity = null;
                _pool.OnEntityDestroyed += (pool, entity) => {
                    eventPool = pool;
                    eventEntity = entity;
                    entity.HasComponentA().should_be_false();
                };
                _pool.DestroyEntity(e);
                eventPool.should_be_same(_pool);
                eventEntity.should_be_same(e);
            };

            it["dispatches OnEntityDestroyed when destroying a new entity. The entity is released only after event dispatch is done"] = () => {
                var e = _pool.CreateEntity();
                Pool eventPool = null;
                Entity eventEntity = null;
                _pool.OnEntityDestroyed += (pool, entity) => {
                    eventPool = pool;
                    eventEntity = entity;
                    var newEntity = _pool.CreateEntity();
                    newEntity.should_not_be_null();
                    newEntity.should_not_be_same(eventEntity);
                };
                _pool.DestroyEntity(e);
                eventPool.should_be_same(_pool);
                var reusedEntity = _pool.CreateEntity();
                eventEntity.should_be_same(e);
                reusedEntity.should_be_same(e);
            };

            it["dispatches OnGroupCreated when creating a new group"] = () => {
                Pool eventPool = null;
                Group eventGroup = null;
                _pool.OnGroupCreated += (pool, g) => {
                    eventPool = pool;
                    eventGroup = g;
                };
                var group = _pool.GetGroup(Matcher.AllOf(0));
                eventPool.should_be_same(_pool);
                eventGroup.should_be_same(group);
            };

            it["doesn't dispatch OnGroupCreated when group alredy exists"] = () => {
                _pool.GetGroup(Matcher.AllOf(0));
                _pool.OnGroupCreated += (pool, g) => this.Fail();
                _pool.GetGroup(Matcher.AllOf(0));
            };

            it["removes all external delegates when destroying an entity"] = () => {
                var e = _pool.CreateEntity();
                e.OnComponentAdded += (entity, index, component) => this.Fail();
                e.OnComponentRemoved += (entity, index, component) => this.Fail();
                e.OnComponentReplaced += (entity, index, previousComponent, newComponent) => this.Fail();
                _pool.DestroyEntity(e);
                var e2 = _pool.CreateEntity();
                e2.should_be_same(e);
                e2.AddComponentA();
                e2.ReplaceComponentA(Component.A);
                e2.RemoveComponentA();
            };
        };

        context["entity pool"] = () => {

            it["gets entity from object pool"] = () => {
                var e = _pool.CreateEntity();
                e.should_not_be_null();
                e.GetType().should_be(typeof(Entity));
            };

            it["destroys entity when pushing back to object pool"] = () => {
                var e = _pool.CreateEntity();
                e.AddComponentA();
                _pool.DestroyEntity(e);
                e.HasComponent(CID.ComponentA).should_be_false();
            };

            it["returns pushed entity"] = () => {
                var e = _pool.CreateEntity();
                e.AddComponentA();
                _pool.DestroyEntity(e);
                var entity = _pool.CreateEntity();
                entity.HasComponent(CID.ComponentA).should_be_false();
                entity.should_be_same(e);
            };

            it["returns pushed entity only after observer is cleared"] = () => {
                var e = _pool.CreateEntity();
                var groupA = _pool.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
                var observer = new GroupObserver(groupA, GroupEventType.OnEntityAdded);
                e.AddComponentA();
                _pool.DestroyEntity(e);
                var entity1 = _pool.CreateEntity();
                entity1.HasComponent(CID.ComponentA).should_be_false();
                entity1.should_not_be_same(e);
                observer.ClearCollectedEntities();
                var entity2 = _pool.CreateEntity();
                entity2.should_be_same(e);
            };

            it["returns new entity"] = () => {
                var e = _pool.CreateEntity();
                e.AddComponentA();
                _pool.DestroyEntity(e);
                _pool.CreateEntity();
                var entityFromPool = _pool.CreateEntity();
                entityFromPool.HasComponent(CID.ComponentA).should_be_false();
                entityFromPool.should_not_be_same(e);
            };

            it["sets up entity from pool"] = () => {
                _pool.DestroyEntity(_pool.CreateEntity());                
                var g = _pool.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
                var e = _pool.CreateEntity();
                e.AddComponentA();
                g.GetEntities().should_contain(e);
            };

            context["when entity gets destroyed and pushed to object pool"] = () => {
                Entity e = null;
                before = () => {
                    e = _pool.CreateEntity();
                    e.AddComponentA();
                    _pool.DestroyEntity(e);
                };

                it["throws when adding component"] = expect<EntityIsNotEnabledException>(() => e.AddComponentA());
                it["throws when removing component"] = expect<EntityIsNotEnabledException>(() => e.RemoveComponentA());
                it["throws when replacing component"] = expect<EntityIsNotEnabledException>(() => e.ReplaceComponentA(new ComponentA()));
                it["throws when replacing component with null"] = expect<EntityIsNotEnabledException>(() => e.ReplaceComponentA(null));
            };
        };

        context["get entities"] = () => {

            it["gets empty group for matcher when no entities were created"] = () => {
                var g = _pool.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
                g.should_not_be_null();
                g.GetEntities().should_be_empty();
            };

            context["when entities created"] = () => {
                Entity eAB1 = null;
                Entity eAB2 = null;
                Entity eA = null;

                IMatcher matcher = Matcher.AllOf(new [] {
                    CID.ComponentA,
                    CID.ComponentB
                });

                before = () => {
                    eAB1 = _pool.CreateEntity();
                    eAB1.AddComponentA();
                    eAB1.AddComponentB();
                    eAB2 = _pool.CreateEntity();
                    eAB2.AddComponentA();
                    eAB2.AddComponentB();
                    eA = _pool.CreateEntity();
                    eA.AddComponentA();
                };

                it["gets group with matching entities"] = () => {
                    var g = _pool.GetGroup(matcher).GetEntities();
                    g.Length.should_be(2);
                    g.should_contain(eAB1);
                    g.should_contain(eAB2);
                };

                it["gets cached group"] = () => {
                    _pool.GetGroup(matcher).should_be_same(_pool.GetGroup(matcher));
                };

                it["cached group contains newly created matching entity"] = () => {
                    var g = _pool.GetGroup(matcher);
                    eA.AddComponentB();
                    g.GetEntities().should_contain(eA);
                };

                it["cached group doesn't contain entity which are not matching anymore"] = () => {
                    var g = _pool.GetGroup(matcher);
                    eAB1.RemoveComponentA();
                    g.GetEntities().should_not_contain(eAB1);
                };

                it["removes destroyed entity"] = () => {
                    var g = _pool.GetGroup(matcher);
                    _pool.DestroyEntity(eAB1);
                    g.GetEntities().should_not_contain(eAB1);
                };

                it["throws when destroying an entity the pool doesn't contain"] = expect<PoolDoesNotContainEntityException>(() => {
                    var e = _pool.CreateEntity();
                    _pool.DestroyEntity(e);
                    _pool.DestroyEntity(e);
                });

                it["group dispatches OnEntityRemoved and OnEntityAdded when replacing components"] = () => {
                    var g = _pool.GetGroup(matcher);
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
                    var g = _pool.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
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
            };
        };
    }
}

