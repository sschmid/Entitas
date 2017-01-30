using Entitas;
using NSpec;

class describe_Group : nspec {

    IGroup<TestEntity> _groupA;

    void assertContains(params TestEntity[] expectedEntities) {
        _groupA.count.should_be(expectedEntities.Length);

        var entities = _groupA.GetEntities();
        entities.Length.should_be(expectedEntities.Length);

        foreach(var e in expectedEntities) {
            entities.should_contain(e);
            _groupA.ContainsEntity(e).should_be_true();
        }
    }

    void assertContainsNot(TestEntity entity) {
        _groupA.count.should_be(0);
        _groupA.GetEntities().should_be_empty();
        _groupA.ContainsEntity(entity).should_be_false();
    }

    void when_created() {

        TestEntity eA1 = null;
        TestEntity eA2 = null;

        before = () => {
            _groupA = new Group<TestEntity>(Matcher<TestEntity>.AllOf(CID.ComponentA));
            eA1 = this.CreateEntity().AddComponentA();
            eA2 = this.CreateEntity().AddComponentA();
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

                it["removes entity"] = () => {
                    eA1.RemoveComponentA();
                    handleSilently(eA1);

                    assertContainsNot(eA1);
                };
            };
        };

        context["when entity is not enabled"] = () => {

            it["doesn't add entity"] = () => {
                eA1.destroy();
                handleSilently(eA1);
                assertContainsNot(eA1);
            };
        };

        it["doesn't add entity when not matching"] = () => {
            var e = this.CreateEntity().AddComponentB();
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

        it["throws when attempting to get single entity and multiple matching entities exist"] = expect<GroupSingleEntityException<TestEntity>>(() => {
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
                _groupA.OnEntityRemoved += delegate { this.Fail(); };
                _groupA.OnEntityUpdated += delegate { this.Fail(); };

                handleAddEA(eA1);
                didDispatch.should_be(1);
            };

            it["doesn't dispatches OnEntityAdded when matching entity already has been added"] = () => {
                handleAddEA(eA1);
                _groupA.OnEntityAdded += delegate { this.Fail(); };
                _groupA.OnEntityRemoved += delegate { this.Fail(); };
                _groupA.OnEntityUpdated += delegate { this.Fail(); };
                handleAddEA(eA1);
                didDispatch.should_be(0);
            };

            it["doesn't dispatches OnEntityAdded when entity is not matching"] = () => {
                var e = this.CreateEntity().AddComponentB();
                _groupA.OnEntityAdded += delegate { this.Fail(); };
                _groupA.OnEntityRemoved += delegate { this.Fail(); };
                _groupA.OnEntityUpdated += delegate { this.Fail(); };
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
                _groupA.OnEntityAdded += delegate { this.Fail(); };
                _groupA.OnEntityUpdated += delegate { this.Fail(); };

                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);

                didDispatch.should_be(1);
            };

            it["doesn't dispatch OnEntityRemoved when entity didn't get removed"] = () => {
                _groupA.OnEntityRemoved += delegate { this.Fail(); };
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
                _groupA.OnEntityRemoved += delegate { this.Fail(); };
                _groupA.OnEntityAdded += delegate { this.Fail(); };
                _groupA.OnEntityUpdated += delegate { this.Fail(); };
                updateEA(eA1, new ComponentA());
            };

            it["removes all event handlers"] = () => {
                _groupA.OnEntityAdded += delegate { this.Fail(); };
                _groupA.OnEntityRemoved += delegate { this.Fail(); };
                _groupA.OnEntityUpdated += delegate { this.Fail(); };

                _groupA.RemoveAllEventHandlers();

                handleAddEA(eA1);

                var cA = eA1.GetComponentA();
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, cA);

                eA1.AddComponentA();
                handleAddEA(eA1);
                updateEA(eA1, Component.A);
            };
        };

        context["internal caching"] = () => {

            context["GetEntities()"] = () => {

                IEntity[] cache = null;

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

                it["doesn't update cache when updating an entity"] = () => {
                    updateEA(eA1, new ComponentA());
                    _groupA.GetEntities().should_be_same(cache);
                };
            };

            context["SingleEntity()"] = () => {

                IEntity cache = null;

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

                it["doesn't update cache when single entity is updated"] = () => {
                    updateEA(eA1, new ComponentA());
                    _groupA.GetSingleEntity().should_be_same(cache);
                };
            };
        };

        context["reference counting"] = () => {

            it["retains matched entity"] = () => {
                eA1.retainCount.should_be(0);
                handleSilently(eA1);
                eA1.retainCount.should_be(1);
            };

            it["releases removed entity"] = () => {
                handleSilently(eA1);
                eA1.RemoveComponentA();
                handleSilently(eA1);
                eA1.retainCount.should_be(0);
            };

            it["invalidates entitiesCache (silent mode)"] = () => {
                var didExecute = 0;
                eA1.OnEntityReleased += entity => {
                    didExecute += 1;
                    _groupA.GetEntities().Length.should_be(0);
                };
                handleSilently(eA1);
                _groupA.GetEntities();
                eA1.RemoveComponentA();
                handleSilently(eA1);
                didExecute.should_be(1);
            };

            it["invalidates entitiesCache"] = () => {
                var didExecute = 0;
                eA1.OnEntityReleased += entity => {
                    didExecute += 1;
                    _groupA.GetEntities().Length.should_be(0);
                };
                handleAddEA(eA1);
                _groupA.GetEntities();
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);
                didExecute.should_be(1);
            };

            it["invalidates singleEntityCache (silent mode)"] = () => {
                var didExecute = 0;
                eA1.OnEntityReleased += entity => {
                    didExecute += 1;
                    _groupA.GetSingleEntity().should_be_null();
                };
                handleSilently(eA1);
                _groupA.GetSingleEntity();
                eA1.RemoveComponentA();
                handleSilently(eA1);
                didExecute.should_be(1);
            };

            it["invalidates singleEntityCache"] = () => {
                var didExecute = 0;
                eA1.OnEntityReleased += entity => {
                    didExecute += 1;
                    _groupA.GetSingleEntity().should_be_null();
                };
                handleAddEA(eA1);
                _groupA.GetSingleEntity();
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);
                didExecute.should_be(1);
            };

            it["retains entity until after event handlers were called"] = () => {
                handleAddEA(eA1);
                var didDispatch = 0;
                _groupA.OnEntityRemoved += (group, entity, index, component) => {
                    didDispatch += 1;
                    entity.retainCount.should_be(1);
                };
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);

                didDispatch.should_be(1);
                eA1.retainCount.should_be(0);
            };
        };

        it["can ToString"] = () => {
            var m = Matcher<TestEntity>.AllOf(Matcher<TestEntity>.AllOf(0), Matcher<TestEntity>.AllOf(1));
            var group = new Group<TestEntity>(m);
            group.ToString().should_be("Group(AllOf(0, 1))");
        };
    }

    void handleSilently(TestEntity entity) {
        _groupA.HandleEntitySilently(entity);
    }

    void handle(TestEntity entity, int index, IComponent component) {
        _groupA.HandleEntity(entity, index, component);
    }

    void handleAddEA(TestEntity entity) {
        handle(entity, CID.ComponentA, entity.GetComponentA());
    }

    void handleAddEB(TestEntity entity) {
        handle(entity, CID.ComponentB, entity.GetComponentB());
    }

    void handleRemoveEA(TestEntity entity, IComponent component) {
        handle(entity, CID.ComponentA, component);
    }

    void updateEA(TestEntity entity, IComponent component) {
        _groupA.UpdateEntity(entity, CID.ComponentA, Component.A, component);
    }
}
