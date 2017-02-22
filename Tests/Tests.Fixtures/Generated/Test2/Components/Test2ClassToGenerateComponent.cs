public partial class Test2Entity {

    public ClassToGenerateComponent classToGenerate { get { return (ClassToGenerateComponent)GetComponent(Test2ComponentsLookup.ClassToGenerate); } }
    public bool hasClassToGenerate { get { return HasComponent(Test2ComponentsLookup.ClassToGenerate); } }

    public void AddClassToGenerate(My.Namespace.ClassToGenerate newValue) {
        var component = CreateComponent<ClassToGenerateComponent>(Test2ComponentsLookup.ClassToGenerate);
        component.value = newValue;
        AddComponent(Test2ComponentsLookup.ClassToGenerate, component);
    }

    public void ReplaceClassToGenerate(My.Namespace.ClassToGenerate newValue) {
        var component = CreateComponent<ClassToGenerateComponent>(Test2ComponentsLookup.ClassToGenerate);
        component.value = newValue;
        ReplaceComponent(Test2ComponentsLookup.ClassToGenerate, component);
    }

    public void RemoveClassToGenerate() {
        RemoveComponent(Test2ComponentsLookup.ClassToGenerate);
    }
}

public sealed partial class Test2Matcher {

    static Entitas.IMatcher<Test2Entity> _matcherClassToGenerate;

    public static Entitas.IMatcher<Test2Entity> ClassToGenerate {
        get {
            if(_matcherClassToGenerate == null) {
                var matcher = (Entitas.Matcher<Test2Entity>)Entitas.Matcher<Test2Entity>.AllOf(Test2ComponentsLookup.ClassToGenerate);
                matcher.componentNames = Test2ComponentsLookup.componentNames;
                _matcherClassToGenerate = matcher;
            }

            return _matcherClassToGenerate;
        }
    }
}
