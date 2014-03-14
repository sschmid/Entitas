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

    void when_repo_created() {

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
            it["doesn't get entities when no entities were created"] = () => {
                _repo.GetEntities(EntityMatcher.AllOf(new [] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                })).should_be_empty();
            };

            it["gets created entities matching allOf matcher"] = () => {
                var e1 = _repo.CreateEntity();
                e1.AddComponent(new ComponentA());
                e1.AddComponent(new ComponentB());
                var e2 = _repo.CreateEntity();
                e2.AddComponent(new ComponentA());
                var entities = _repo.GetEntities(EntityMatcher.AllOf(new[] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                }));

                entities.should_contain(e1);
                entities.should_not_contain(e2);
                entities.Length.should_be(1);
            };

            it["gets created entities matching anyOf matcher"] = () => {
                var e1 = _repo.CreateEntity();
                e1.AddComponent(new ComponentA());
                e1.AddComponent(new ComponentB());
                var e2 = _repo.CreateEntity();
                e2.AddComponent(new ComponentA());
                var entities = _repo.GetEntities(EntityMatcher.AnyOf(new[] {
                    typeof(ComponentA),
                    typeof(ComponentB)
                }));

                entities.should_contain(e1);
                entities.should_contain(e2);
                entities.Length.should_be(2);
            };
        };
    }
}

