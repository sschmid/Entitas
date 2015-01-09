using NSpec;
using Entitas;

class describe_Context : nspec {
    Context _ctx;

    void before_each() {
        _ctx = new Context(CID.NumComponents);
    }

    void when_created() {

        it["has no entities when no entities were created"] = () => {
            _ctx.GetEntities().should_be_empty();
        };

        it["creates entity"] = () => {
            var e = _ctx.CreateEntity();
            e.should_not_be_null();
            e.GetType().should_be(typeof(Entity));
        };

        it["doesn't have entites that were not created with CreateEntity()"] = () => {
            _ctx.HasEntity(this.CreateEntity()).should_be_false();
        };

        it["has entites that were created with CreateEntity()"] = () => {
            _ctx.HasEntity(_ctx.CreateEntity()).should_be_true();
        };

        it["returns all created entities"] = () => {
            var e1 = _ctx.CreateEntity();
            var e2 = _ctx.CreateEntity();
            var entities = _ctx.GetEntities();
            entities.should_contain(e1);
            entities.should_contain(e2);
            entities.Length.should_be(2);
        };

        it["destroys entity and removes it"] = () => {
            var e = _ctx.CreateEntity();
            _ctx.DestroyEntity(e);
            _ctx.HasEntity(e).should_be_false();
        };

        it["destroys an entity and removes all its components"] = () => {
            var e = _ctx.CreateEntity();
            e.AddComponentA();
            _ctx.DestroyEntity(e);
            e.GetComponents().should_be_empty();
        };

        it["destroys all entites"] = () => {
            var e = _ctx.CreateEntity();
            e.AddComponentA();
            _ctx.CreateEntity();
            _ctx.DestroyAllEntities();
            _ctx.GetEntities().should_be_empty();
            e.GetComponents().should_be_empty();
        };

        it["caches entities"] = () => {
            _ctx.CreateEntity();
            var entities1 = _ctx.GetEntities();
            var entities2 = _ctx.GetEntities();
            entities1.should_be_same(entities2);
            _ctx.DestroyEntity(_ctx.CreateEntity());
            _ctx.GetEntities().should_not_be_same(entities1);
        };

        context["entity pool"] = () => {

            it["gets entity from object pool"] = () => {
                var e = _ctx.CreateEntity();
                e.should_not_be_null();
                e.GetType().should_be(typeof(Entity));
            };

            it["destroys entity when pushing back to object pool"] = () => {
                var e = this.CreateEntity();
                e.AddComponentA();
                _ctx.DestroyEntity(e);
                e.HasComponent(CID.ComponentA).should_be_false();
            };

            it["returns pushed entity"] = () => {
                var e = this.CreateEntity();
                e.AddComponentA();
                _ctx.DestroyEntity(e);
                var entity = _ctx.CreateEntity();
                entity.HasComponent(CID.ComponentA).should_be_false();
                entity.should_be_same(e);
            };

            it["returns new entity"] = () => {
                var e = this.CreateEntity();
                e.AddComponentA();
                _ctx.DestroyEntity(e);
                _ctx.CreateEntity();
                var entityFromPool = _ctx.CreateEntity();
                entityFromPool.HasComponent(CID.ComponentA).should_be_false();
                entityFromPool.should_not_be_same(e);
            };

            it["sets up entity from pool"] = () => {
                _ctx.DestroyEntity(_ctx.CreateEntity());                
                var g = _ctx.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
                var e = _ctx.CreateEntity();
                e.AddComponentA();
                g.GetEntities().should_contain(e);
            };
        };

        context["get entities"] = () => {

            it["gets empty group for matcher when no entities were created"] = () => {
                var g = _ctx.GetGroup(Matcher.AllOf(new [] { CID.ComponentA }));
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
                    eAB1 = _ctx.CreateEntity();
                    eAB1.AddComponentA();
                    eAB1.AddComponentB();
                    eAB2 = _ctx.CreateEntity();
                    eAB2.AddComponentA();
                    eAB2.AddComponentB();
                    eA = _ctx.CreateEntity();
                    eA.AddComponentA();
                };

                it["gets group with matching entities"] = () => {
                    var g = _ctx.GetGroup(matcher).GetEntities();
                    g.Length.should_be(2);
                    g.should_contain(eAB1);
                    g.should_contain(eAB2);
                };

                it["gets cached group"] = () => {
                    _ctx.GetGroup(matcher).should_be_same(_ctx.GetGroup(matcher));
                };

                it["cached group contains newly created matching entity"] = () => {
                    var g = _ctx.GetGroup(matcher);
                    eA.AddComponentB();
                    g.GetEntities().should_contain(eA);
                };

                it["cached group doesn't contain entity which are not matching anymore"] = () => {
                    var g = _ctx.GetGroup(matcher);
                    eAB1.RemoveComponentA();
                    g.GetEntities().should_not_contain(eAB1);
                };

                it["removes destroyed entity"] = () => {
                    var g = _ctx.GetGroup(matcher);
                    _ctx.DestroyEntity(eAB1);
                    g.GetEntities().should_not_contain(eAB1);
                };

                it["ignores adding components to destroyed entity"] = () => {
                    var g = _ctx.GetGroup(matcher);
                    _ctx.DestroyEntity(eA);
                    eA.AddComponentA();
                    eA.AddComponentB();
                    g.GetEntities().should_not_contain(eA);
                };

                it["group dispatches OnEntityWillBeRemoved, OnEntityRemoved and OnEntityAdded when replacing components"] = () => {
                    var g = _ctx.GetGroup(matcher);
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

