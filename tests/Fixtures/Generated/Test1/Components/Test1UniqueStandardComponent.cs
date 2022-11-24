//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.Plugins.ComponentContextApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class Test1Context {

    public Test1Entity uniqueStandardEntity { get { return GetGroup(Test1Matcher.UniqueStandard).GetSingleEntity(); } }
    public UniqueStandardComponent uniqueStandard { get { return uniqueStandardEntity.uniqueStandard; } }
    public bool hasUniqueStandard { get { return uniqueStandardEntity != null; } }

    public Test1Entity SetUniqueStandard(string newValue) {
        if (hasUniqueStandard) {
            throw new Entitas.EntitasException("Could not set UniqueStandard!\n" + this + " already has an entity with UniqueStandardComponent!",
                "You should check if the context already has a uniqueStandardEntity before setting it or use context.ReplaceUniqueStandard().");
        }
        var entity = CreateEntity();
        entity.AddUniqueStandard(newValue);
        return entity;
    }

    public void ReplaceUniqueStandard(string newValue) {
        var entity = uniqueStandardEntity;
        if (entity == null) {
            entity = SetUniqueStandard(newValue);
        } else {
            entity.ReplaceUniqueStandard(newValue);
        }
    }

    public void RemoveUniqueStandard() {
        uniqueStandardEntity.Destroy();
    }
}

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.Plugins.ComponentEntityApiGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public partial class Test1Entity {

    public UniqueStandardComponent uniqueStandard { get { return (UniqueStandardComponent)GetComponent(Test1ComponentsLookup.UniqueStandard); } }
    public bool hasUniqueStandard { get { return HasComponent(Test1ComponentsLookup.UniqueStandard); } }

    public void AddUniqueStandard(string newValue) {
        var index = Test1ComponentsLookup.UniqueStandard;
        var component = (UniqueStandardComponent)CreateComponent(index, typeof(UniqueStandardComponent));
        component.Value = newValue;
        AddComponent(index, component);
    }

    public void ReplaceUniqueStandard(string newValue) {
        var index = Test1ComponentsLookup.UniqueStandard;
        var component = (UniqueStandardComponent)CreateComponent(index, typeof(UniqueStandardComponent));
        component.Value = newValue;
        ReplaceComponent(index, component);
    }

    public void RemoveUniqueStandard() {
        RemoveComponent(Test1ComponentsLookup.UniqueStandard);
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

    static Entitas.IMatcher<Test1Entity> _matcherUniqueStandard;

    public static Entitas.IMatcher<Test1Entity> UniqueStandard {
        get {
            if (_matcherUniqueStandard == null) {
                var matcher = (Entitas.Matcher<Test1Entity>)Entitas.Matcher<Test1Entity>.AllOf(Test1ComponentsLookup.UniqueStandard);
                matcher.ComponentNames = Test1ComponentsLookup.componentNames;
                _matcherUniqueStandard = matcher;
            }

            return _matcherUniqueStandard;
        }
    }
}
