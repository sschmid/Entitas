//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class Test1Entity {

    public Test1AnyMultipleEventsStandardEventListenerComponent test1AnyMultipleEventsStandardEventListener { get { return (Test1AnyMultipleEventsStandardEventListenerComponent)GetComponent(Test1ComponentsLookup.Test1AnyMultipleEventsStandardEventListener); } }
    public bool hasTest1AnyMultipleEventsStandardEventListener { get { return HasComponent(Test1ComponentsLookup.Test1AnyMultipleEventsStandardEventListener); } }

    public void AddTest1AnyMultipleEventsStandardEventListener(System.Collections.Generic.List<ITest1AnyMultipleEventsStandardEventListener> newValue) {
        var index = Test1ComponentsLookup.Test1AnyMultipleEventsStandardEventListener;
        var component = (Test1AnyMultipleEventsStandardEventListenerComponent)CreateComponent(index, typeof(Test1AnyMultipleEventsStandardEventListenerComponent));
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceTest1AnyMultipleEventsStandardEventListener(System.Collections.Generic.List<ITest1AnyMultipleEventsStandardEventListener> newValue) {
        var index = Test1ComponentsLookup.Test1AnyMultipleEventsStandardEventListener;
        var component = (Test1AnyMultipleEventsStandardEventListenerComponent)CreateComponent(index, typeof(Test1AnyMultipleEventsStandardEventListenerComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveTest1AnyMultipleEventsStandardEventListener() {
        RemoveComponent(Test1ComponentsLookup.Test1AnyMultipleEventsStandardEventListener);
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
public sealed partial class Test1Matcher {

    static Entitas.IMatcher<Test1Entity> _matcherTest1AnyMultipleEventsStandardEventListener;

    public static Entitas.IMatcher<Test1Entity> Test1AnyMultipleEventsStandardEventListener {
        get {
            if (_matcherTest1AnyMultipleEventsStandardEventListener == null) {
                var matcher = (Entitas.Matcher<Test1Entity>)Entitas.Matcher<Test1Entity>.AllOf(Test1ComponentsLookup.Test1AnyMultipleEventsStandardEventListener);
                matcher.ComponentNames = Test1ComponentsLookup.componentNames;
                _matcherTest1AnyMultipleEventsStandardEventListener = matcher;
            }

            return _matcherTest1AnyMultipleEventsStandardEventListener;
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
public partial class Test1Entity {

    public void AddTest1AnyMultipleEventsStandardEventListener(ITest1AnyMultipleEventsStandardEventListener value) {
        var listeners = hasTest1AnyMultipleEventsStandardEventListener
            ? test1AnyMultipleEventsStandardEventListener.Value
            : new System.Collections.Generic.List<ITest1AnyMultipleEventsStandardEventListener>();
        listeners.Add(value);
        ReplaceTest1AnyMultipleEventsStandardEventListener(listeners);
    }

    public void RemoveTest1AnyMultipleEventsStandardEventListener(ITest1AnyMultipleEventsStandardEventListener value, bool removeComponentWhenEmpty = true) {
        var listeners = test1AnyMultipleEventsStandardEventListener.Value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0) {
            RemoveTest1AnyMultipleEventsStandardEventListener();
        } else {
            ReplaceTest1AnyMultipleEventsStandardEventListener(listeners);
        }
    }
}
