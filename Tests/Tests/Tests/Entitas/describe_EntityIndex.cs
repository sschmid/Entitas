using Entitas;
using NSpec;
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
                index = new PrimaryEntityIndex<TestEntity, string>(group, (e, c) => {
                    var nameAge = c as NameAgeComponent;
                    return nameAge != null
                        ? nameAge.name
                        : ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
                });
            };

            context["when entity for key doesn't exist"] = () => {

                it["doesn't have entity"] = () => {
                    index.HasEntity("unknownKey").should_be_false();
                };

                it["throws exception when attempting to get entity"] = expect<EntityIndexException>(() => {
                    index.GetEntity("unknownKey");
                });

                it["returns null when trying to get entity"] = () => {
                    index.TryGetEntity("unknownKey").should_be_null();
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

                it["has entity"] = () => {
                    index.HasEntity(name).should_be_true();
                };

                it["gets entity for key"] = () => {
                    index.GetEntity(name).should_be_same(entity);
                };

                it["gets entity when trying"] = () => {
                    index.TryGetEntity(name).should_be_same(entity);
                };

                it["retains entity"] = () => {
                    entity.retainCount.should_be(3); // Context, Group, EntityIndex
                };

                it["has existing entity"] = () => {
                    var newIndex = new PrimaryEntityIndex<TestEntity, string>(group, (e, c) => {
                        var nameAge = c as NameAgeComponent;
                        return nameAge != null
                            ? nameAge.name
                            : ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
                    });
                    newIndex.HasEntity(name).should_be_true();
                };

                it["releases and removes entity from index when component gets removed"] = () => {
                    entity.RemoveComponent(CID.ComponentA);
                    index.HasEntity(name).should_be_false();
                    entity.retainCount.should_be(1); // Context
                };

                it["throws when adding an entity for the same key"] = expect<EntityIndexException>(() => {
                    var nameAgeComponent = new NameAgeComponent();
                    nameAgeComponent.name = name;
                    entity = ctx.CreateEntity();
                    entity.AddComponent(CID.ComponentA, nameAgeComponent);
                });

                context["when deactivated"] = () => {

                    before = () => {
                        index.Deactivate();
                    };

                    it["clears index and releases entity"] = () => {
                        index.HasEntity(name).should_be_false();
                        entity.retainCount.should_be(2); // Context, Group
                    };

                    it["doesn't add entities anymore"] = () => {
                        var nameAgeComponent = new NameAgeComponent();
                        nameAgeComponent.name = name;
                        ctx.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
                        index.HasEntity(name).should_be_false();
                    };

                    context["when activated"] = () => {

                        before = () => {
                            index.Activate();
                        };

                        it["has existing entity"] = () => {
                            index.HasEntity(name).should_be_true();
                        };

                        it["adds new entities"] = () => {
                            var nameAgeComponent = new NameAgeComponent();
                            nameAgeComponent.name = "Jack";
                            entity = ctx.CreateEntity();
                            entity.AddComponent(CID.ComponentA, nameAgeComponent);

                            index.HasEntity("Jack").should_be_true();
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
                index = new PrimaryEntityIndex<TestEntity, string>(group, (e, c) => {
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
                    entity.owners.should_contain(index);
                };

                it["has entity"] = () => {
                    index.HasEntity(name + "1").should_be_true();
                    index.HasEntity(name + "2").should_be_true();
                };

                it["gets entity for key"] = () => {
                    index.GetEntity(name + "1").should_be_same(entity);
                    index.GetEntity(name + "2").should_be_same(entity);
                };

                it["releases and removes entity from index when component gets removed"] = () => {
                    entity.RemoveComponent(CID.ComponentA);
                    index.HasEntity(name + "1").should_be_false();
                    index.HasEntity(name + "2").should_be_false();
                    entity.owners.should_not_contain(index);
                };

                it["has existing entity"] = () => {
                    index.Deactivate();
                    index.Activate();
                    index.GetEntity(name + "1").should_be_same(entity);
                    index.GetEntity(name + "2").should_be_same(entity);
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
                index = new EntityIndex<TestEntity, string>(group, (e, c) => {
                    var nameAge = c as NameAgeComponent;
                    return nameAge != null
                        ? nameAge.name
                        : ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
                });
            };

            context["when entity for key doesn't exist"] = () => {

                it["has no entities"] = () => {
                    index.GetEntities("unknownKey").should_be_empty();
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
                    entities.Count.should_be(2);
                    entities.should_contain(entity1);
                    entities.should_contain(entity2);
                };

                it["retains entity"] = () => {
                    entity1.retainCount.should_be(3); // Context, Group, EntityIndex
                    entity2.retainCount.should_be(3); // Context, Group, EntityIndex
                };

                it["has existing entities"] = () => {
                    var newIndex = new EntityIndex<TestEntity, string>(group, (e, c) => {
                        var nameAge = c as NameAgeComponent;
                        return nameAge != null
                            ? nameAge.name
                            : ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
                    });
                    newIndex.GetEntities(name).Count.should_be(2);
                };  

                it["releases and removes entity from index when component gets removed"] = () => {
                    entity1.RemoveComponent(CID.ComponentA);
                    index.GetEntities(name).Count.should_be(1);
                    entity1.retainCount.should_be(1); // Context
                };

                context["when deactivated"] = () => {

                    before = () => {
                        index.Deactivate();
                    };

                    it["clears index and releases entity"] = () => {
                        index.GetEntities(name).should_be_empty();
                        entity1.retainCount.should_be(2); // Context, Group
                        entity2.retainCount.should_be(2); // Context, Group
                    };

                    it["doesn't add entities anymore"] = () => {
                        ctx.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
                        index.GetEntities(name).should_be_empty();
                    };

                    context["when activated"] = () => {

                        before = () => {
                            index.Activate();
                        };

                        it["has existing entities"] = () => {
                            var entities = index.GetEntities(name);
                            entities.Count.should_be(2);
                            entities.should_contain(entity1);
                            entities.should_contain(entity2);
                        };

                        it["adds new entities"] = () => {
                            var entity3 = ctx.CreateEntity();
                            entity3.AddComponent(CID.ComponentA, nameAgeComponent);

                            var entities = index.GetEntities(name);
                            entities.Count.should_be(3);
                            entities.should_contain(entity1);
                            entities.should_contain(entity2);
                            entities.should_contain(entity3);
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
                index = new EntityIndex<TestEntity, string>(group, (e, c) => {
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
                    entity1.owners.should_contain(index);
                    entity2.owners.should_contain(index);
                };

                it["has entity"] = () => {
                    index.GetEntities("1").Count.should_be(1);
                    index.GetEntities("2").Count.should_be(2);
                    index.GetEntities("3").Count.should_be(1);
                };

                it["gets entity for key"] = () => {
                    index.GetEntities("1").First().should_be_same(entity1);
                    index.GetEntities("2").should_contain(entity1);
                    index.GetEntities("2").should_contain(entity2);
                    index.GetEntities("3").First().should_be_same(entity2);
                };

                it["releases and removes entity from index when component gets removed"] = () => {
                    entity1.RemoveComponent(CID.ComponentA);
                    index.GetEntities("1").Count.should_be(0);
                    index.GetEntities("2").Count.should_be(1);
                    index.GetEntities("3").Count.should_be(1);
                    entity1.owners.should_not_contain(index);
                    entity2.owners.should_contain(index);
                };

                it["has existing entities"] = () => {
                    index.Deactivate();
                    index.Activate();
                    index.GetEntities("1").First().should_be_same(entity1);
                    index.GetEntities("2").should_contain(entity1);
                    index.GetEntities("2").should_contain(entity2);
                    index.GetEntities("3").First().should_be_same(entity2);
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
            index = new EntityIndex<TestEntity, string>(group, (e, c) => {
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

            receivedComponent.should_be_same(nameAgeComponent2);
        };

        it["works with NoneOf"] = () => {

            var receivedComponents = new List<IComponent>();

            var nameAgeComponent1 = new NameAgeComponent();
            nameAgeComponent1.name = "Max";

            var nameAgeComponent2 = new NameAgeComponent();
            nameAgeComponent2.name = "Jack";

            group = ctx.GetGroup(Matcher<TestEntity>.AllOf(CID.ComponentA).NoneOf(CID.ComponentB));
            index = new EntityIndex<TestEntity, string>(group, (e, c) => {
                receivedComponents.Add(c);

                if(c == nameAgeComponent1) {
                    return ((NameAgeComponent)c).name;
                }

                return ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
            });

            var entity = ctx.CreateEntity();
            entity.AddComponent(CID.ComponentA, nameAgeComponent1);
            entity.AddComponent(CID.ComponentB, nameAgeComponent2);

            receivedComponents.Count.should_be(2);
            receivedComponents[0].should_be(nameAgeComponent1);
            receivedComponents[1].should_be(nameAgeComponent2);
        };
    }
}
