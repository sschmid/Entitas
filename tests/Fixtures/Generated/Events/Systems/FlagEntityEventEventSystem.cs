//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.Plugins.EventSystemGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed class FlagEntityEventEventSystem : Entitas.ReactiveSystem<Test1Entity> {

    readonly System.Collections.Generic.List<IFlagEntityEventListener> _listenerBuffer;

    public FlagEntityEventEventSystem(Contexts contexts) : base(contexts.test1) {
        _listenerBuffer = new System.Collections.Generic.List<IFlagEntityEventListener>();
    }

    protected override Entitas.ICollector<Test1Entity> GetTrigger(Entitas.IContext<Test1Entity> context) {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Added(Test1Matcher.FlagEntityEvent)
        );
    }

    protected override bool Filter(Test1Entity entity) {
        return entity.isFlagEntityEvent && entity.hasFlagEntityEventListener;
    }

    protected override void Execute(System.Collections.Generic.List<Test1Entity> entities) {
        foreach (var e in entities) {
            
            _listenerBuffer.Clear();
            _listenerBuffer.AddRange(e.flagEntityEventListener.Value);
            foreach (var listener in _listenerBuffer) {
                listener.OnFlagEntityEvent(e);
            }
        }
    }
}
