using NSpec;
using Entitas;
using System;

class describe_EntitasErrorMessages : nspec {
    static void printErrorMessage(Action action) {
        try {
            action();
        } catch (Exception exception) {
            Console.ForegroundColor = ConsoleColor.DarkCyan;
            Console.WriteLine("================================================================================");
            Console.WriteLine(exception.GetType());
            Console.WriteLine("--------------------------------------------------------------------------------");
            Console.WriteLine(exception.Message);
            Console.WriteLine("================================================================================");
            Console.ResetColor();
        }
    }

    void when_throwing() {

        Pool pool = null;
        Entity entity = null;
        before = () => {
            var componentNames = new [] { "Health", "Position", "View" };
            var metaData = new PoolMetaData("My Pool", componentNames);
            pool = new Pool(componentNames.Length, 42, metaData);
            entity = pool.CreateEntity();
        };

        it["creates exception with hint separated by newLine"] = () => {
            var msg = "Message";
            var hint = "Hint";
            var ex = new EntitasException(msg, hint);
            ex.Message.should_be(msg + "\n" + hint);
        };

        it["ignores hint when null"] = () => {
            var msg = "Message";
            string hint = null;
            var ex = new EntitasException(msg, hint);
            ex.Message.should_be(msg);
        };

        context["Entity"] = () => {

            context["when not enabled"] = () => {

                before = () => {
                    pool.DestroyEntity(entity);
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
                pool.CreateEntity().AddComponentA();
                pool.CreateEntity().AddComponentA();
                var matcher = (Matcher)Matcher.AllOf(CID.ComponentA);
                matcher.componentNames = new [] { "Health", "Position", "View" };
                var group = pool.GetGroup(matcher);
                group.GetSingleEntity();
            });
        };


        context["GroupObserver"] = () => {

            it["unbalanced goups"] = () => printErrorMessage(() => {
                var g1 = new Group(Matcher.AllOf(CID.ComponentA));
                var g2 = new Group(Matcher.AllOf(CID.ComponentB));
                var e1 = GroupEventType.OnEntityAdded;

                new GroupObserver(new [] { g1, g2 }, new [] { e1 });
            });
        };

        context["Pool"] = () => {

            it["wrong PoolMetaData componentNames count"] = () => printErrorMessage(() => {
                var componentNames = new [] { "Health", "Position", "View" };
                var metaData = new PoolMetaData("My Pool", componentNames);
                new Pool(1, 0, metaData);
            });

            it["destroy entity which is not in pool"] = () => printErrorMessage(() => {
                pool.DestroyEntity(new Entity(0, null));
            });

            it["destroy retained entities"] = () => printErrorMessage(() => {
                pool.CreateEntity().Retain(this);
                pool.DestroyAllEntities();
            });

            it["releases entity before destroy"] = () => printErrorMessage(() => {
                entity.Release(pool);
            });
        };

        context["CollectionExtension"] = () => {
            
            it["get single entity when more than one exist"] = () => printErrorMessage(() => {
                new Entity[2].SingleEntity();
            });
        };
    }
}

