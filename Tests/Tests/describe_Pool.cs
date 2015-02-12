using NSpec;
using Entitas;

class describe_Pool : nspec {
    Pool _pool;

    void before_each() {
        _pool = new Pool(CID.NumComponents);
    }

    void when_created() {

        it["has no entities when no entities were created"] = () => {
            _pool.GetEntities().should_be_empty();
        };

        it["creates entity"] = () => {
            var e = _pool.CreateEntity();
            e.should_not_be_null();
            e.GetType().should_be(typeof(Entity));
        };

        it["doesn't have entites that were not created with CreateEntity()"] = () => {
            _pool.HasEntity(this.CreateEntity()).should_be_false();
        };

        it["has entites that were created with CreateEntity()"] = () => {
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

        it["destroys all entites"] = () => {
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

        context["entity pool"] = () => {

            it["gets entity from object pool"] = () => {
                var e = _pool.CreateEntity();
                e.should_not_be_null();
                e.GetType().should_be(typeof(Entity));
            };

            it["destroys entity when pushing back to object pool"] = () => {
                var e = this.CreateEntity();
                e.AddComponentA();
                _pool.DestroyEntity(e);
                e.HasComponent(CID.ComponentA).should_be_false();
            };

            it["returns pushed entity"] = () => {
                var e = this.CreateEntity();
                e.AddComponentA();
                _pool.DestroyEntity(e);
                var entity = _pool.CreateEntity();
                entity.HasComponent(CID.ComponentA).should_be_false();
                entity.should_be_same(e);
            };

            it["returns new entity"] = () => {
                var e = this.CreateEntity();
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

                it["ignores adding components to destroyed entity"] = () => {
                    var g = _pool.GetGroup(matcher);
                    _pool.DestroyEntity(eA);
                    eA.AddComponentA();
                    eA.AddComponentB();
                    g.GetEntities().should_not_contain(eA);
                };

                it["group dispatches OnEntityWillBeRemoved, OnEntityRemoved and OnEntityAdded when replacing components"] = () => {
                    var g = _pool.GetGroup(matcher);
                    var didDispatchWillBeRemoved = 0;
                    var didDispatchRemoved = 0;
                    var didDispatchAdded = 0;
                    Group eventGroupWillBeRemoved = null;
                    Group eventGroupRemoved = null;
                    Group eventGroupAdded = null;
                    Entity eventEntityWillBeRemoved = null;
                    Entity eventEntityRemoved = null;
                    Entity eventEntityAdded = null;
                    g.OnEntityWillBeRemoved += (group, entity) => {
                        eventGroupWillBeRemoved = group;
                        eventEntityWillBeRemoved = entity;
                        didDispatchWillBeRemoved++;
                    };
                    g.OnEntityRemoved += (group, entity) => {
                        eventGroupRemoved = group;
                        eventEntityRemoved = entity;
                        didDispatchRemoved++;
                    };
                    g.OnEntityAdded += (group, entity) => {
                        eventGroupAdded = group;
                        eventEntityAdded = entity;
                        didDispatchAdded++;
                    };
                    eAB1.WillRemoveComponent(CID.ComponentA);
                    eAB1.ReplaceComponentA(new ComponentA());

                    eventGroupWillBeRemoved.should_be_same(g);
                    eventGroupRemoved.should_be_same(g);
                    eventGroupAdded.should_be_same(g);
                    eventEntityWillBeRemoved.should_be_same(eAB1);
                    eventEntityRemoved.should_be_same(eAB1);
                    eventEntityAdded.should_be_same(eAB1);
                    didDispatchWillBeRemoved.should_be(1);
                    didDispatchRemoved.should_be(1);
                    didDispatchAdded.should_be(1);
                };
            };
        };
    }
}

