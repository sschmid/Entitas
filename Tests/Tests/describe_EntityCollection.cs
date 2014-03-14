using NSpec;
using Entitas;

class describe_EntityCollection : nspec {
    EntityCollection _collection;
    Entity _e;

    void before_each() {
        _collection = new EntityCollection(EntityMatcher.AllOf(new [] { typeof(ComponentA) }));
        _e = new Entity();
        _e.AddComponent(new ComponentA());
    }

    void when_collection_created() {
        it["doesn't have entites which haven't been added"] = () => {
            _collection.GetEntities().should_be_empty();
        };

        it["adds matching entity"] = () => {
            _collection.AddEntityIfMatching(_e);
            _collection.GetEntities().should_contain(_e);
        };

        it["doesn't add same entity twice"] = () => {
            _collection.AddEntityIfMatching(_e);
            _collection.AddEntityIfMatching(_e);
            _collection.GetEntities().should_contain(_e);
            _collection.GetEntities().Length.should_be(1);
        };

        it["doesn't add entity when not matching"] = () => {
            var e = new Entity();
            e.AddComponent(new ComponentB());
            _collection.AddEntityIfMatching(e);
            _collection.GetEntities().should_be_empty();
        };

        it["removes entity"] = () => {
            _collection.AddEntityIfMatching(_e);
            _collection.RemoveEntity(_e);
            _collection.GetEntities().should_be_empty();
        };

        it["gets null when single entity does not exist"] = () => {
            _collection.GetSingleEntity().should_be_null();
        };

        it["gets single entity"] = () => {
            _collection.AddEntityIfMatching(_e);
            _collection.GetSingleEntity().should_be_same(_e);
        };

        it["throws when attempting to get single entity and multiple matching entites exist"] = expect<SingleEntityException>(() => {
            _collection.AddEntityIfMatching(_e);
            var e = new Entity();
            e.AddComponent(new ComponentA());
            _collection.AddEntityIfMatching(e);
            _collection.GetSingleEntity();
        });

        context["events"] = () => {
            it["dispatches OnEntityAdded when matching entity added"] = () => {
                var didDispatch = 0;
                _collection.OnEntityAdded += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_e);
                };

                _collection.AddEntityIfMatching(_e);
                didDispatch.should_be(1);
            };

            it["doesn't dispatches OnEntityAdded when matching entity already has been added"] = () => {
                var didDispatch = 0;
                _collection.OnEntityAdded += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_e);
                };

                _collection.AddEntityIfMatching(_e);
                _collection.AddEntityIfMatching(_e);
                didDispatch.should_be(1);
            };

            it["dispatches OnEntityRemove when entity got removed"] = () => {
                var didDispatch = 0;
                _collection.OnEntityRemoved += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_e);
                };

                _collection.AddEntityIfMatching(_e);
                _collection.RemoveEntity(_e);
                didDispatch.should_be(1);
            };

            it["doesn't dispatch OnEntityRemove when entity didn't get removed"] = () => {
                var didDispatch = 0;
                _collection.OnEntityRemoved += (collection, entity) => {
                    didDispatch++;
                };

                _collection.RemoveEntity(_e);
                didDispatch.should_be(0);
            };
        };
    }
}

