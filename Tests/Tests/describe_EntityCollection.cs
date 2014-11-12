using NSpec;
using Entitas;

class describe_EntityCollection : nspec {
    EntityCollection _collection;
    Entity _eA1;
    Entity _eA2;

    void before_each() {
        _collection = new EntityCollection(Matcher.AllOf(new [] { CID.ComponentA }));
        _eA1 = this.CreateEntity();
        _eA1.AddComponentA();
        _eA2 = this.CreateEntity();
        _eA2.AddComponentA();
    }

    void when_created() {
        it["doesn't have entites which haven't been added"] = () => {
            _collection.GetEntities().should_be_empty();
        };

        it["is empty"] = () => {
            _collection.Count.should_be(0);
        };

        it["adds matching entity"] = () => {
            add(_eA1);
            _collection.GetEntities().should_contain(_eA1);
        };

        it["isn't empty"] = () => {
            add(_eA1);
            _collection.Count.should_be(1);
        };

        it["doesn't add same entity twice"] = () => {
            add(_eA1);
            add(_eA1);
            _collection.GetEntities().should_contain(_eA1);
            _collection.GetEntities().Length.should_be(1);
        };

        it["doesn't add entity when not matching"] = () => {
            var e = this.CreateEntity();
            e.AddComponentB();
            add(e);
            _collection.GetEntities().should_be_empty();
        };

        it["removes entity"] = () => {
            add(_eA1);
            removeEA1();
            _collection.GetEntities().should_be_empty();
        };

        it["gets null when single entity does not exist"] = () => {
            _collection.GetSingleEntity().should_be_null();
        };

        it["gets single entity"] = () => {
            add(_eA1);
            _collection.GetSingleEntity().should_be_same(_eA1);
        };

        it["throws when attempting to get single entity and multiple matching entites exist"] = expect<SingleEntityException>(() => {
            add(_eA1);
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

                add(_eA1);
                didDispatch.should_be(1);
            };

            it["doesn't dispatches OnEntityAdded when matching entity already has been added"] = () => {
                _collection.OnEntityAdded += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_eA1);
                };

                add(_eA1);
                add(_eA1);
                didDispatch.should_be(1);
            };

            it["dispatches OnEntityRemoved when entity got removed"] = () => {
                _collection.OnEntityRemoved += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_eA1);
                };

                add(_eA1);
                removeEA1();
                didDispatch.should_be(1);
            };

            it["doesn't dispatch OnEntityRemoved when entity didn't get removed"] = () => {
                _collection.OnEntityRemoved += (collection, entity) => this.Fail();
                removeEA1();
            };
            
            it["dispatches OnEntityWillBeRemoved when entity will be removed"] = () => {
                _collection.OnEntityWillBeRemoved += (collection, entity) => {
                    didDispatch++;
                    collection.should_be_same(_collection);
                    entity.should_be_same(_eA1);
                };

                add(_eA1);
                willRemoveEA1();
                didDispatch.should_be(1);
            };

            it["doesn't dispatch OnEntityWillBeRemoved when collection doesn't contain entity"] = () => {
                _collection.OnEntityWillBeRemoved += (collection, entity) => this.Fail();
                add(_eA1);
                _collection.WillRemoveEntity(new Entity(0));
            };

            it["dispatches OnEntityRemoved and OnEntityAdded when updating"] = () => {
                add(_eA1);
                var removed = 0;
                var added = 0;
                _collection.OnEntityRemoved += (collection, entity) => removed++;
                _collection.OnEntityWillBeRemoved += (collection, entity) => this.Fail();
                _collection.OnEntityAdded += (collection, entity) => added++;

                _collection.UpdateEntity(_eA1);

                removed.should_be(1);
                added.should_be(1);
            };

            it["doesn't dispatch OnEntityRemoved and OnEntityAdded when updating when collection doesn't contain entity"] = () => {
                _collection.OnEntityRemoved += (collection, entity) => this.Fail();
                _collection.OnEntityWillBeRemoved += (collection, entity) => this.Fail();
                _collection.OnEntityAdded += (collection, entity) => this.Fail();
                _collection.UpdateEntity(_eA1);
            };
        };

        context["internal caching"] = () => {
            it["gets cached entities"] = () => {
                add(_eA1);
                _collection.GetEntities().should_be_same(_collection.GetEntities());
            };

            it["updates cache when adding a new matching entity"] = () => {
                add(_eA1);
                var c = _collection.GetEntities();
                add(_eA2);
                c.should_not_be_same(_collection.GetEntities());
            };

            it["doesn't update cache when attempting to add a not matching entity"] = () => {
                add(_eA1);
                var c = _collection.GetEntities();
                var e = this.CreateEntity();
                add(e);
                c.should_be_same(_collection.GetEntities());
            };

            it["updates cache when removing an entity"] = () => {
                add(_eA1);
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
                add(_eA1);
                _collection.GetSingleEntity().should_be_same(_collection.GetSingleEntity());
            };

            it["updates cache when new single entity was added"] = () => {
                add(_eA1);
                var s = _collection.GetSingleEntity();
                removeEA1();
                add(_eA2);
                s.should_not_be_same(_collection.GetSingleEntity());
            };

            it["updates cache when single entity is removed"] = () => {
                add(_eA1);
                var s = _collection.GetSingleEntity();
                removeEA1();
                s.should_not_be_same(_collection.GetSingleEntity());
            };
        };
    }

    void add(Entity entity) {
        _collection.AddEntityIfMatching(entity);
    }

    void willRemoveEA1() {
        _collection.WillRemoveEntity(_eA1);
    }

    void removeEA1() {
        _collection.RemoveEntity(_eA1);
    }
}

