//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class Test1Entity {

    public My.Namespace.MultipleEntityIndexesComponent myNamespaceMultipleEntityIndexes { get { return (My.Namespace.MultipleEntityIndexesComponent)GetComponent(Test1ComponentsLookup.MyNamespaceMultipleEntityIndexes); } }
    public bool hasMyNamespaceMultipleEntityIndexes { get { return HasComponent(Test1ComponentsLookup.MyNamespaceMultipleEntityIndexes); } }

    public void AddMyNamespaceMultipleEntityIndexes(string newValue, string newValue2) {
        var index = Test1ComponentsLookup.MyNamespaceMultipleEntityIndexes;
        var component = (My.Namespace.MultipleEntityIndexesComponent)CreateComponent(index, typeof(My.Namespace.MultipleEntityIndexesComponent));
        component.value = newValue;
        component.value2 = newValue2;
        AddComponent(index, component);
    }

    public void ReplaceMyNamespaceMultipleEntityIndexes(string newValue, string newValue2) {
        var index = Test1ComponentsLookup.MyNamespaceMultipleEntityIndexes;
        var component = (My.Namespace.MultipleEntityIndexesComponent)CreateComponent(index, typeof(My.Namespace.MultipleEntityIndexesComponent));
        component.value = newValue;
        component.value2 = newValue2;
        ReplaceComponent(index, component);
    }

    public void RemoveMyNamespaceMultipleEntityIndexes() {
        RemoveComponent(Test1ComponentsLookup.MyNamespaceMultipleEntityIndexes);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentEntityApiInterfaceGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class Test1Entity : IMyNamespaceMultipleEntityIndexesEntity { }

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class Test1Matcher {

    static Entitas.IMatcher<Test1Entity> _matcherMyNamespaceMultipleEntityIndexes;

    public static Entitas.IMatcher<Test1Entity> MyNamespaceMultipleEntityIndexes {
        get {
            if (_matcherMyNamespaceMultipleEntityIndexes == null) {
                var matcher = (Entitas.Matcher<Test1Entity>)Entitas.Matcher<Test1Entity>.AllOf(Test1ComponentsLookup.MyNamespaceMultipleEntityIndexes);
                matcher.componentNames = Test1ComponentsLookup.componentNames;
                _matcherMyNamespaceMultipleEntityIndexes = matcher;
            }

            return _matcherMyNamespaceMultipleEntityIndexes;
        }
    }
}
