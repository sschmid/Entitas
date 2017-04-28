using System;
using Entitas;
using Entitas.Blueprints;
using NSpec;

class describe_EntitasErrorMessages : nspec {

    static void printErrorMessage(Action action) {
        try {
            action();
        } catch(Exception exception) {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine("Exception preview for: " + exception.GetType());
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine(exception.Message);
            Console.WriteLine("================================================================================");
            Console.ResetColor();
        }
    }

    void when_throwing() {

        MyTestContext ctx = null;
        TestEntity entity = null;

        before = () => {
            var componentNames = new [] { "Health", "Position", "View" };
            var contextInfo = new ContextInfo("My Context", componentNames, null);
            ctx = new MyTestContext(componentNames.Length, 42, contextInfo);
            entity = ctx.CreateEntity();
        };

        context["Entity"] = () => {

            context["when not enabled"] = () => {

                before = () => {
                    entity.Destroy();
                };

                it["add a component"] = () => printErrorMessage(() => entity.AddComponentA());
                it["remove a component"] = () => printErrorMessage(() => entity.RemoveComponentA());
                it["replace a component"] = () => printErrorMessage(() => entity.ReplaceComponentA(Component.A));
            };

            context["when enabled"] = () => {

                it["add a component twice"] = () => printErrorMessage(() => {
                    entity.AddComponentA();
                    entity.AddComponentA();
                });

                it["remove a component that doesn't exist"] = () => printErrorMessage(() => {
                    entity.RemoveComponentA();
                });

                it["get a component that doesn't exist"] = () => printErrorMessage(() => {
                    entity.GetComponentA();
                });

                it["retain an entity twice"] = () => printErrorMessage(() => {
                    var owner = new object();
                    entity.Retain(owner);
                    entity.Retain(owner);
                });

                it["release an entity with wrong owner"] = () => printErrorMessage(() => {
                    var owner = new object();
                    entity.Release(owner);
                });
            };
        };

        context["Group"] = () => {

            it["get single entity when multiple exist"] = () => printErrorMessage(() => {
                ctx.CreateEntity().AddComponentA();
                ctx.CreateEntity().AddComponentA();
                var matcher = (Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentA);
                matcher.componentNames = ctx.contextInfo.componentNames;
                var group = ctx.GetGroup(matcher);
                group.GetSingleEntity();
            });
        };

        context["Collector<TestEntity>"] = () => {

            it["unbalanced goups"] = () => printErrorMessage(() => {
                var g1 = new Group<TestEntity>(Matcher<TestEntity>.AllOf(CID.ComponentA));
                var g2 = new Group<TestEntity>(Matcher<TestEntity>.AllOf(CID.ComponentB));
                var e1 = GroupEvent.Added;

                new Collector<TestEntity>(new [] { g1, g2 }, new [] { e1 });
            });
        };

        context["Context"] = () => {

            it["wrong ContextInfo componentNames count"] = () => printErrorMessage(() => {
                var componentNames = new [] { "Health", "Position", "View" };
                var contextInfo = new ContextInfo("My Context", componentNames, null);
                new MyTestContext(1, 0, contextInfo);
            });

            it["destroy retained entities"] = () => printErrorMessage(() => {
                ctx.CreateEntity().Retain(this);
                ctx.DestroyAllEntities();
            });

            it["releases entity before destroy"] = () => printErrorMessage(() => {
                entity.Release(ctx);
            });

            it["unknown entityIndex"] = () => printErrorMessage(() => {
                ctx.GetEntityIndex("unknown");
            });

            it["duplicate entityIndex"] = () => printErrorMessage(() => {
                var groupA = ctx.GetGroup((Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentA));
                var index = new PrimaryEntityIndex<TestEntity, string>("TestIndex", groupA, (arg1, arg2) => string.Empty);
                ctx.AddEntityIndex(index);
                ctx.AddEntityIndex(index);
            });
        };

        context["CollectionExtension"] = () => {

            it["get single entity when more than one exist"] = () => printErrorMessage(() => {
                new IEntity[2].SingleEntity();
            });
        };

        context["ComponentBlueprint"] = () => {

            it["type doesn't implement IComponent"] = () => printErrorMessage(() => {
                var componentBlueprint = new ComponentBlueprint();
                componentBlueprint.fullTypeName = "string";
                componentBlueprint.CreateComponent(entity);
            });

            it["type doesn't exist"] = () => printErrorMessage(() => {
                var componentBlueprint = new ComponentBlueprint();
                componentBlueprint.fullTypeName = "UnknownType";
                componentBlueprint.CreateComponent(entity);
            });

            it["invalid field name"] = () => printErrorMessage(() => {
                var componentBlueprint = new ComponentBlueprint();
                componentBlueprint.index = 0;
                componentBlueprint.fullTypeName = typeof(NameAgeComponent).FullName;
                componentBlueprint.members = new [] {
                    new SerializableMember("xxx", "publicFieldValue"),
                    new SerializableMember("publicProperty", "publicPropertyValue")
                };
                componentBlueprint.CreateComponent(entity);
            });
        };

        context["EntityIndex"] = () => {

            it["no entity with key"] = () => printErrorMessage(() => {
                var groupA = ctx.GetGroup((Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentA));
                var index = new PrimaryEntityIndex<TestEntity, string>("TestIndex", groupA, (e, c) => ((NameAgeComponent)c).name);
                index.GetEntity("unknownKey");
            });

            it["multiple entities for primary key"] = () => printErrorMessage(() => {
                var groupA = ctx.GetGroup((Matcher<TestEntity>)Matcher<TestEntity>.AllOf(CID.ComponentA));
                new PrimaryEntityIndex<TestEntity, string>("TestIndex", groupA, (e, c) => ((NameAgeComponent)c).name);

                var nameAge = new NameAgeComponent();
                nameAge.name = "Max";
                nameAge.age = 42;

                ctx.CreateEntity().AddComponent(CID.ComponentA, nameAge);
                ctx.CreateEntity().AddComponent(CID.ComponentA, nameAge);
            });
        };
    }
}
