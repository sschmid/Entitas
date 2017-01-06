using System;
using Entitas;
using Entitas.Serialization.Blueprints;
using NSpec;

class describe_EntitasErrorMessages : EntitasTest {

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

        before = () => {
            var componentNames = new [] { "Health", "Position", "View" };
            var contextInfo = new ContextInfo("My Pool", componentNames, null);
            _context = new Context(componentNames.Length, 42, contextInfo);
            _entity = createEntity();
        };

        it["creates exception with hint separated by newLine"] = () => {

            // given
            const string msg = "Message";
            const string hint = "Hint";
            var ex = new EntitasException(msg, hint);

            // then
            ex.Message.should_be(msg + "\n" + hint);
        };

        it["ignores hint when null"] = () => {

            // given
            const string msg = "Message";
            string hint = null;
            var ex = new EntitasException(msg, hint);

            // then
            ex.Message.should_be(msg);
        };

        context["Entity"] = () => {

            context["when not enabled"] = () => {

                before = () => {
                    _context.DestroyEntity(_entity);
                };

                it["add a component"] = () => printErrorMessage(() => _entity.AddComponentA());
                it["remove a component"] = () => printErrorMessage(() => _entity.RemoveComponentA());
                it["replace a component"] = () => printErrorMessage(() => _entity.ReplaceComponentA(Component.A));
            };

            context["when enabled"] = () => {

                it["add a component twice"] = () => printErrorMessage(() => {
                    _entity.AddComponentA();
                    _entity.AddComponentA();
                });

                it["remove a component that doesn't exist"] = () => printErrorMessage(() => {
                    _entity.RemoveComponentA();
                });

                it["get a component that doesn't exist"] = () => printErrorMessage(() => {
                    _entity.GetComponentA();
                });

                it["retain an entity twice"] = () => printErrorMessage(() => {
                    var owner = new object();
                    _entity.Retain(owner);
                    _entity.Retain(owner);
                });

                it["release an entity with wrong owner"] = () => printErrorMessage(() => {
                    var owner = new object();
                    _entity.Release(owner);
                });
            };
        };

        context["Group"] = () => {

            it["get single entity when multiple exist"] = () => printErrorMessage(() => {
                createEntityA();
                createEntityA();
                var matcher = createMatcherA();
                matcher.componentNames = _context.contextInfo.componentNames;
                var group = _context.GetGroup(matcher);
                group.GetSingleEntity();
            });
        };

        context["EntityCollector"] = () => {

            it["unbalanced goups"] = () => printErrorMessage(() => {
                var g1 = new Group(Matcher.AllOf(CID.ComponentA));
                var g2 = new Group(Matcher.AllOf(CID.ComponentB));
                var e1 = GroupEventType.OnEntityAdded;

                new EntityCollector(new [] { g1, g2 }, new [] { e1 });
            });
        };

        context["Pool"] = () => {

            it["wrong ContextInfo componentNames count"] = () => printErrorMessage(() => {
                var componentNames = new [] { "Health", "Position", "View" };
                var contextInfo = new ContextInfo("My Pool", componentNames, null);
                new Context(1, 0, contextInfo);
            });

            it["destroy entity which is not in pool"] = () => printErrorMessage(() => {
                _context.DestroyEntity(new Entity(0, null));
            });

            it["destroy retained entities"] = () => printErrorMessage(() => {
                createEntity().Retain(this);
                _context.DestroyAllEntities();
            });

            it["releases entity before destroy"] = () => printErrorMessage(() => {
                _entity.Release(_context);
            });

            it["unknown entityIndex"] = () => printErrorMessage(() => {
                _context.GetEntityIndex("unknown");
            });

            it["duplicate entityIndex"] = () => printErrorMessage(() => {
                var index = new PrimaryEntityIndex<string>(getGroupA(), null);
                _context.AddEntityIndex("duplicate", index);
                _context.AddEntityIndex("duplicate", index);
            });
        };

        context["CollectionExtension"] = () => {

            it["get single entity when more than one exist"] = () => printErrorMessage(() => {
                new Entity[2].SingleEntity();
            });
        };

        context["ComponentBlueprint"] = () => {

            it["type doesn't implement IComponent"] = () => printErrorMessage(() => {
                var componentBlueprint = new ComponentBlueprint();
                componentBlueprint.fullTypeName = "string";
                componentBlueprint.CreateComponent(_entity);
            });

            it["type doesn't exist"] = () => printErrorMessage(() => {
                var componentBlueprint = new ComponentBlueprint();
                componentBlueprint.fullTypeName = "UnknownType";
                componentBlueprint.CreateComponent(_entity);
            });

            it["invalid field name"] = () => printErrorMessage(() => {
                var componentBlueprint = new ComponentBlueprint();
                componentBlueprint.index = 0;
                componentBlueprint.fullTypeName = typeof(NameAgeComponent).FullName;
                componentBlueprint.members = new [] {
                    new SerializableMember("xxx", "publicFieldValue"),
                    new SerializableMember("publicProperty", "publicPropertyValue")
                };
                componentBlueprint.CreateComponent(_entity);
            });
        };

        context["EntityIndex"] = () => {

            it["no entity with key"] = () => printErrorMessage(() => {
                createPrimaryIndex().GetEntity("unknownKey");
            });

            it["multiple entities for primary key"] = () => printErrorMessage(() => {
                createPrimaryIndex();
                var nameAge = createNameAge();
                _context.CreateEntity().AddComponent(CID.ComponentA, nameAge);
                _context.CreateEntity().AddComponent(CID.ComponentA, nameAge);
            });
        };
    }
}
