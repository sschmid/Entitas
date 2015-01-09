using NSpec;
using Entitas;

class describe_Group : nspec {
    Group _group;
    Entity _eA1;
    Entity _eA2;

    void before_each() {
        _group = new Group(Matcher.AllOf(new [] { CID.ComponentA }));
        _eA1 = this.CreateEntity();
        _eA1.AddComponentA();
        _eA2 = this.CreateEntity();
        _eA2.AddComponentA();
    }

    void when_created() {
        it["doesn't have entites which haven't been added"] = () => {
            _group.GetEntities().should_be_empty();
        };

        it["is empty"] = () => {
            _group.Count.should_be(0);
        };

        it["adds matching entity"] = () => {
            add(_eA1);
            _group.GetEntities().should_contain(_eA1);
        };

        it["isn't empty"] = () => {
            add(_eA1);
            _group.Count.should_be(1);
        };

        it["doesn't add same entity twice"] = () => {
            add(_eA1);
            add(_eA1);
            _group.GetEntities().should_contain(_eA1);
            _group.GetEntities().Length.should_be(1);
        };

        it["doesn't add entity when not matching"] = () => {
            var e = this.CreateEntity();
            e.AddComponentB();
            add(e);
            _group.GetEntities().should_be_empty();
        };

        it["removes entity"] = () => {
            add(_eA1);
            removeEA1();
            _group.GetEntities().should_be_empty();
        };

        it["gets null when single entity does not exist"] = () => {
            _group.GetSingleEntity().should_be_null();
        };

        it["gets single entity"] = () => {
            add(_eA1);
            _group.GetSingleEntity().should_be_same(_eA1);
        };

        it["throws when attempting to get single entity and multiple matching entites exist"] = expect<SingleEntityException>(() => {
            add(_eA1);
            add(_eA2);
            _group.GetSingleEntity();
        });

        context["events"] = () => {
            var didDispatch = 0;

            before = () => {
                didDispatch = 0;
            };

            it["dispatches OnEntityAdded when matching entity added"] = () => {
                _group.OnEntityAdded += (group, entity) => {
                    didDispatch++;
                    group.should_be_same(_group);
                    entity.should_be_same(_eA1);
                };

                add(_eA1);
                didDispatch.should_be(1);
            };

            it["doesn't dispatches OnEntityAdded when matching entity already has been added"] = () => {
                _group.OnEntityAdded += (group, entity) => {
                    didDispatch++;
                    group.should_be_same(_group);
                    entity.should_be_same(_eA1);
                };

                add(_eA1);
                add(_eA1);
                didDispatch.should_be(1);
            };

            it["dispatches OnEntityRemoved when entity got removed"] = () => {
                _group.OnEntityRemoved += (group, entity) => {
                    didDispatch++;
                    group.should_be_same(_group);
                    entity.should_be_same(_eA1);
                };

                add(_eA1);
                removeEA1();
                didDispatch.should_be(1);
            };

            it["doesn't dispatch OnEntityRemoved when entity didn't get removed"] = () => {
                _group.OnEntityRemoved += (group, entity) => this.Fail();
                removeEA1();
            };
            
            it["dispatches OnEntityWillBeRemoved when entity will be removed"] = () => {
                _group.OnEntityWillBeRemoved += (group, entity) => {
                    didDispatch++;
                    group.should_be_same(_group);
                    entity.should_be_same(_eA1);
                };

                add(_eA1);
                willRemoveEA1();
                didDispatch.should_be(1);
            };

            it["doesn't dispatch OnEntityWillBeRemoved when group doesn't contain entity"] = () => {
                _group.OnEntityWillBeRemoved += (group, entity) => this.Fail();
                add(_eA1);
                _group.WillRemoveEntity(new Entity(0));
            };

            it["dispatches OnEntityRemoved and OnEntityAdded when updating"] = () => {
                add(_eA1);
                var removed = 0;
                var added = 0;
                _group.OnEntityRemoved += (group, entity) => removed++;
                _group.OnEntityWillBeRemoved += (group, entity) => this.Fail();
                _group.OnEntityAdded += (group, entity) => added++;

                _group.UpdateEntity(_eA1);

                removed.should_be(1);
                added.should_be(1);
            };

            it["doesn't dispatch OnEntityRemoved and OnEntityAdded when updating when group doesn't contain entity"] = () => {
                _group.OnEntityRemoved += (group, entity) => this.Fail();
                _group.OnEntityWillBeRemoved += (group, entity) => this.Fail();
                _group.OnEntityAdded += (group, entity) => this.Fail();
                _group.UpdateEntity(_eA1);
            };
        };

        context["internal caching"] = () => {
            it["gets cached entities"] = () => {
                add(_eA1);
                _group.GetEntities().should_be_same(_group.GetEntities());
            };

            it["updates cache when adding a new matching entity"] = () => {
                add(_eA1);
                var g = _group.GetEntities();
                add(_eA2);
                g.should_not_be_same(_group.GetEntities());
            };

            it["doesn't update cache when attempting to add a not matching entity"] = () => {
                add(_eA1);
                var g = _group.GetEntities();
                var e = this.CreateEntity();
                add(e);
                g.should_be_same(_group.GetEntities());
            };

            it["updates cache when removing an entity"] = () => {
                add(_eA1);
                var g = _group.GetEntities();
                removeEA1();
                g.should_not_be_same(_group.GetEntities());
            };

            it["doesn't update cache when attempting to remove an entity that wasn't added before"] = () => {
                var g = _group.GetEntities();
                removeEA1();
                g.should_be_same(_group.GetEntities());
            };

            it["gets cached singleEntities"] = () => {
                add(_eA1);
                _group.GetSingleEntity().should_be_same(_group.GetSingleEntity());
            };

            it["updates cache when new single entity was added"] = () => {
                add(_eA1);
                var s = _group.GetSingleEntity();
                removeEA1();
                add(_eA2);
                s.should_not_be_same(_group.GetSingleEntity());
            };

            it["updates cache when single entity is removed"] = () => {
                add(_eA1);
                var s = _group.GetSingleEntity();
                removeEA1();
                s.should_not_be_same(_group.GetSingleEntity());
            };
        };
    }

    void add(Entity entity) {
        _group.AddEntityIfMatching(entity);
    }

    void willRemoveEA1() {
        _group.WillRemoveEntity(_eA1);
    }

    void removeEA1() {
        _group.RemoveEntity(_eA1);
    }
}

