public partial class TestEntity {

    public Test2ContextComponent test2Context { get { return (Test2ContextComponent)GetComponent(TestComponentsLookup.Test2Context); } }
    public bool hasTest2Context { get { return HasComponent(TestComponentsLookup.Test2Context); } }

    public void AddTest2Context(string newValue) {
        var component = CreateComponent<Test2ContextComponent>(TestComponentsLookup.Test2Context);
        component.value = newValue;
        AddComponent(TestComponentsLookup.Test2Context, component);
    }

    public void ReplaceTest2Context(string newValue) {
        var component = CreateComponent<Test2ContextComponent>(TestComponentsLookup.Test2Context);
        component.value = newValue;
        ReplaceComponent(TestComponentsLookup.Test2Context, component);
    }

    public void RemoveTest2Context() {
        RemoveComponent(TestComponentsLookup.Test2Context);
    }
}

public sealed partial class TestMatcher {

    static Entitas.IMatcher<TestEntity> _matcherTest2Context;

    public static Entitas.IMatcher<TestEntity> Test2Context {
        get {
            if(_matcherTest2Context == null) {
                var matcher = (Entitas.Matcher<TestEntity>)Entitas.Matcher<TestEntity>.AllOf(TestComponentsLookup.Test2Context);
                matcher.componentNames = TestComponentsLookup.componentNames;
                _matcherTest2Context = matcher;
            }

            return _matcherTest2Context;
        }
    }
}
