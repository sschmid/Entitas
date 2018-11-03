using NSpec;

class describe_MultiReactiveSystem : nspec {

    void when_executing() {

        context["when triggered"] = () => {

            MultiReactiveSystemSpy system = null;
            TestEntity e1 = null;
            Test2Entity e2 = null;

            before = () => {
                var contexts = new Contexts();
                system = new MultiReactiveSystemSpy(contexts);
                system.executeAction = entities => {
                    foreach (var e in entities) {
                        e.nameAge.age += 10;
                    }
                };

                e1 = contexts.test.CreateEntity();
                e1.AddNameAge("Max", 42);

                e2 = contexts.test2.CreateEntity();
                e2.AddNameAge("Jack", 24);

                system.Execute();
            };

            it["processes entities from different contexts"] = () => {
                system.entities.Length.should_be(2);
                system.entities.should_contain(e1);
                system.entities.should_contain(e2);

                e1.nameAge.age.should_be(52);
                e2.nameAge.age.should_be(34);
            };

            it["executes once"] = () => {
                system.didExecute.should_be(1);
            };

            it["retains once even when multiple collectors contain entity"] = () => {
                system.didExecute.should_be(1);
            };

            it["can ToString"] = () => {
                system.ToString().should_be("MultiReactiveSystem(MultiReactiveSystemSpy)");
            };
        };



        context["when multiple collectors are triggered with same entity"] = () => {

            MultiTriggeredMultiReactiveSystemSpy system = null;
            TestEntity e1 = null;

            before = () => {
                var contexts = new Contexts();
                system = new MultiTriggeredMultiReactiveSystemSpy(contexts);
                e1 = contexts.test.CreateEntity();
                e1.AddNameAge("Max", 42);

                e1.RemoveNameAge();

                system.Execute();
            };

            it["executes once"] = () => {
                system.didExecute.should_be(1);
            };

            it["merges collected entities and removes duplicates"] = () => {
                system.entities.Length.should_be(1);
            };

            it["clears merged collected entities"] = () => {
                system.Execute();
                system.didExecute.should_be(1);
            };
        };
    }
}
