using NSpec;
using Entitas;

class describe_EntityCollection : nspec {
    EntityCollection _collection;
    Entity _eA1;
    Entity _eA2;

    void before_each() {
        _collection = new EntityCollection(EntityMatcher.AllOf(new [] { CP.ComponentA }));
        _eA1 = createEntity();
        addComponentA(_eA1);
        _eA2 = createEntity();
        addComponentA(_eA2);
    }

    void when_created() {
        it["doesn't have entites which haven't been added"] = () => {
            _collection.GetEntities().should_be_empty();
        };

        it["adds matching entity"] = () => {
            addEA1();
            _collection.GetEntities().should_contain(_eA1);
        };

        it["doesn't add same entity twice"] = () => {
            addEA1();
            addEA1();
            _collection.GetEntities().should_contain(_eA1);
            _collection.GetEntities().Length.should_be(1);
        };

        it["doesn't add entity when not matching"] = () => {
            var e = createEntity();
            addComponentB(e);
            add(e);
            _collection.GetEntities().should_be_empty();
        };

        it["removes entity"] = () => {
            addEA1();
            removeEA1();
            _collection.GetEntities().should_be_empty();
        };

        it["replaces existing entity"] = () => {
            addEA1();
            replaceEA1();
            _collection.GetEntities().should_contain(_eA1);
        };

        it["doesn't add entity when replacing with an entity that hasn't been added before"] = () => {
            replaceEA1();
            _collection.GetEntities().should_not_contain(_eA1);
        };

        it["gets null when single entity does not exist"] = () => {
            _collection.GetSingleEntity().should_be_null();
        };

        it["gets single entity"] = () => {
            addEA1();
            _collection.GetSingleEntity().should_be_same(_eA1);
        };

        it["throws when attempting to get single entity and multiple matching entites exist"] = expect<SingleEntityException>(() => {
            addEA1();
            add(_eA2);
            _collection.GetSingleEntity();
        });

        context["events"] = () => {
            var didDispatch = 0;

            before = () => {
                didDispatch = 0;
            };

            it["dispatches OnEntityAdded when matching entity added"] = () => {
                _collection.OnEntityAdded += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_eA1);
                };

                addEA1();
                didDispatch.should_be(1);
            };

            it["doesn't dispatches OnEntityAdded when matching entity already has been added"] = () => {
                _collection.OnEntityAdded += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_eA1);
                };

                addEA1();
                addEA1();
                didDispatch.should_be(1);
            };

            it["dispatches OnEntityRemove when entity got removed"] = () => {
                _collection.OnEntityRemoved += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_eA1);
                };

                addEA1();
                removeEA1();
                didDispatch.should_be(1);
            };

            it["doesn't dispatch OnEntityRemove when entity didn't get removed"] = () => {
                _collection.OnEntityRemoved += (collection, entity) => {
                    didDispatch++;
                };

                removeEA1();
                didDispatch.should_be(0);
            };

            it["dispatches OnEntityRemoved and OnEntityAdded when entity got replaced"] = () => {
                addEA1();
                var didDispatchAdded = 0;
                var didDispatchRemoved = 0;
                EntityCollection eventCollectionAdded = null;
                EntityCollection eventCollectionRemoved = null;
                Entity eventEntityAdded = null;
                Entity eventEntityRemoved = null;
                _collection.OnEntityAdded += (collection, entity) => {
                    eventCollectionAdded = collection;
                    eventEntityAdded = entity;
                    didDispatchAdded++;
                };
                _collection.OnEntityRemoved += (collection, entity) => {
                    eventCollectionRemoved = collection;
                    eventEntityRemoved = entity;
                    didDispatchRemoved++;
                };
                _collection.ReplaceEntity(_eA1);

                didDispatchAdded.should_be(1);
                didDispatchRemoved.should_be(1);
                eventCollectionAdded.should_be_same(_collection);
                eventCollectionRemoved.should_be_same(_collection);
                eventEntityAdded.should_be_same(_eA1);
                eventEntityRemoved.should_be_same(_eA1);
            };
        };

        context["internal caching"] = () => {
            it["gets cached entities"] = () => {
                addEA1();
                _collection.GetEntities().should_be_same(_collection.GetEntities());
            };

            it["updates cache when adding a new matching entity"] = () => {
                addEA1();
                var c = _collection.GetEntities();
                add(_eA2);
                c.should_not_be_same(_collection.GetEntities());
            };

            it["doesn't update cache when attempting to add a not matching entity"] = () => {
                addEA1();
                var c = _collection.GetEntities();
                var e = createEntity();
                add(e);
                c.should_be_same(_collection.GetEntities());
            };
        
            it["updates cache when removing an entity"] = () => {
                addEA1();
                var c = _collection.GetEntities();
                removeEA1();
                c.should_not_be_same(_collection.GetEntities());
            };

            it["doesn't update cache when attempting to remove an entity that wasn't added before"] = () => {
                var c = _collection.GetEntities();
                removeEA1();
                c.should_be_same(_collection.GetEntities());
            };

            it["gets cached singleEntities"] = () => {
                addEA1();
                _collection.GetSingleEntity().should_be_same(_collection.GetSingleEntity());
            };

            it["updates cache when new single entity was added"] = () => {
                addEA1();
                var s = _collection.GetSingleEntity();
                removeEA1();
                add(_eA2);
                s.should_not_be_same(_collection.GetSingleEntity());
            };

            it["updates cache when single entity is removed"] = () => {
                addEA1();
                var s = _collection.GetSingleEntity();
                removeEA1();
                s.should_not_be_same(_collection.GetSingleEntity());
            };

            it["doesn't update cache when replacing an entity"] = () => {
                addEA1();
                var s = _collection.GetSingleEntity();
                var e = _collection.GetEntities();
                replaceEA1();
                s.should_be_same(_collection.GetSingleEntity());
                e.should_be_same(_collection.GetEntities());
            };
        };
    }

    Entity createEntity() {
        return new Entity(CP.NumComponents);
    }

    void addComponentA(Entity entity) {
        entity.AddComponent(CP.ComponentA, new ComponentA());
    }

    void addComponentB(Entity entity) {
        entity.AddComponent(CP.ComponentB, new ComponentB());
    }

    void addEA1() {
        _collection.AddEntityIfMatching(_eA1);
    }

    void add(Entity entity) {
        _collection.AddEntityIfMatching(entity);
    }

    void removeEA1() {
        _collection.RemoveEntity(_eA1);
    }

    void replaceEA1() {
        _collection.ReplaceEntity(_eA1);
    }
}

