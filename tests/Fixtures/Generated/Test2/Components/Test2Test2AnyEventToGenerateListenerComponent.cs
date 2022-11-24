//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class Test2Entity {

    public Test2AnyEventToGenerateListenerComponent test2AnyEventToGenerateListener { get { return (Test2AnyEventToGenerateListenerComponent)GetComponent(Test2ComponentsLookup.Test2AnyEventToGenerateListener); } }
    public bool hasTest2AnyEventToGenerateListener { get { return HasComponent(Test2ComponentsLookup.Test2AnyEventToGenerateListener); } }

    public void AddTest2AnyEventToGenerateListener(System.Collections.Generic.List<ITest2AnyEventToGenerateListener> newValue) {
        var index = Test2ComponentsLookup.Test2AnyEventToGenerateListener;
        var component = (Test2AnyEventToGenerateListenerComponent)CreateComponent(index, typeof(Test2AnyEventToGenerateListenerComponent));
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceTest2AnyEventToGenerateListener(System.Collections.Generic.List<ITest2AnyEventToGenerateListener> newValue) {
        var index = Test2ComponentsLookup.Test2AnyEventToGenerateListener;
        var component = (Test2AnyEventToGenerateListenerComponent)CreateComponent(index, typeof(Test2AnyEventToGenerateListenerComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveTest2AnyEventToGenerateListener() {
        RemoveComponent(Test2ComponentsLookup.Test2AnyEventToGenerateListener);
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.Plugins.ComponentMatcherApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed partial class Test2Matcher {

    static Entitas.IMatcher<Test2Entity> _matcherTest2AnyEventToGenerateListener;

    public static Entitas.IMatcher<Test2Entity> Test2AnyEventToGenerateListener {
        get {
            if (_matcherTest2AnyEventToGenerateListener == null) {
                var matcher = (Entitas.Matcher<Test2Entity>)Entitas.Matcher<Test2Entity>.AllOf(Test2ComponentsLookup.Test2AnyEventToGenerateListener);
                matcher.ComponentNames = Test2ComponentsLookup.componentNames;
                _matcherTest2AnyEventToGenerateListener = matcher;
            }

            return _matcherTest2AnyEventToGenerateListener;
        }
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.Plugins.EventEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class Test2Entity {

    public void AddTest2AnyEventToGenerateListener(ITest2AnyEventToGenerateListener value) {
        var listeners = hasTest2AnyEventToGenerateListener
            ? test2AnyEventToGenerateListener.Value
            : new System.Collections.Generic.List<ITest2AnyEventToGenerateListener>();
        listeners.Add(value);
        ReplaceTest2AnyEventToGenerateListener(listeners);
    }

    public void RemoveTest2AnyEventToGenerateListener(ITest2AnyEventToGenerateListener value, bool removeComponentWhenEmpty = true) {
        var listeners = test2AnyEventToGenerateListener.Value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0) {
            RemoveTest2AnyEventToGenerateListener();
        } else {
            ReplaceTest2AnyEventToGenerateListener(listeners);
        }
    }
}
