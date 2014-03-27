using NSpec;
using Entitas;

class describe_EntityCollection : nspec {
    EntityCollection _collection;
    Entity _e1;
    Entity _e2;
    ComponentA _componentA;

    void fail() {
        true.should_be_false();
    }

    void before_each() {
        _collection = new EntityCollection(EntityMatcher.AllOf(new [] { typeof(ComponentA) }));
        _componentA = new ComponentA();
        _e1 = new Entity();
        _e1.AddComponent(_componentA);
        _e2 = new Entity();
        _e2.AddComponent(_componentA);
    }

    void when_created() {
        it["doesn't have entites which haven't been added"] = () => {
            _collection.GetEntities().should_be_empty();
        };

        it["adds matching entity"] = () => {
            _collection.HandleEntity(_e1);
            _collection.GetEntities().should_contain(_e1);
        };

        it["doesn't add same entity twice"] = () => {
            _collection.HandleEntity(_e1);
            _collection.HandleEntity(_e1);
            _collection.GetEntities().should_contain(_e1);
            _collection.GetEntities().Length.should_be(1);
        };

        it["doesn't add entity when not matching"] = () => {
            var e = new Entity();
            e.AddComponent(new ComponentB());
            _collection.HandleEntity(e);
            _collection.GetEntities().should_be_empty();
        };

        it["removes entity"] = () => {
            _collection.HandleEntity(_e1);
            _collection.RemoveEntity(_e1);
            _collection.GetEntities().should_be_empty();
        };

        it["removed entity when not matching"] = () => {
            _collection.HandleEntity(_e1);
            _e1.RemoveComponent(typeof(ComponentA));
            _collection.HandleEntity(_e1);
            _collection.GetEntities().should_not_contain(_e1);
        };

        it["doesn't remove entity when matching"] = () => {
            _collection.HandleEntity(_e1);
            _collection.HandleEntity(_e1);
            _collection.GetEntities().should_contain(_e1);
        };

        it["replaces existing entity"] = () => {
            _collection.HandleEntity(_e1);
            _collection.HandleEntity(_e1, _componentA);
            _collection.GetEntities().should_contain(_e1);
        };

        it["adds entity when replacing with an entity that hasn't been added before"] = () => {
            _collection.HandleEntity(_e1, _componentA);
            _collection.GetEntities().should_contain(_e1);
        };

        it["doesn't add entity when replacing with an entity that hasn't been added before when not matching"] = () => {
            var entity = new Entity();
            _collection.HandleEntity(entity, new ComponentB());
            _collection.GetEntities().should_be_empty();
        };

        it["gets null when single entity does not exist"] = () => {
            _collection.GetSingleEntity().should_be_null();
        };

        it["gets single entity"] = () => {
            _collection.HandleEntity(_e1);
            _collection.GetSingleEntity().should_be_same(_e1);
        };

        it["throws when attempting to get single entity and multiple matching entites exist"] = expect<SingleEntityException>(() => {
            _collection.HandleEntity(_e1);
            _collection.HandleEntity(_e2);
            _collection.GetSingleEntity();
        });

        context["events"] = () => {
            it["dispatches OnEntityAdded when matching entity added"] = () => {
                var didDispatch = 0;
                _collection.OnEntityAdded += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_e1);
                };

                _collection.HandleEntity(_e1);
                didDispatch.should_be(1);
            };

            it["doesn't dispatches OnEntityAdded when matching entity already has been added"] = () => {
                var didDispatch = 0;
                _collection.OnEntityAdded += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_e1);
                };

                _collection.HandleEntity(_e1);
                _collection.HandleEntity(_e1);
                didDispatch.should_be(1);
            };

            it["dispatches OnEntityRemove when entity got removed"] = () => {
                var didDispatch = 0;
                _collection.OnEntityRemoved += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_e1);
                };

                _collection.HandleEntity(_e1);
                _collection.RemoveEntity(_e1);
                didDispatch.should_be(1);
            };

            it["doesn't dispatch OnEntityRemove when entity didn't get removed"] = () => {
                var didDispatch = 0;
                _collection.OnEntityRemoved += (collection, entity) => {
                    didDispatch++;
                };

                _collection.RemoveEntity(_e1);
                didDispatch.should_be(0);
            };

            it["dispatches OnEntityRemoved and OnEntityAdded when entity got replaced"] = () => {
                _collection.HandleEntity(_e1);
                var didDispatchAdded = 0;
                EntityCollection eventCollectionAdded = null;
                Entity eventEntityAdded = null;
                var didDispatchRemoved = 0;
                EntityCollection eventCollectionRemoved = null;
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
                _collection.HandleEntity(_e1, _componentA);

                didDispatchAdded.should_be(1);
                eventCollectionAdded.should_be_same(_collection);
                eventEntityAdded.should_be_same(_e1);
                didDispatchRemoved.should_be(1);
                eventCollectionRemoved.should_be_same(_collection);
                eventEntityRemoved.should_be_same(_e1);
            };

            it["dispatches OnEntityAdded when entity added by replacing"] = () => {
                var didDispatch = 0;
                EntityCollection eventCollection = null;
                Entity eventEntity = null;
                _collection.OnEntityAdded += (collection, entity) => {
                    eventCollection = collection;
                    eventEntity = entity;
                    didDispatch++;
                };
                _collection.OnEntityRemoved += (collection, entity) => fail();
                _collection.HandleEntity(_e1, _componentA);

                didDispatch.should_be(1);
                eventCollection.should_be_same(_collection);
                eventEntity.should_be_same(_e1);
            };
        };

        context["internal caching"] = () => {
            it["gets cached entities"] = () => {
                _collection.HandleEntity(_e1);
                _collection.GetEntities().should_be_same(_collection.GetEntities());
            };

            it["updates cache when adding a new matching entity"] = () => {
                _collection.HandleEntity(_e1);
                var c = _collection.GetEntities();
                _collection.HandleEntity(_e2);
                c.should_not_be_same(_collection.GetEntities());
            };

            it["doesn't update cache when attempting to add a not matching entity"] = () => {
                _collection.HandleEntity(_e1);
                var c = _collection.GetEntities();
                var e = new Entity();
                _collection.HandleEntity(e);
                c.should_be_same(_collection.GetEntities());
            };
        
            it["updates cache when removing an entity"] = () => {
                _collection.HandleEntity(_e1);
                var c = _collection.GetEntities();
                _collection.RemoveEntity(_e1);
                c.should_not_be_same(_collection.GetEntities());
            };

            it["doesn't update cache when attempting to remove an entity that wasn't added before"] = () => {
                var c = _collection.GetEntities();
                _collection.RemoveEntity(_e1);
                c.should_be_same(_collection.GetEntities());
            };

            it["gets cached singleEntities"] = () => {
                _collection.HandleEntity(_e1);
                _collection.GetSingleEntity().should_be_same(_collection.GetSingleEntity());
            };

            it["updates cache when new single entity was added"] = () => {
                _collection.HandleEntity(_e1);
                var s = _collection.GetSingleEntity();
                _collection.RemoveEntity(_e1);
                _collection.HandleEntity(_e2);
                s.should_not_be_same(_collection.GetSingleEntity());
            };

            it["updates cache when single entity is removed"] = () => {
                _collection.HandleEntity(_e1);
                var s = _collection.GetSingleEntity();
                _collection.RemoveEntity(_e1);
                s.should_not_be_same(_collection.GetSingleEntity());
            };

            it["doesn't update cache when replacing an entity"] = () => {
                _collection.HandleEntity(_e1);
                var s = _collection.GetSingleEntity();
                var e = _collection.GetEntities();
                _collection.HandleEntity(_e1, _componentA);
                s.should_be_same(_collection.GetSingleEntity());
                e.should_be_same(_collection.GetEntities());
            };

            it["updates cache when replacing an entity that hasn't been added before"] = () => {
                var s = _collection.GetSingleEntity();
                var e = _collection.GetEntities();
                _collection.HandleEntity(_e1, _componentA);
                s.should_not_be_same(_collection.GetSingleEntity());
                e.should_not_be_same(_collection.GetEntities());
            };
        };

        it["doesn't replace existing entity when component is not matching"] = () => {
            _collection.HandleEntity(_e1);
            _collection.OnEntityAdded += (collection, entity) => fail();
            _collection.HandleEntity(_e1, new ComponentB());
        };

        it["removes existing entity when component is not matching but included und exceptTypes of AllOfExceptMatcher"] = () => {
            var c = new EntityCollection(EntityMatcher.AllOfExcept(ES.Types<ComponentA>(), ES.Types<ComponentB>()));
            c.HandleEntity(_e1);
            var componentB = new ComponentB();
            _e1.AddComponent(componentB);
            c.HandleEntity(_e1, componentB);
            c.GetEntities().should_not_contain(_e1);
        };

        it["empty collection, AllOf A, replacing A, then B "] = () => {
            var added = 0;
            var removed = 0;
            var c = new EntityCollection(EntityMatcher.AllOf(ES.Types<ComponentA>()));
            c.OnEntityAdded += (collection, entity) => added++;
            c.OnEntityRemoved += (collection, entity) => removed++;
            var e = new Entity();

            var componentA = new ComponentA();
            e.AddComponent(componentA);
            c.HandleEntity(e, componentA);
            c.GetEntities().should_contain(e);
            added.should_be(1);
            removed.should_be(0);

            var componentB = new ComponentB();
            e.AddComponent(componentB);
            c.HandleEntity(e, componentB);
            c.GetEntities().should_contain(e);
            added.should_be(1);
            removed.should_be(0);
        };

        it["empty collection, AllOf A except B, replacing A, then B "] = () => {
            var added = 0;
            var removed = 0;
            var c = new EntityCollection(EntityMatcher.AllOfExcept(ES.Types<ComponentA>(), ES.Types<ComponentB>()));
            c.OnEntityAdded += (collection, entity) => added++;
            c.OnEntityRemoved += (collection, entity) => removed++;
            var e = new Entity();

            var componentA = new ComponentA();
            e.AddComponent(componentA);
            c.HandleEntity(e, componentA);
            c.GetEntities().should_contain(e);
            added.should_be(1);
            removed.should_be(0);

            var componentB = new ComponentB();
            e.AddComponent(componentB);
            c.HandleEntity(e, componentB);
            c.GetEntities().should_not_contain(e);
            added.should_be(1);
            removed.should_be(1);
        };
    }
}

