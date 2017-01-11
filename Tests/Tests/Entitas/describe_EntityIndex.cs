using Entitas;
using NSpec;
using System.Collections.Generic;

class describe_EntityIndex : nspec {

    void when_primary_index() {

        PrimaryEntityIndex<string> index = null;
        Context ctx = null;
        Group group = null;

        before = () => {
            ctx = new Context(CID.TotalComponents);
            group = ctx.GetGroup(Matcher.AllOf(CID.ComponentA));
            index = new PrimaryEntityIndex<string>(group, (e, c) => {
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
            Entity entity = null;

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
                var newIndex = new PrimaryEntityIndex<string>(group, (e, c) => {
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

                context["when actrivated"] = () => {

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
    }

    void when_index() {

        EntityIndex<string> index = null;
        Context ctx = null;
        Group group = null;

        before = () => {
            ctx = new Context(CID.TotalComponents);
            group = ctx.GetGroup(Matcher.AllOf(CID.ComponentA));
            index = new EntityIndex<string>(group, (e, c) => {
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
            Entity entity1 = null;
            Entity entity2 = null;

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
                var newIndex = new EntityIndex<string>(group, (e, c) => {
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

                context["when actrivated"] = () => {

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
    }

    void when_index_multiple_components() {

        #pragma warning disable
        EntityIndex<string> index = null;
        Context ctx = null;
        Group group = null;

        before = () => {
            ctx = new Context(CID.TotalComponents);
        };

        it["gets last component that triggered adding entity to group"] = () => {

            IComponent receivedComponent = null;

            group = ctx.GetGroup(Matcher.AllOf(CID.ComponentA, CID.ComponentB));
            index = new EntityIndex<string>(group, (e, c) => {
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

            group = ctx.GetGroup(Matcher.AllOf(CID.ComponentA).NoneOf(CID.ComponentB));
            index = new EntityIndex<string>(group, (e, c) => {
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
