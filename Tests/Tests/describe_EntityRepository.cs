using NSpec;
using Entitas;

class describe_EntityRepository : nspec {
    EntityRepository _repo;

    void before_each() {
        _repo = new EntityRepository();
    }

    void it_creates_entities_starting_with_specified_creation_index() {
        const int i = 1;
        new EntityRepository(i).CreateEntity().creationIndex.should_be(i);
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
            _repo.HasEntity(new Entity()).should_be_false();
        };

        it["has entites that were created with CreateEntity()"] = () => {
            var e = _repo.CreateEntity();
            _repo.HasEntity(e).should_be_true();
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
            e.AddComponent(new ComponentA());
            _repo.DestroyEntity(e);
            e.GetComponents().should_be_empty();
        };

        it["destroys all entites"] = () => {
            var e = _repo.CreateEntity();
            e.AddComponent(new ComponentA());
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
                var c = _repo.GetCollection(EntityMatcher.AllOf(new [] { typeof(ComponentA) }));
                c.should_not_be_null();
                c.GetEntities().should_be_empty();
            };

            Entity e1 = null;
            Entity e2 = null;
            Entity e3 = null;
            IEntityMatcher matcher = EntityMatcher.AllOf(new [] {
                typeof(ComponentA),
                typeof(ComponentB)
            });
            context["when entities created"] = () => {
                before = () => {
                    e1 = _repo.CreateEntity();
                    e1.AddComponent(new ComponentA());
                    e1.AddComponent(new ComponentB());
                    e2 = _repo.CreateEntity();
                    e2.AddComponent(new ComponentA());
                    e2.AddComponent(new ComponentB());
                    e3 = _repo.CreateEntity();
                    e3.AddComponent(new ComponentA());
                };

                it["gets collection with matching entities"] = () => {
                    var c = _repo.GetCollection(matcher).GetEntities();
                    c.should_not_be_empty();
                    c.Length.should_be(2);
                    c.should_contain(e1);
                    c.should_contain(e2);
                };

                it["gets cached collection"] = () => {
                    _repo.GetCollection(matcher).should_be_same(_repo.GetCollection(matcher));
                };

                it["cached collection contains newly created matching entity"] = () => {
                    var c = _repo.GetCollection(matcher);
                    e3.AddComponent(new ComponentB());
                    c.GetEntities().should_contain(e3);
                };

                it["cached collection doesn't contain entity which are not matching anymore"] = () => {
                    var c = _repo.GetCollection(matcher);
                    e1.RemoveComponent(typeof(ComponentA));
                    c.GetEntities().should_not_contain(e1);
                };

                it["removes destroyed entity"] = () => {
                    var c = _repo.GetCollection(matcher);
                    _repo.DestroyEntity(e1);
                    c.GetEntities().should_not_contain(e1);
                };

                it["ignores adding components to destroyed entity"] = () => {
                    var c = _repo.GetCollection(matcher);
                    _repo.DestroyEntity(e3);
                    e3.AddComponent(new ComponentA());
                    e3.AddComponent(new ComponentB());
                    c.GetEntities().should_not_contain(e3);
                };
            };
        };
    }
}

