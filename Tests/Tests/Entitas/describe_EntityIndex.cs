using Entitas;
using NSpec;

class describe_EntityIndex : nspec {

    void when_indexing() {

        EntityIndex<string> index = null;
        Pool pool = null;
        Group group = null;

        before = () => {
            pool = new Pool(CID.NumComponents);
            group = pool.GetGroup(Matcher.AllOf(CID.ComponentA));
            index = new EntityIndex<string>(group, c => ((NameAgeComponent)c).name);
        };

        context["when entity with key doesn't exist"] = () => {

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

        context["when entity with key exists"] = () => {

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

            it["gets entity with key"] = () => {
                index.GetEntity(name).should_be_same(entity);
            };

            it["gets entity when trying"] = () => {
                index.TryGetEntity(name).should_be_same(entity);
            };

            it["retains entity"] = () => {
                entity.retainCount.should_be(3); // Pool, Group, EntityIndex
            };

            it["has existing entity"] = () => {
                var newIndex = new EntityIndex<string>(group, c => ((NameAgeComponent)c).name);
                newIndex.HasEntity(name).should_be_true();
            };

            it["releases and removes entity from index when component gets removed"] = () => {
                entity.RemoveComponent(CID.ComponentA);
                index.HasEntity(name).should_be_false();
                entity.retainCount.should_be(1); // Pool
            };

            it["throws when adding an entity with the same key"] = expect<EntityIndexException>(() => {
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

            // TODO
            // Deconstructor Unity test
            // Profile in Unity
            // Multiple with same key, what happens? Single, Multiple possible?
        };
    }
}

