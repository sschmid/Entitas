using Entitas;
using NSpec;
using System.Collections.Generic;

class describe_EntityIndex : nspec {

    void when_primary_index() {

        PrimaryEntityIndex<string> index = null;
        Pool pool = null;
        Group group = null;

        before = () => {
            pool = new Pool(CID.NumComponents);
            group = pool.GetGroup(Matcher.AllOf(CID.ComponentA));
            index = new PrimaryEntityIndex<string>(group, (e, c) => ((NameAgeComponent)c).name);
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
                entity = pool.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
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
                entity.retainCount.should_be(3); // Pool, Group, EntityIndex
            };

            it["has existing entity"] = () => {
                var newIndex = new PrimaryEntityIndex<string>(group, (e, c) => ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name);
                newIndex.HasEntity(name).should_be_true();
            };

            it["releases and removes entity from index when component gets removed"] = () => {
                entity.RemoveComponent(CID.ComponentA);
                index.HasEntity(name).should_be_false();
                entity.retainCount.should_be(1); // Pool
            };

            it["throws when adding an entity for the same key"] = expect<EntityIndexException>(() => {
                var nameAgeComponent = new NameAgeComponent();
                nameAgeComponent.name = name;
                entity = pool.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
            });

            context["when deactivated"] = () => {

                before = () => {
                    index.Deactivate();
                };

                it["clears index and releases entity"] = () => {
                    index.HasEntity(name).should_be_false();
                    entity.retainCount.should_be(2); // Pool, Group
                };

                it["doesn't add entities anymore"] = () => {
                    var nameAgeComponent = new NameAgeComponent();
                    nameAgeComponent.name = name;
                    pool.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
                    index.HasEntity(name).should_be_false();
                };
            };
        };
    }

    void when_index() {

        EntityIndex<string> index = null;
        Pool pool = null;
        Group group = null;

        before = () => {
            pool = new Pool(CID.NumComponents);
            group = pool.GetGroup(Matcher.AllOf(CID.ComponentA));
            index = new EntityIndex<string>(group, (e, c) => ((NameAgeComponent)c).name);
        };

        context["when entity for key doesn't exist"] = () => {

            it["has no entities"] = () => {
                index.GetEntities("unknownKey").should_be_empty();
            };
        };

        context["when entity for key exists"] = () => {

            const string name = "Max";
            Entity entity1 = null;
            Entity entity2 = null;

            before = () => {
                var nameAgeComponent = new NameAgeComponent();
                nameAgeComponent.name = name;
                entity1 = pool.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
                entity2 = pool.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
            };

            it["gets entities for key"] = () => {
                var entities = index.GetEntities(name);
                entities.Count.should_be(2);
                entities.should_contain(entity1);
                entities.should_contain(entity2);
            };

            it["retains entity"] = () => {
                entity1.retainCount.should_be(3); // Pool, Group, EntityIndex
                entity2.retainCount.should_be(3); // Pool, Group, EntityIndex
            };

            it["has existing entity"] = () => {
                var newIndex = new EntityIndex<string>(group, (e, c) => ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name);
                newIndex.GetEntities(name).Count.should_be(2);
            };

            it["releases and removes entity from index when component gets removed"] = () => {
                entity1.RemoveComponent(CID.ComponentA);
                index.GetEntities(name).Count.should_be(1);
                entity1.retainCount.should_be(1); // Pool
            };

            context["when deactivated"] = () => {

                before = () => {
                    index.Deactivate();
                };

                it["clears index and releases entity"] = () => {
                    index.GetEntities(name).should_be_empty();
                    entity1.retainCount.should_be(2); // Pool, Group
                    entity2.retainCount.should_be(2); // Pool, Group
                };

                it["doesn't add entities anymore"] = () => {
                    var nameAgeComponent = new NameAgeComponent();
                    nameAgeComponent.name = name;
                    pool.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent);
                    index.GetEntities(name).should_be_empty();
                };
            };
        };
    }

    void when_index_multiple_components() {

        EntityIndex<string> index = null;
        Pool pool = null;
        Group group = null;

        before = () => {
            pool = new Pool(CID.NumComponents);
        };

        it["gets last component that triggered adding entity to group"] = () => {

            IComponent receivedComponent = null;

            group = pool.GetGroup(Matcher.AllOf(CID.ComponentA, CID.ComponentB));
            index = new EntityIndex<string>(group, (e, c) => {
                receivedComponent = c;
                return ((NameAgeComponent)c).name;
            });

            var nameAgeComponent1 = new NameAgeComponent();
            nameAgeComponent1.name = "Max";

            var nameAgeComponent2 = new NameAgeComponent();
            nameAgeComponent2.name = "Jack";

            pool.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent1)
                               .AddComponent(CID.ComponentB, nameAgeComponent2);

            receivedComponent.should_be_same(nameAgeComponent2);
        };

        it["works with NoneOf"] = () => {

            var receivedComponents = new List<IComponent>();

            var nameAgeComponent1 = new NameAgeComponent();
            nameAgeComponent1.name = "Max";

            var nameAgeComponent2 = new NameAgeComponent();
            nameAgeComponent2.name = "Jack";

            group = pool.GetGroup(Matcher.AllOf(CID.ComponentA).NoneOf(CID.ComponentB));
            index = new EntityIndex<string>(group, (e, c) => {
                receivedComponents.Add(c);

                if (c == nameAgeComponent1) {
                    return ((NameAgeComponent)c).name;
                }

                return ((NameAgeComponent)e.GetComponent(CID.ComponentA)).name;
            });

            pool.CreateEntity().AddComponent(CID.ComponentA, nameAgeComponent1)
                               .AddComponent(CID.ComponentB, nameAgeComponent2);

            receivedComponents.Count.should_be(2);
        };
    }
}

