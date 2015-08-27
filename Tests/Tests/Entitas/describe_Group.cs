using NSpec;
using Entitas;

class describe_Group : nspec {
    Group _groupA;
    Entity _eA1;
    Entity _eA2;

    void before_each() {
        _groupA = new Group(Matcher.AllOf(new [] { CID.ComponentA }));
        _eA1 = this.CreateEntity();
        _eA1.AddComponentA();
        _eA2 = this.CreateEntity();
        _eA2.AddComponentA();
    }

    void when_created() {

        it["doesn't have entities which haven't been added"] = () => {
            _groupA.GetEntities().should_be_empty();
        };

        it["is empty"] = () => {
            _groupA.Count.should_be(0);
        };

        it["doesn't contain entity"] = () => {
            _groupA.ContainsEntity(_eA1).should_be_false();
        };

        context["when entity is matching"] = () => {
            before = () => {
                handleSilently(_eA1);
            };

            it["adds matching entity"] = () => {
                var entities = _groupA.GetEntities();
                entities.Length.should_be(1);
                entities.should_contain(_eA1);
            };
            it["isn't empty"] = () => _groupA.Count.should_be(1);
            it["contains entity"] = () => _groupA.ContainsEntity(_eA1).should_be_true();
            it["doesn't add same entity twice"] = () => {
                handleSilently(_eA1);
                var entities = _groupA.GetEntities();
                entities.Length.should_be(1);
                entities.should_contain(_eA1);
            };

            context["when entity doesn't match anymore"] = () => {
                before = () => {
                    _eA1.RemoveComponentA();
                    handleSilently(_eA1);
                };

                it["removes entity"] = () => _groupA.GetEntities().should_be_empty();
                it["is empty"] = () => _groupA.Count.should_be(0);
                it["doesn't contains entity"] = () => _groupA.ContainsEntity(_eA1).should_be_false();
            };
        };

        it["doesn't add entity when not matching"] = () => {
            var e = this.CreateEntity();
            e.AddComponentB();
            handleSilently(e);
            _groupA.GetEntities().should_be_empty();
            _groupA.Count.should_be(0);
            _groupA.ContainsEntity(e).should_be_false();
        };

        it["gets null when single entity does not exist"] = () => {
            _groupA.GetSingleEntity().should_be_null();
        };

        it["gets single entity"] = () => {
            handleSilently(_eA1);
            _groupA.GetSingleEntity().should_be_same(_eA1);
        };

        it["throws when attempting to get single entity and multiple matching entities exist"] = expect<SingleEntityException>(() => {
            handleSilently(_eA1);
            handleSilently(_eA2);
            _groupA.GetSingleEntity();
        });

        context["events"] = () => {
            var didDispatch = 0;

            before = () => {
                didDispatch = 0;
            };

            it["dispatches OnEntityAdded when matching entity added"] = () => {
                _groupA.OnEntityAdded += (group, entity, index, component) => {
                    didDispatch++;
                    group.should_be_same(_groupA);
                    entity.should_be_same(_eA1);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(Component.A);
                };
                _groupA.OnEntityRemoved += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => this.Fail();

                handleAddEA(_eA1);
                didDispatch.should_be(1);
            };

            it["doesn't dispatches OnEntityAdded when matching entity already has been added"] = () => {
                handleAddEA(_eA1);
                _groupA.OnEntityAdded += (group, entity, index, component) => didDispatch++;
                _groupA.OnEntityRemoved += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => this.Fail();
                handleAddEA(_eA1);
                didDispatch.should_be(0);
            };

            it["doesn't dispatches OnEntityAdded when entity is not matching"] = () => {
                var e = this.CreateEntity();
                e.AddComponentB();
                _groupA.OnEntityAdded += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityRemoved += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => this.Fail();
                handleAddEB(e);
            };

            it["dispatches OnEntityRemoved when entity got removed"] = () => {
                handleSilently(_eA1);
                _groupA.OnEntityRemoved += (group, entity, index, component) => {
                    didDispatch++;
                    group.should_be_same(_groupA);
                    entity.should_be_same(_eA1);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(Component.A);
                };
                _groupA.OnEntityAdded += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => this.Fail();

                _eA1.RemoveComponentA();
                handleRemoveEA(_eA1, Component.A);

                didDispatch.should_be(1);
            };

            it["doesn't dispatch OnEntityRemoved when entity didn't get removed"] = () => {
                _groupA.OnEntityRemoved += (group, entity, index, component) => this.Fail();
                _eA1.RemoveComponentA();
                handleRemoveEA(_eA1, Component.A);
            };

            it["dispatches OnEntityRemoved, OnEntityAdded and OnEntityUpdated when updating"] = () => {
                handleSilently(_eA1);
                var removed = 0;
                var added = 0;
                var updated = 0;
                var newComponentA = new ComponentA();
                _groupA.OnEntityRemoved += (group, entity, index, component) => {
                    removed += 1;
                    group.should_be(_groupA);
                    entity.should_be(_eA1);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(Component.A);
                };
                _groupA.OnEntityAdded += (group, entity, index, component) => {
                    added += 1;
                    group.should_be(_groupA);
                    entity.should_be(_eA1);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(newComponentA);
                };
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => {
                    updated += 1;
                    group.should_be(_groupA);
                    entity.should_be(_eA1);
                    index.should_be(CID.ComponentA);
                    previousComponent.should_be_same(Component.A);
                    newComponent.should_be_same(newComponentA);
                };

                updateEA(_eA1, newComponentA);

                removed.should_be(1);
                added.should_be(1);
                updated.should_be(1);
            };

            it["doesn't dispatch OnEntityRemoved and OnEntityAdded when updating when group doesn't contain entity"] = () => {
                _groupA.OnEntityRemoved += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityAdded += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => this.Fail();
                updateEA(_eA1, new ComponentA());
            };
        };

        context["internal caching"] = () => {
            context["GetEntities()"] = () => {
                Entity[] cache = null;
                before = () => {
                    handleSilently(_eA1);
                    cache = _groupA.GetEntities();
                };

                it["gets cached entities"] = () => {
                    _groupA.GetEntities().should_be_same(cache);
                };

                it["updates cache when adding a new matching entity"] = () => {
                    handleSilently(_eA2);
                    _groupA.GetEntities().should_not_be_same(cache);
                };

                it["doesn't update cache when attempting to add a not matching entity"] = () => {
                    var e = this.CreateEntity();
                    handleSilently(e);
                    _groupA.GetEntities().should_be_same(cache);
                };

                it["updates cache when removing an entity"] = () => {
                    _eA1.RemoveComponentA();
                    handleSilently(_eA1);
                    _groupA.GetEntities().should_not_be_same(cache);
                };

                it["doesn't update cache when attempting to remove an entity that wasn't added before"] = () => {
                    _eA2.RemoveComponentA();
                    handleSilently(_eA2);
                    _groupA.GetEntities().should_be_same(cache);
                };
            };

            context["SingleEntity()"] = () => {
                Entity cache = null;
                before = () => {
                    handleSilently(_eA1);
                    cache = _groupA.GetSingleEntity();
                };

                it["gets cached singleEntities"] = () => {
                    _groupA.GetSingleEntity().should_be_same(cache);
                };

                it["updates cache when new single entity was added"] = () => {
                    _eA1.RemoveComponentA();
                    handleSilently(_eA1);
                    handleSilently(_eA2);
                    _groupA.GetSingleEntity().should_not_be_same(cache);
                };

                it["updates cache when single entity is removed"] = () => {
                    _eA1.RemoveComponentA();
                    handleSilently(_eA1);
                    _groupA.GetSingleEntity().should_not_be_same(cache);
                };
            };
        };

        context["reference counting"] = () => {

            it["retains matched entity"] = () => {
                _eA1.GetRefCount().should_be(0);
                handleSilently(_eA1);
                _eA1.GetRefCount().should_be(1);
            };

            it["releases removed entity"] = () => {
                handleSilently(_eA1);
                _eA1.RemoveComponentA();
                handleSilently(_eA1);
                _eA1.GetRefCount().should_be(0);
            };

            it["invalidates entitiesCache (silent mode)"] = () => {
                _eA1.OnEntityReleased += entity => {
                    _groupA.GetEntities().Length.should_be(0);
                };
                handleSilently(_eA1);
                _groupA.GetEntities();
                _eA1.RemoveComponentA();
                handleSilently(_eA1);
            };

            it["invalidates entitiesCache"] = () => {
                _eA1.OnEntityReleased += entity => {
                    _groupA.GetEntities().Length.should_be(0);
                };
                handleAddEA(_eA1);
                _groupA.GetEntities();
                _eA1.RemoveComponentA();
                handleRemoveEA(_eA1, Component.A);
            };

            it["invalidates singleEntityCache (silent mode)"] = () => {
                _eA1.OnEntityReleased += entity => {
                    _groupA.GetSingleEntity().should_be_null();
                };
                handleSilently(_eA1);
                _groupA.GetSingleEntity();
                _eA1.RemoveComponentA();
                handleSilently(_eA1);
            };

            it["invalidates singleEntityCache"] = () => {
                _eA1.OnEntityReleased += entity => {
                    _groupA.GetSingleEntity().should_be_null();
                };
                handleAddEA(_eA1);
                _groupA.GetSingleEntity();
                _eA1.RemoveComponentA();
                handleRemoveEA(_eA1, Component.A);
            };

            it["retains entity until removed"] = () => {
                handleAddEA(_eA1);
                var didDispatch = 0;
                _groupA.OnEntityRemoved += (group, entity, index, component) => {
                    didDispatch += 1;
                    entity.GetRefCount().should_be(1);
                };
                _eA1.RemoveComponentA();
                handleRemoveEA(_eA1, Component.A);

                didDispatch.should_be(1);
                _eA1.GetRefCount().should_be(0);
            };
        };

        it["can ToString"] = () => {
            var m = Matcher.AllOf(Matcher.AllOf(0), Matcher.AllOf(1));
            var group = new Group(m);
            group.ToString().should_be("Group(AllOf(0, 1))");
        };
    }

    void handleSilently(Entity entity) {
        _groupA.HandleEntitySilently(entity);
    }

    void handle(Entity entity, int index, IComponent component) {
        _groupA.HandleEntity(entity, index, component);
    }

    void handleAddEA(Entity entity) {
        handle(entity, CID.ComponentA, entity.GetComponent(CID.ComponentA));
    }

    void handleAddEB(Entity entity) {
        handle(entity, CID.ComponentB, entity.GetComponent(CID.ComponentB));
    }

    void handleRemoveEA(Entity entity, IComponent component) {
        handle(entity, CID.ComponentA, component);
    }

    void updateEA(Entity entity, IComponent component) {
        _groupA.UpdateEntity(entity, CID.ComponentA, Component.A, component);
    }
}

