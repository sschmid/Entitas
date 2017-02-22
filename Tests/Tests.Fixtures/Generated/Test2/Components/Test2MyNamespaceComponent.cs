public partial class Test2Entity {

    public My.Namespace.MyNamespaceComponent myNamespace { get { return (My.Namespace.MyNamespaceComponent)GetComponent(Test2ComponentsLookup.MyNamespace); } }
    public bool hasMyNamespace { get { return HasComponent(Test2ComponentsLookup.MyNamespace); } }

    public void AddMyNamespace(string newValue) {
        var component = CreateComponent<My.Namespace.MyNamespaceComponent>(Test2ComponentsLookup.MyNamespace);
        component.value = newValue;
        AddComponent(Test2ComponentsLookup.MyNamespace, component);
    }

    public void ReplaceMyNamespace(string newValue) {
        var component = CreateComponent<My.Namespace.MyNamespaceComponent>(Test2ComponentsLookup.MyNamespace);
        component.value = newValue;
        ReplaceComponent(Test2ComponentsLookup.MyNamespace, component);
    }

    public void RemoveMyNamespace() {
        RemoveComponent(Test2ComponentsLookup.MyNamespace);
    }
}

public sealed partial class Test2Matcher {

    static Entitas.IMatcher<Test2Entity> _matcherMyNamespace;

    public static Entitas.IMatcher<Test2Entity> MyNamespace {
        get {
            if(_matcherMyNamespace == null) {
                var matcher = (Entitas.Matcher<Test2Entity>)Entitas.Matcher<Test2Entity>.AllOf(Test2ComponentsLookup.MyNamespace);
                matcher.componentNames = Test2ComponentsLookup.componentNames;
                _matcherMyNamespace = matcher;
            }

            return _matcherMyNamespace;
        }
    }
}
