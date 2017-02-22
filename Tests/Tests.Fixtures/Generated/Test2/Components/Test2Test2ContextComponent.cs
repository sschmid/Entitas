public partial class Test2Entity {

    public Test2ContextComponent test2Context { get { return (Test2ContextComponent)GetComponent(Test2ComponentsLookup.Test2Context); } }
    public bool hasTest2Context { get { return HasComponent(Test2ComponentsLookup.Test2Context); } }

    public void AddTest2Context(string newValue) {
        var component = CreateComponent<Test2ContextComponent>(Test2ComponentsLookup.Test2Context);
        component.value = newValue;
        AddComponent(Test2ComponentsLookup.Test2Context, component);
    }

    public void ReplaceTest2Context(string newValue) {
        var component = CreateComponent<Test2ContextComponent>(Test2ComponentsLookup.Test2Context);
        component.value = newValue;
        ReplaceComponent(Test2ComponentsLookup.Test2Context, component);
    }

    public void RemoveTest2Context() {
        RemoveComponent(Test2ComponentsLookup.Test2Context);
    }
}

public sealed partial class Test2Matcher {

    static Entitas.IMatcher<Test2Entity> _matcherTest2Context;

    public static Entitas.IMatcher<Test2Entity> Test2Context {
        get {
            if(_matcherTest2Context == null) {
                var matcher = (Entitas.Matcher<Test2Entity>)Entitas.Matcher<Test2Entity>.AllOf(Test2ComponentsLookup.Test2Context);
                matcher.componentNames = Test2ComponentsLookup.componentNames;
                _matcherTest2Context = matcher;
            }

            return _matcherTest2Context;
        }
    }
}
