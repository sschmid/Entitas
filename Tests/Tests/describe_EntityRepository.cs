using NSpec;
using Entitas;

class describe_EntityRepository : nspec {
    EntityRepository _repo;

    void before_each() {
        _repo = new EntityRepository(CP.NumComponents);
    }

    void it_creates_entities_starting_with_specified_creation_index() {
        new EntityRepository(0, 1).CreateEntity().creationIndex.should_be(1);
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

        it["increments creation index when creating entities"] = () => {
            _repo.CreateEntity().creationIndex.should_be(0);
            _repo.CreateEntity().creationIndex.should_be(1);
        };

        it["doesn't have entites that were not created with CreateEntity()"] = () => {
            _repo.HasEntity(new Entity(CP.NumComponents)).should_be_false();
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
            addComponentA(e);
            _repo.DestroyEntity(e);
            e.GetComponents().should_be_empty();
        };

        it["destroys all entites"] = () => {
            var e = _repo.CreateEntity();
            addComponentA(e);
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

        context["get entities"] = () => {

            it["gets empty collection for matcher when no entities were created"] = () => {
                var c = _repo.GetCollection(EntityMatcher.AllOf(new [] { CP.ComponentA }));
                c.should_not_be_null();
                c.GetEntities().should_be_empty();
            };

            context["when entities created"] = () => {
                Entity eAB1 = null;
                Entity eAB2 = null;
                Entity eA = null;

                IEntityMatcher matcher = EntityMatcher.AllOf(new [] {
                    CP.ComponentA,
                    CP.ComponentB
                });

                before = () => {
                    eAB1 = _repo.CreateEntity();
                    addComponentA(eAB1);
                    addComponentB(eAB1);
                    eAB2 = _repo.CreateEntity();
                    addComponentA(eAB2);
                    addComponentB(eAB2);
                    eA = _repo.CreateEntity();
                    addComponentA(eA);
                };

                it["gets collection with matching entities"] = () => {
                    var c = _repo.GetCollection(matcher).GetEntities();
                    c.should_not_be_empty();
                    c.Length.should_be(2);
                    c.should_contain(eAB1);
                    c.should_contain(eAB2);
                };

                it["gets cached collection"] = () => {
                    _repo.GetCollection(matcher).should_be_same(_repo.GetCollection(matcher));
                };

                it["cached collection contains newly created matching entity"] = () => {
                    var c = _repo.GetCollection(matcher);
                    addComponentB(eA);
                    c.GetEntities().should_contain(eA);
                };

                it["cached collection doesn't contain entity which are not matching anymore"] = () => {
                    var c = _repo.GetCollection(matcher);
                    removeComponentA(eAB1);
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
                    addComponentA(eA);
                    addComponentB(eA);
                    c.GetEntities().should_not_contain(eA);
                };

                it["collections dispatches OnEntityRemoved and OnEntityAdded when replacing components"] = () => {
                    var c = _repo.GetCollection(matcher);
                    var didDispachRemoved = 0;
                    var didDispachAdded = 0;
                    EntityCollection eventCollectionRemoved = null;
                    EntityCollection eventCollectionAdded = null;
                    Entity eventEntityRemoved = null;
                    Entity eventEntityAdded = null;
                    c.OnEntityRemoved += (collection, entity) => {
                        eventCollectionRemoved = collection;
                        eventEntityRemoved = entity;
                        didDispachRemoved++;
                    };
                    c.OnEntityAdded += (collection, entity) => {
                        eventCollectionAdded = collection;
                        eventEntityAdded = entity;
                        didDispachAdded++;
                    };
                    eAB1.ReplaceComponent(CP.ComponentA, new ComponentA());

                    eventCollectionRemoved.should_be_same(c);
                    eventCollectionAdded.should_be_same(c);
                    eventEntityRemoved.should_be_same(eAB1);
                    eventEntityAdded.should_be_same(eAB1);
                    didDispachRemoved.should_be(1);
                    didDispachAdded.should_be(1);
                    didDispachAdded.should_be(1);
                };
            };
        };
    }

    void addComponentA(Entity entity) {
        entity.AddComponent(CP.ComponentA, new ComponentA());
    }

    void addComponentB(Entity entity) {
        entity.AddComponent(CP.ComponentB, new ComponentB());
    }

    void removeComponentA(Entity entity) {
        entity.RemoveComponent(CP.ComponentA);
    }
}

