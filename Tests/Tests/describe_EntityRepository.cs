using NSpec;
using Entitas;

class describe_EntityRepository : nspec {
    EntityRepository _repo;

    void before_each() {
        _repo = new EntityRepository(CID.NumComponents);
    }

    void when_created() {

        it["has no entities when no entities were created"] = () => {
            _repo.GetEntities().should_be_empty();
        };

        it["creates entity"] = () => {
            var e = _repo.CreateEntity();
            e.should_not_be_null();
            e.GetType().should_be(typeof(Entity));
        };

        it["doesn't have entites that were not created with CreateEntity()"] = () => {
            _repo.HasEntity(this.CreateEntity()).should_be_false();
        };

        it["has entites that were created with CreateEntity()"] = () => {
            _repo.HasEntity(_repo.CreateEntity()).should_be_true();
        };

        it["returns all created entities"] = () => {
            var e1 = _repo.CreateEntity();
            var e2 = _repo.CreateEntity();
            var entities = _repo.GetEntities();
            entities.should_contain(e1);
            entities.should_contain(e2);
            entities.Length.should_be(2);
        };

        it["destroys entity and removes it"] = () => {
            var e = _repo.CreateEntity();
            _repo.DestroyEntity(e);
            _repo.HasEntity(e).should_be_false();
        };

        it["destroys an entity and removes all its components"] = () => {
            var e = _repo.CreateEntity();
            e.AddComponentA();
            _repo.DestroyEntity(e);
            e.GetComponents().should_be_empty();
        };

        it["destroys all entites"] = () => {
            var e = _repo.CreateEntity();
            e.AddComponentA();
            _repo.CreateEntity();
            _repo.DestroyAllEntities();
            _repo.GetEntities().should_be_empty();
            e.GetComponents().should_be_empty();
        };

        it["caches entities"] = () => {
            _repo.CreateEntity();
            var entities1 = _repo.GetEntities();
            var entities2 = _repo.GetEntities();
            entities1.should_be_same(entities2);
            _repo.DestroyEntity(_repo.CreateEntity());
            _repo.GetEntities().should_not_be_same(entities1);
        };

        context["entity pool"] = () => {

            it["gets entity from object pool"] = () => {
                var e = _repo.CreateEntity();
                e.should_not_be_null();
                e.GetType().should_be(typeof(Entity));
            };

            it["destroys entity when pushing back to object pool"] = () => {
                var e = this.CreateEntity();
                e.AddComponentA();
                _repo.DestroyEntity(e);
                e.HasComponent(CID.ComponentA).should_be_false();
            };

            it["returns pushed entity"] = () => {
                var e = this.CreateEntity();
                e.AddComponentA();
                _repo.DestroyEntity(e);
                var entity = _repo.CreateEntity();
                entity.HasComponent(CID.ComponentA).should_be_false();
                entity.should_be_same(e);
            };

            it["returns new entity"] = () => {
                var e = this.CreateEntity();
                e.AddComponentA();
                _repo.DestroyEntity(e);
                _repo.CreateEntity();
                var entityFromPool = _repo.CreateEntity();
                entityFromPool.HasComponent(CID.ComponentA).should_be_false();
                entityFromPool.should_not_be_same(e);
            };

            it["sets up entity from pool"] = () => {
                _repo.DestroyEntity(_repo.CreateEntity());                
                var c = _repo.GetCollection(Matcher.AllOf(new [] { CID.ComponentA }));
                var e = _repo.CreateEntity();
                e.AddComponentA();
                c.GetEntities().should_contain(e);
            };
        };

        context["get entities"] = () => {

            it["gets empty collection for matcher when no entities were created"] = () => {
                var c = _repo.GetCollection(Matcher.AllOf(new [] { CID.ComponentA }));
                c.should_not_be_null();
                c.GetEntities().should_be_empty();
            };

            context["when entities created"] = () => {
                Entity eAB1 = null;
                Entity eAB2 = null;
                Entity eA = null;

                IEntityMatcher matcher = Matcher.AllOf(new [] {
                    CID.ComponentA,
                    CID.ComponentB
                });

                before = () => {
                    eAB1 = _repo.CreateEntity();
                    eAB1.AddComponentA();
                    eAB1.AddComponentB();
                    eAB2 = _repo.CreateEntity();
                    eAB2.AddComponentA();
                    eAB2.AddComponentB();
                    eA = _repo.CreateEntity();
                    eA.AddComponentA();
                };

                it["gets collection with matching entities"] = () => {
                    var c = _repo.GetCollection(matcher).GetEntities();
                    c.Length.should_be(2);
                    c.should_contain(eAB1);
                    c.should_contain(eAB2);
                };

                it["gets cached collection"] = () => {
                    _repo.GetCollection(matcher).should_be_same(_repo.GetCollection(matcher));
                };

                it["cached collection contains newly created matching entity"] = () => {
                    var c = _repo.GetCollection(matcher);
                    eA.AddComponentB();
                    c.GetEntities().should_contain(eA);
                };

                it["cached collection doesn't contain entity which are not matching anymore"] = () => {
                    var c = _repo.GetCollection(matcher);
                    eAB1.RemoveComponentA();
                    c.GetEntities().should_not_contain(eAB1);
                };

                it["removes destroyed entity"] = () => {
                    var c = _repo.GetCollection(matcher);
                    _repo.DestroyEntity(eAB1);
                    c.GetEntities().should_not_contain(eAB1);
                };

                it["ignores adding components to destroyed entity"] = () => {
                    var c = _repo.GetCollection(matcher);
                    _repo.DestroyEntity(eA);
                    eA.AddComponentA();
                    eA.AddComponentB();
                    c.GetEntities().should_not_contain(eA);
                };

                it["collections dispatches OnEntityWillBeRemoved, OnEntityRemoved and OnEntityAdded when replacing components"] = () => {
                    var c = _repo.GetCollection(matcher);
                    var didDispatchWillBeRemoved = 0;
                    var didDispatchRemoved = 0;
                    var didDispatchAdded = 0;
                    EntityCollection eventCollectionWillBeRemoved = null;
                    EntityCollection eventCollectionRemoved = null;
                    EntityCollection eventCollectionAdded = null;
                    Entity eventEntityWillBeRemoved = null;
                    Entity eventEntityRemoved = null;
                    Entity eventEntityAdded = null;
                    c.OnEntityWillBeRemoved += (collection, entity) => {
                        eventCollectionWillBeRemoved = collection;
                        eventEntityWillBeRemoved = entity;
                        didDispatchWillBeRemoved++;
                    };
                    c.OnEntityRemoved += (collection, entity) => {
                        eventCollectionRemoved = collection;
                        eventEntityRemoved = entity;
                        didDispatchRemoved++;
                    };
                    c.OnEntityAdded += (collection, entity) => {
                        eventCollectionAdded = collection;
                        eventEntityAdded = entity;
                        didDispatchAdded++;
                    };
                    eAB1.WillRemoveComponent(CID.ComponentA);
                    eAB1.ReplaceComponentA(new ComponentA());

                    eventCollectionWillBeRemoved.should_be_same(c);
                    eventCollectionRemoved.should_be_same(c);
                    eventCollectionAdded.should_be_same(c);
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

