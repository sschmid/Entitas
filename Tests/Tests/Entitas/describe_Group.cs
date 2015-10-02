using NSpec;
using Entitas;

class describe_Group : nspec {
    Group _groupA;

    void assertContains(params Entity[] expectedEntities) {
        _groupA.count.should_be(expectedEntities.Length);
        var entities = _groupA.GetEntities();
        entities.Length.should_be(expectedEntities.Length);
        foreach (var e in expectedEntities) {
            entities.should_contain(e);
            _groupA.ContainsEntity(e).should_be_true();
        }
    }

    void assertContainsNot(Entity entity) {
        _groupA.count.should_be(0);
        _groupA.GetEntities().should_be_empty();
       _groupA.ContainsEntity(entity).should_be_false();
    }

    void when_created() {

        Entity eA1 = null;
        Entity eA2 = null;

        before = () => {
            _groupA = new Group(Matcher.AllOf(new [] { CID.ComponentA }));
            eA1 = this.CreateEntity();
            eA1.AddComponentA();
            eA2 = this.CreateEntity();
            eA2.AddComponentA();
        };

        context["initial state"] = () => {
            it["doesn't have entities which haven't been added"] = () => {
                _groupA.GetEntities().should_be_empty();
            };

            it["is empty"] = () => {
                _groupA.count.should_be(0);
            };

            it["doesn't contain entity"] = () => {
                _groupA.ContainsEntity(eA1).should_be_false();
            };
        };

        context["when entity is matching"] = () => {
            before = () => {
                handleSilently(eA1);
            };

            it["adds matching entity"] = () => {
                assertContains(eA1);
            };

            it["doesn't add same entity twice"] = () => {
                handleSilently(eA1);
                assertContains(eA1);
            };

            context["when entity doesn't match anymore"] = () => {
                before = () => {
                    eA1.RemoveComponentA();
                    handleSilently(eA1);
                };

                it["removes entity"] = () => {
                    assertContainsNot(eA1);
                };
            };
        };

        it["doesn't add entity when not matching"] = () => {
            var e = this.CreateEntity();
            e.AddComponentB();
            handleSilently(e);
            assertContainsNot(e);
        };

        it["gets null when single entity does not exist"] = () => {
            _groupA.GetSingleEntity().should_be_null();
        };

        it["gets single entity"] = () => {
            handleSilently(eA1);
            _groupA.GetSingleEntity().should_be_same(eA1);
        };

        it["throws when attempting to get single entity and multiple matching entities exist"] = expect<SingleEntityException>(() => {
            handleSilently(eA1);
            handleSilently(eA2);
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
                    entity.should_be_same(eA1);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(Component.A);
                };
                _groupA.OnEntityRemoved += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => this.Fail();

                handleAddEA(eA1);
                didDispatch.should_be(1);
            };

            it["doesn't dispatches OnEntityAdded when matching entity already has been added"] = () => {
                handleAddEA(eA1);
                _groupA.OnEntityAdded += (group, entity, index, component) => didDispatch++;
                _groupA.OnEntityRemoved += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => this.Fail();
                handleAddEA(eA1);
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
                handleSilently(eA1);
                _groupA.OnEntityRemoved += (group, entity, index, component) => {
                    didDispatch++;
                    group.should_be_same(_groupA);
                    entity.should_be_same(eA1);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(Component.A);
                };
                _groupA.OnEntityAdded += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => this.Fail();

                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);

                didDispatch.should_be(1);
            };

            it["doesn't dispatch OnEntityRemoved when entity didn't get removed"] = () => {
                _groupA.OnEntityRemoved += (group, entity, index, component) => this.Fail();
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);
            };

            it["dispatches OnEntityRemoved, OnEntityAdded and OnEntityUpdated when updating"] = () => {
                handleSilently(eA1);
                var removed = 0;
                var added = 0;
                var updated = 0;
                var newComponentA = new ComponentA();
                _groupA.OnEntityRemoved += (group, entity, index, component) => {
                    removed += 1;
                    group.should_be(_groupA);
                    entity.should_be(eA1);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(Component.A);
                };
                _groupA.OnEntityAdded += (group, entity, index, component) => {
                    added += 1;
                    group.should_be(_groupA);
                    entity.should_be(eA1);
                    index.should_be(CID.ComponentA);
                    component.should_be_same(newComponentA);
                };
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => {
                    updated += 1;
                    group.should_be(_groupA);
                    entity.should_be(eA1);
                    index.should_be(CID.ComponentA);
                    previousComponent.should_be_same(Component.A);
                    newComponent.should_be_same(newComponentA);
                };

                updateEA(eA1, newComponentA);

                removed.should_be(1);
                added.should_be(1);
                updated.should_be(1);
            };

            it["doesn't dispatch OnEntityRemoved and OnEntityAdded when updating when group doesn't contain entity"] = () => {
                _groupA.OnEntityRemoved += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityAdded += (group, entity, index, component) => this.Fail();
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => this.Fail();
                updateEA(eA1, new ComponentA());
            };
        };

        context["internal caching"] = () => {
            context["GetEntities()"] = () => {
                Entity[] cache = null;
                before = () => {
                    handleSilently(eA1);
                    cache = _groupA.GetEntities();
                };

                it["gets cached entities"] = () => {
                    _groupA.GetEntities().should_be_same(cache);
                };

                it["updates cache when adding a new matching entity"] = () => {
                    handleSilently(eA2);
                    _groupA.GetEntities().should_not_be_same(cache);
                };

                it["doesn't update cache when attempting to add a not matching entity"] = () => {
                    var e = this.CreateEntity();
                    handleSilently(e);
                    _groupA.GetEntities().should_be_same(cache);
                };

                it["updates cache when removing an entity"] = () => {
                    eA1.RemoveComponentA();
                    handleSilently(eA1);
                    _groupA.GetEntities().should_not_be_same(cache);
                };

                it["doesn't update cache when attempting to remove an entity that wasn't added before"] = () => {
                    eA2.RemoveComponentA();
                    handleSilently(eA2);
                    _groupA.GetEntities().should_be_same(cache);
                };
            };

            context["SingleEntity()"] = () => {
                Entity cache = null;
                before = () => {
                    handleSilently(eA1);
                    cache = _groupA.GetSingleEntity();
                };

                it["gets cached singleEntities"] = () => {
                    _groupA.GetSingleEntity().should_be_same(cache);
                };

                it["updates cache when new single entity was added"] = () => {
                    eA1.RemoveComponentA();
                    handleSilently(eA1);
                    handleSilently(eA2);
                    _groupA.GetSingleEntity().should_not_be_same(cache);
                };

                it["updates cache when single entity is removed"] = () => {
                    eA1.RemoveComponentA();
                    handleSilently(eA1);
                    _groupA.GetSingleEntity().should_not_be_same(cache);
                };
            };
        };

        context["reference counting"] = () => {

            it["retains matched entity"] = () => {
                eA1.RefCount().should_be(0);
                handleSilently(eA1);
                eA1.RefCount().should_be(1);
            };

            it["releases removed entity"] = () => {
                handleSilently(eA1);
                eA1.RemoveComponentA();
                handleSilently(eA1);
                eA1.RefCount().should_be(0);
            };

            it["invalidates entitiesCache (silent mode)"] = () => {
                eA1.OnEntityReleased += entity => {
                    _groupA.GetEntities().Length.should_be(0);
                };
                handleSilently(eA1);
                _groupA.GetEntities();
                eA1.RemoveComponentA();
                handleSilently(eA1);
            };

            it["invalidates entitiesCache"] = () => {
                eA1.OnEntityReleased += entity => {
                    _groupA.GetEntities().Length.should_be(0);
                };
                handleAddEA(eA1);
                _groupA.GetEntities();
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);
            };

            it["invalidates singleEntityCache (silent mode)"] = () => {
                eA1.OnEntityReleased += entity => {
                    _groupA.GetSingleEntity().should_be_null();
                };
                handleSilently(eA1);
                _groupA.GetSingleEntity();
                eA1.RemoveComponentA();
                handleSilently(eA1);
            };

            it["invalidates singleEntityCache"] = () => {
                eA1.OnEntityReleased += entity => {
                    _groupA.GetSingleEntity().should_be_null();
                };
                handleAddEA(eA1);
                _groupA.GetSingleEntity();
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);
            };

            it["retains entity until removed"] = () => {
                handleAddEA(eA1);
                var didDispatch = 0;
                _groupA.OnEntityRemoved += (group, entity, index, component) => {
                    didDispatch += 1;
                    entity.RefCount().should_be(1);
                };
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);

                didDispatch.should_be(1);
                eA1.RefCount().should_be(0);
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

