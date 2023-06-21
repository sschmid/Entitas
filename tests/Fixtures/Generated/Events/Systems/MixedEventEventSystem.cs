//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.CodeGeneration.Plugins.EventSystemGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed class MixedEventEventSystem : Entitas.ReactiveSystem<Test1Entity> {

    readonly System.Collections.Generic.List<IMixedEventListener> _listenerBuffer;

    public MixedEventEventSystem(Contexts contexts) : base(contexts.test1) {
        _listenerBuffer = new System.Collections.Generic.List<IMixedEventListener>();
    }

    protected override Entitas.ICollector<Test1Entity> GetTrigger(Entitas.IContext<Test1Entity> context) {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Added(Test1Matcher.MixedEvent)
        );
    }

    protected override bool Filter(Test1Entity entity) {
        return entity.hasMixedEvent && entity.hasMixedEventListener;
    }

    protected override void Execute(System.Collections.Generic.List<Test1Entity> entities) {
        foreach (var e in entities) {
            var component = e.mixedEvent;
            _listenerBuffer.Clear();
            _listenerBuffer.AddRange(e.mixedEventListener.value);
            foreach (var listener in _listenerBuffer) {
                listener.OnMixedEvent(e, component.value);
            }
        }
    }
}