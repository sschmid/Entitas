using System.Collections.Generic;
using System.Linq;
using Entitas;
using NSpec;
using Shouldly;

class describe_Group : nspec {

    IGroup<TestEntity> _groupA;

    void assertContains(params TestEntity[] expectedEntities) {
        _groupA.count.ShouldBe(expectedEntities.Length);

        var entities = _groupA.GetEntities();
        entities.Length.ShouldBe(expectedEntities.Length);

        foreach (var e in expectedEntities) {
            entities.ShouldContain(e);
            _groupA.ContainsEntity(e).ShouldBeTrue();
        }
    }

    void assertContainsNot(TestEntity entity) {
        _groupA.count.ShouldBe(0);
        _groupA.GetEntities().ShouldBeEmpty();
        _groupA.ContainsEntity(entity).ShouldBeFalse();
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
                _groupA.GetEntities().ShouldBeEmpty();
            };

            it["doesn't add entities to buffer"] = () => {
                var buffer = new List<TestEntity>();
                buffer.Add(this.CreateEntity());
                var retBuffer = _groupA.GetEntities(buffer);
                buffer.ShouldBeEmpty();
                retBuffer.ShouldBeSameAs(buffer);
            };

            it["is empty"] = () => {
                _groupA.count.ShouldBe(0);
            };

            it["doesn't contain entity"] = () => {
                _groupA.ContainsEntity(eA1).ShouldBeFalse();
            };
        };

        context["when entity is matching"] = () => {

            before = () => {
                handleSilently(eA1);
            };

            it["adds matching entity"] = () => {
                assertContains(eA1);
            };

            it["fills buffer with entities"] = () => {
                var buffer = new List<TestEntity>();
                _groupA.GetEntities(buffer);
                buffer.Count.ShouldBe(1);
                buffer[0].ShouldBeSameAs(eA1);
            };

            it["clears buffer before filling"] = () => {
                var buffer = new List<TestEntity>();
                buffer.Add(this.CreateEntity());
                buffer.Add(this.CreateEntity());
                _groupA.GetEntities(buffer);
                buffer.Count.ShouldBe(1);
                buffer[0].ShouldBeSameAs(eA1);
            };

            it["doesn't add same entity twice"] = () => {
                handleSilently(eA1);
                assertContains(eA1);
            };

            it["enumerates group"] = () => {
                var i = 0;
                IEntity e = null;
                foreach (var entity in _groupA) {
                    i++;
                    e = entity;
                }

                i.ShouldBe(1);
                e.ShouldBeSameAs(eA1);
            };

            it["returns enumerable"] = () => {
                _groupA.AsEnumerable().Single().ShouldBeSameAs(eA1);
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
                eA1.InternalDestroy();
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
            _groupA.GetSingleEntity().ShouldBeNull();
        };

        it["gets single entity"] = () => {
            handleSilently(eA1);
            _groupA.GetSingleEntity().ShouldBeSameAs(eA1);
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
                    group.ShouldBeSameAs(_groupA);
                    entity.ShouldBeSameAs(eA1);
                    index.ShouldBe(CID.ComponentA);
                    component.ShouldBeSameAs(Component.A);
                };
                _groupA.OnEntityRemoved += delegate { this.Fail(); };
                _groupA.OnEntityUpdated += delegate { this.Fail(); };

                handleAddEA(eA1);
                didDispatch.ShouldBe(1);
            };

            it["doesn't dispatches OnEntityAdded when matching entity already has been added"] = () => {
                handleAddEA(eA1);
                _groupA.OnEntityAdded += delegate { this.Fail(); };
                _groupA.OnEntityRemoved += delegate { this.Fail(); };
                _groupA.OnEntityUpdated += delegate { this.Fail(); };
                handleAddEA(eA1);
                didDispatch.ShouldBe(0);
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
                    group.ShouldBeSameAs(_groupA);
                    entity.ShouldBeSameAs(eA1);
                    index.ShouldBe(CID.ComponentA);
                    component.ShouldBeSameAs(Component.A);
                };
                _groupA.OnEntityAdded += delegate { this.Fail(); };
                _groupA.OnEntityUpdated += delegate { this.Fail(); };

                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);

                didDispatch.ShouldBe(1);
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
                    group.ShouldBe(_groupA);
                    entity.ShouldBe(eA1);
                    index.ShouldBe(CID.ComponentA);
                    component.ShouldBeSameAs(Component.A);
                };
                _groupA.OnEntityAdded += (group, entity, index, component) => {
                    added += 1;
                    group.ShouldBe(_groupA);
                    entity.ShouldBe(eA1);
                    index.ShouldBe(CID.ComponentA);
                    component.ShouldBeSameAs(newComponentA);
                };
                _groupA.OnEntityUpdated += (group, entity, index, previousComponent, newComponent) => {
                    updated += 1;
                    group.ShouldBe(_groupA);
                    entity.ShouldBe(eA1);
                    index.ShouldBe(CID.ComponentA);
                    previousComponent.ShouldBeSameAs(Component.A);
                    newComponent.ShouldBeSameAs(newComponentA);
                };

                updateEA(eA1, newComponentA);

                removed.ShouldBe(1);
                added.ShouldBe(1);
                updated.ShouldBe(1);
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
                    _groupA.GetEntities().ShouldBeSameAs(cache);
                };

                it["updates cache when adding a new matching entity"] = () => {
                    handleSilently(eA2);
                    _groupA.GetEntities().ShouldNotBeSameAs(cache);
                };

                it["doesn't update cache when attempting to add a not matching entity"] = () => {
                    var e = this.CreateEntity();
                    handleSilently(e);
                    _groupA.GetEntities().ShouldBeSameAs(cache);
                };

                it["updates cache when removing an entity"] = () => {
                    eA1.RemoveComponentA();
                    handleSilently(eA1);
                    _groupA.GetEntities().ShouldNotBeSameAs(cache);
                };

                it["doesn't update cache when attempting to remove an entity that wasn't added before"] = () => {
                    eA2.RemoveComponentA();
                    handleSilently(eA2);
                    _groupA.GetEntities().ShouldBeSameAs(cache);
                };

                it["doesn't update cache when updating an entity"] = () => {
                    updateEA(eA1, new ComponentA());
                    _groupA.GetEntities().ShouldBeSameAs(cache);
                };
            };

            context["SingleEntity()"] = () => {

                IEntity cache = null;

                before = () => {
                    handleSilently(eA1);
                    cache = _groupA.GetSingleEntity();
                };

                it["gets cached singleEntities"] = () => {
                    _groupA.GetSingleEntity().ShouldBeSameAs(cache);
                };

                it["updates cache when new single entity was added"] = () => {
                    eA1.RemoveComponentA();
                    handleSilently(eA1);
                    handleSilently(eA2);
                    _groupA.GetSingleEntity().ShouldNotBeSameAs(cache);
                };

                it["updates cache when single entity is removed"] = () => {
                    eA1.RemoveComponentA();
                    handleSilently(eA1);
                    _groupA.GetSingleEntity().ShouldNotBeSameAs(cache);
                };

                it["doesn't update cache when single entity is updated"] = () => {
                    updateEA(eA1, new ComponentA());
                    _groupA.GetSingleEntity().ShouldBeSameAs(cache);
                };
            };
        };

        context["reference counting"] = () => {

            it["retains matched entity"] = () => {
                eA1.retainCount.ShouldBe(0);
                handleSilently(eA1);
                eA1.retainCount.ShouldBe(1);
            };

            it["releases removed entity"] = () => {
                handleSilently(eA1);
                eA1.RemoveComponentA();
                handleSilently(eA1);
                eA1.retainCount.ShouldBe(0);
            };

            it["invalidates entitiesCache (silent mode)"] = () => {
                var didExecute = 0;
                eA1.OnEntityReleased += entity => {
                    didExecute += 1;
                    _groupA.GetEntities().Length.ShouldBe(0);
                };
                handleSilently(eA1);
                _groupA.GetEntities();
                eA1.RemoveComponentA();
                handleSilently(eA1);
                didExecute.ShouldBe(1);
            };

            it["invalidates entitiesCache"] = () => {
                var didExecute = 0;
                eA1.OnEntityReleased += entity => {
                    didExecute += 1;
                    _groupA.GetEntities().Length.ShouldBe(0);
                };
                handleAddEA(eA1);
                _groupA.GetEntities();
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);
                didExecute.ShouldBe(1);
            };

            it["invalidates singleEntityCache (silent mode)"] = () => {
                var didExecute = 0;
                eA1.OnEntityReleased += entity => {
                    didExecute += 1;
                    _groupA.GetSingleEntity().ShouldBeNull();
                };
                handleSilently(eA1);
                _groupA.GetSingleEntity();
                eA1.RemoveComponentA();
                handleSilently(eA1);
                didExecute.ShouldBe(1);
            };

            it["invalidates singleEntityCache"] = () => {
                var didExecute = 0;
                eA1.OnEntityReleased += entity => {
                    didExecute += 1;
                    _groupA.GetSingleEntity().ShouldBeNull();
                };
                handleAddEA(eA1);
                _groupA.GetSingleEntity();
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);
                didExecute.ShouldBe(1);
            };

            it["retains entity until after event handlers were called"] = () => {
                handleAddEA(eA1);
                var didDispatch = 0;
                _groupA.OnEntityRemoved += (group, entity, index, component) => {
                    didDispatch += 1;
                    entity.retainCount.ShouldBe(1);
                };
                eA1.RemoveComponentA();
                handleRemoveEA(eA1, Component.A);

                didDispatch.ShouldBe(1);
                eA1.retainCount.ShouldBe(0);
            };
        };

        it["can ToString"] = () => {
            var m = Matcher<TestEntity>.AllOf(Matcher<TestEntity>.AllOf(0), Matcher<TestEntity>.AllOf(1));
            var group = new Group<TestEntity>(m);
            group.ToString().ShouldBe("Group(AllOf(0, 1))");
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
