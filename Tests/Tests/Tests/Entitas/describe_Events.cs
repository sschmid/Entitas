using NSpec;

class describe_Events : nspec {

    void when_events() {

        Contexts contexts = null;

        before = () => { contexts = new Contexts(); };

        context["event"] = () => {

            AnyStandardEventEventSystem eventSystem = null;

            before = () => { eventSystem = new AnyStandardEventEventSystem(contexts); };

            it["can remove listener in callback"] = () => {
                var eventTest = new RemoveEventTest(contexts, false);

                contexts.test.CreateEntity().AddStandardEvent("Test");
                eventSystem.Execute();

                eventTest.value.should_be("Test");
            };

            it["can remove listener in callback in the middle"] = () => {
                var eventTest1 = new RemoveEventTest(contexts, false);
                var eventTest2 = new RemoveEventTest(contexts, false);
                var eventTest3 = new RemoveEventTest(contexts, false);

                contexts.test.CreateEntity().AddStandardEvent("Test");
                eventSystem.Execute();

                eventTest1.value.should_be("Test");
                eventTest2.value.should_be("Test");
                eventTest3.value.should_be("Test");
            };

            it["can remove listener in callback and remove component"] = () => {
                var eventTest = new RemoveEventTest(contexts, true);

                contexts.test.CreateEntity().AddStandardEvent("Test");
                eventSystem.Execute();

                eventTest.value.should_be("Test");
            };
        };

        context["entity event"] = () => {

            FlagEntityEventEventSystem eventSystem = null;

            before = () => { eventSystem = new FlagEntityEventEventSystem(contexts); };

            it["can remove listener in callback"] = () => {
                var eventTest = new RemoveEventTest(contexts, false);

                eventTest.listener.isFlagEntityEvent = true;
                eventSystem.Execute();

                eventTest.value.should_be("true");
            };

            it["can remove listener in callback and remove component"] = () => {
                var eventTest = new RemoveEventTest(contexts, true);

                eventTest.listener.isFlagEntityEvent = true;
                eventSystem.Execute();

                eventTest.value.should_be("true");
            };
        };
    }
}

class RemoveEventTest : IAnyStandardEventListener, IFlagEntityEventListener {

    public TestEntity listener => _listener;
    public string value => _value;

    readonly bool _removeComponentWhenEmpty;
    readonly TestEntity _listener;

    string _value;

    public RemoveEventTest(Contexts contexts, bool removeComponentWhenEmpty) {
        _removeComponentWhenEmpty = removeComponentWhenEmpty;
        _listener = contexts.test.CreateEntity();
        _listener.AddAnyStandardEventListener(this);
        _listener.AddFlagEntityEventListener(this);
    }

    public void OnAnyStandardEvent(TestEntity entity, string value) {
        _listener.RemoveAnyStandardEventListener(this, _removeComponentWhenEmpty);
        _value = value;
    }

    public void OnFlagEntityEvent(TestEntity entity) {
        _listener.RemoveFlagEntityEventListener(this, _removeComponentWhenEmpty);
        _value = "true";
    }
}
