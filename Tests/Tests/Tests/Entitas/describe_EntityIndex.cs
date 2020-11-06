using Entitas;
using NSpec;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

class describe_EntityIndex : nspec {

    void when_primary_index() {

        context["single key"] = () => {

            PrimaryEntityIndex<TestEntity, string> index = null;
            IContext<TestEntity> ctx = null;
            IGroup<TestEntity> group = null;

            before = () => {
                ctx = new MyTestContext();
                group = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
                index = new PrimaryEntityIndex<TestEntity, string>("TestIndex", group, (e, c) => {
                    var nameAge = c as NameAgeComponent;
                    return nameAge != null
                        ? nameAge.name
                        : ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
                });
            };

            context["when entity for key doesn't exist"] = () => {

                it["returns null when getting entity for unknown key"] = () => {
                    index.GetEntity("unknownKey").ShouldBeNull();
                };
            };

            context["when entity for key exists"] = () => {

                const string name = "Max";
                TestEntity entity = null;

                before = () => {
                    var nameAgeComponent = new NameAgeComponent();
                    nameAgeComponent.name = name;
                    entity = ctx.CreateEntity();
                    entity.AddComponent(CID.ComponentA, nameAgeComponent);
                };

                it["gets entity for key"] = () => {
                    index.GetEntity(name).ShouldBeSameAs(entity);
                };

                it["retains entity"] = () => {
                    entity.retainCount.ShouldBe(3); // Context, Group, EntityIndex
                };

                it["has existing entity"] = () => {
                    var newIndex = new PrimaryEntityIndex<TestEntity, string>("TestIndex", group, (e, c) => {
                        var nameAge = c as NameAgeComponent;
                        return nameAge != null
                            ? nameAge.name
                            : ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
                    });
                    newIndex.GetEntity(name).ShouldBeSameAs(entity);
                };

                it["releases and removes entity from index when component gets removed"] = () => {
                    entity.RemoveComponent(CID.ComponentA);
                    index.GetEntity(name).ShouldBeNull();
                    entity.retainCount.ShouldBe(1); // Context
                };

                it["throws when adding an entity for the same key"] = expect<EntityIndexException>(() => {
                    var nameAgeComponent = new NameAgeComponent();
                    nameAgeComponent.name = name;
                    entity = ctx.CreateEntity();
                    entity.AddComponent(CID.ComponentA, nameAgeComponent);
                });

                it["can ToString"] = () => {
                    index.ToString().ShouldBe("PrimaryEntityIndex(TestIndex)");
                };

                context["when deactivated"] = () => {

                    before = () => {
                        index.Deactivate();
                    };

                    it["clears index and releases entity"] = () => {
                        index.GetEntity(name).ShouldBeNull();
                        entity.retainCount.ShouldBe(2); // Context, Group
                    };

                    it["doesn't add entities anymore"] = () => {
                        var nameAgeComponent = new NameAgeComponent();
                        nameAgeComponent.name = name;
                        ctx.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
                        index.GetEntity(name).ShouldBeNull();
                    };

                    context["when activated"] = () => {

                        before = () => {
                            index.Activate();
                        };

                        it["has existing entity"] = () => {
                            index.GetEntity(name).ShouldBeSameAs(entity);
                        };

                        it["adds new entities"] = () => {
                            var nameAgeComponent = new NameAgeComponent();
                            nameAgeComponent.name = "Jack";
                            entity = ctx.CreateEntity();
                            entity.AddComponent(CID.ComponentA, nameAgeComponent);

                            index.GetEntity("Jack").ShouldBeSameAs(entity);
                        };
                    };
                };
            };
        };

        context["multiple keys"] = () => {

            PrimaryEntityIndex<TestEntity, string> index = null;
            IContext<TestEntity> ctx = null;
            IGroup<TestEntity> group = null;

            before = () => {
                ctx = new MyTestContext();
                group = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
                index = new PrimaryEntityIndex<TestEntity, string>("TestIndex", group, (e, c) => {
                    var nameAge = c as NameAgeComponent;
                    return nameAge != null
                        ? new [] { nameAge.name + "1", nameAge.name + "2" }
                        : new [] { ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name + "1", ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name + "2" };
                });
            };

            context["when entity for key exists"] = () => {

                const string name = "Max";
                TestEntity entity = null;

                before = () => {
                    var nameAgeComponent = new NameAgeComponent();
                    nameAgeComponent.name = name;
                    entity = ctx.CreateEntity();
                    entity.AddComponent(CID.ComponentA, nameAgeComponent);
                };

                it["retains entity"] = () => {
                    entity.retainCount.ShouldBe(3);

                    var safeAerc = entity.aerc as SafeAERC;
                    if (safeAerc != null) {
                        safeAerc.owners.ShouldContain(index);
                    }
                };

                it["gets entity for key"] = () => {
                    index.GetEntity(name + "1").ShouldBeSameAs(entity);
                    index.GetEntity(name + "2").ShouldBeSameAs(entity);
                };

                it["releases and removes entity from index when component gets removed"] = () => {
                    entity.RemoveComponent(CID.ComponentA);
                    index.GetEntity(name + "1").ShouldBeNull();
                    index.GetEntity(name + "2").ShouldBeNull();
                    entity.retainCount.ShouldBe(1);

                    var safeAerc = entity.aerc as SafeAERC;
                    if (safeAerc != null) {
                        safeAerc.owners.ShouldNotContain(index);
                    }
                };

                it["has existing entity"] = () => {
                    index.Deactivate();
                    index.Activate();
                    index.GetEntity(name + "1").ShouldBeSameAs(entity);
                    index.GetEntity(name + "2").ShouldBeSameAs(entity);
                };
            };
        };
    }

    void when_index() {

        context["single key"] = () => {

            EntityIndex<TestEntity, string> index = null;
            IContext<TestEntity> ctx = null;
            IGroup<TestEntity> group = null;

            before = () => {
                ctx = new MyTestContext();
                group = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
                index = new EntityIndex<TestEntity, string>("TestIndex", group, (e, c) => {
                    var nameAge = c as NameAgeComponent;
                    return nameAge != null
                        ? nameAge.name
                        : ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
                });
            };

            context["when entity for key doesn't exist"] = () => {

                it["has no entities"] = () => {
                    index.GetEntities("unknownKey").ShouldBeEmpty();
                };
            };

            context["when entity for key exists"] = () => {

                const string name = "Max";
                NameAgeComponent nameAgeComponent = null;
                TestEntity entity1 = null;
                TestEntity entity2 = null;

                before = () => {
                    nameAgeComponent = new NameAgeComponent();
                    nameAgeComponent.name = name;
                    entity1 = ctx.CreateEntity();
                    entity1.AddComponent(CID.ComponentA, nameAgeComponent);
                    entity2 = ctx.CreateEntity();
                    entity2.AddComponent(CID.ComponentA, nameAgeComponent);
                };

                it["gets entities for key"] = () => {
                    var entities = index.GetEntities(name);
                    entities.Count.ShouldBe(2);
                    entities.ShouldContain(entity1);
                    entities.ShouldContain(entity2);
                };

                it["retains entity"] = () => {
                    entity1.retainCount.ShouldBe(3); // Context, Group, EntityIndex
                    entity2.retainCount.ShouldBe(3); // Context, Group, EntityIndex
                };

                it["has existing entities"] = () => {
                    var newIndex = new EntityIndex<TestEntity, string>("TestIndex", group, (e, c) => {
                        var nameAge = c as NameAgeComponent;
                        return nameAge != null
                            ? nameAge.name
                            : ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
                    });
                    newIndex.GetEntities(name).Count.ShouldBe(2);
                };

                it["releases and removes entity from index when component gets removed"] = () => {
                    entity1.RemoveComponent(CID.ComponentA);
                    index.GetEntities(name).Count.ShouldBe(1);
                    entity1.retainCount.ShouldBe(1); // Context
                };

                it["can ToString"] = () => {
                    index.ToString().ShouldBe("EntityIndex(TestIndex)");
                };

                context["when deactivated"] = () => {

                    before = () => {
                        index.Deactivate();
                    };

                    it["clears index and releases entity"] = () => {
                        index.GetEntities(name).ShouldBeEmpty();
                        entity1.retainCount.ShouldBe(2); // Context, Group
                        entity2.retainCount.ShouldBe(2); // Context, Group
                    };

                    it["doesn't add entities anymore"] = () => {
                        ctx.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
                        index.GetEntities(name).ShouldBeEmpty();
                    };

                    context["when activated"] = () => {

                        before = () => {
                            index.Activate();
                        };

                        it["has existing entities"] = () => {
                            var entities = index.GetEntities(name);
                            entities.Count.ShouldBe(2);
                            entities.ShouldContain(entity1);
                            entities.ShouldContain(entity2);
                        };

                        it["adds new entities"] = () => {
                            var entity3 = ctx.CreateEntity();
                            entity3.AddComponent(CID.ComponentA, nameAgeComponent);

                            var entities = index.GetEntities(name);
                            entities.Count.ShouldBe(3);
                            entities.ShouldContain(entity1);
                            entities.ShouldContain(entity2);
                            entities.ShouldContain(entity3);
                        };
                    };
                };
            };
        };

        context["multiple keys"] = () => {

            EntityIndex<TestEntity, string> index = null;
            IContext<TestEntity> ctx = null;
            IGroup<TestEntity> group = null;
            TestEntity entity1 = null;
            TestEntity entity2 = null;

            before = () => {
                ctx = new MyTestContext();
                group = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA));
                index = new EntityIndex<TestEntity, string>("TestIndex", group, (e, c) => {
                    return e == entity1
                        ? new [] { "1", "2" }
                        : new [] { "2", "3" };
                });
            };

            context["when entity for key exists"] = () => {

                before = () => {
                    entity1 = ctx.CreateEntity();
                    entity1.AddComponentA();
                    entity2 = ctx.CreateEntity();
                    entity2.AddComponentA();
                };

                it["retains entity"] = () => {
                    entity1.retainCount.ShouldBe(3);
                    entity2.retainCount.ShouldBe(3);

                    var safeAerc1 = entity1.aerc as SafeAERC;
                    if (safeAerc1 != null) {
                        safeAerc1.owners.ShouldContain(index);
                    }

                    var safeAerc2 = entity1.aerc as SafeAERC;
                    if (safeAerc2 != null) {
                        safeAerc2.owners.ShouldContain(index);
                    }
                };

                it["has entity"] = () => {
                    index.GetEntities("1").Count.ShouldBe(1);
                    index.GetEntities("2").Count.ShouldBe(2);
                    index.GetEntities("3").Count.ShouldBe(1);
                };

                it["gets entity for key"] = () => {
                    index.GetEntities("1").First().ShouldBeSameAs(entity1);
                    index.GetEntities("2").ShouldContain(entity1);
                    index.GetEntities("2").ShouldContain(entity2);
                    index.GetEntities("3").First().ShouldBeSameAs(entity2);
                };

                it["releases and removes entity from index when component gets removed"] = () => {
                    entity1.RemoveComponent(CID.ComponentA);
                    index.GetEntities("1").Count.ShouldBe(0);
                    index.GetEntities("2").Count.ShouldBe(1);
                    index.GetEntities("3").Count.ShouldBe(1);

                    entity1.retainCount.ShouldBe(1);
                    entity2.retainCount.ShouldBe(3);

                    var safeAerc1 = entity1.aerc as SafeAERC;
                    if (safeAerc1 != null) {
                        safeAerc1.owners.ShouldNotContain(index);
                    }

                    var safeAerc2 = entity2.aerc as SafeAERC;
                    if (safeAerc2 != null) {
                        safeAerc2.owners.ShouldContain(index);
                    }
                };

                it["has existing entities"] = () => {
                    index.Deactivate();
                    index.Activate();
                    index.GetEntities("1").First().ShouldBeSameAs(entity1);
                    index.GetEntities("2").ShouldContain(entity1);
                    index.GetEntities("2").ShouldContain(entity2);
                    index.GetEntities("3").First().ShouldBeSameAs(entity2);
                };
            };
        };
    }

    void when_index_multiple_components() {

        #pragma warning disable
        EntityIndex<TestEntity, string> index = null;
        IContext<TestEntity> ctx = null;
        IGroup<TestEntity> group = null;

        before = () => {
            ctx = new MyTestContext();
        };

        it["gets last component that triggered adding entity to group"] = () => {

            IComponent receivedComponent = null;

            group = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA, CID.ComponentB));
            index = new EntityIndex<TestEntity, string>("TestIndex", group, (e, c) => {
                receivedComponent = c;
                return ((NameAgeComponent)c).name;
            });

            var nameAgeComponent1 = new NameAgeComponent();
            nameAgeComponent1.name = "Max";

            var nameAgeComponent2 = new NameAgeComponent();
            nameAgeComponent2.name = "Jack";

            var entity = ctx.CreateEntity();
            entity.AddComponent(CID.ComponentA, nameAgeComponent1);
            entity.AddComponent(CID.ComponentB, nameAgeComponent2);

            receivedComponent.ShouldBeSameAs(nameAgeComponent2);
        };

        it["works with NoneOf"] = () => {

            var receivedComponents = new List<IComponent>();

            var nameAgeComponent1 = new NameAgeComponent();
            nameAgeComponent1.name = "Max";

            var nameAgeComponent2 = new NameAgeComponent();
            nameAgeComponent2.name = "Jack";

            group = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA).NoneOf(CID.ComponentB));
            index = new EntityIndex<TestEntity, string>("TestIndex", group, (e, c) => {
                receivedComponents.Add(c);

                if (c == nameAgeComponent1) {
                    return ((NameAgeComponent)c).name;
                }

                return ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
            });

            var entity = ctx.CreateEntity();
            entity.AddComponent(CID.ComponentA, nameAgeComponent1);
            entity.AddComponent(CID.ComponentB, nameAgeComponent2);

            receivedComponents.Count.ShouldBe(2);
            receivedComponents[0].ShouldBe(nameAgeComponent1);
            receivedComponents[1].ShouldBe(nameAgeComponent2);
        };
    }
}
