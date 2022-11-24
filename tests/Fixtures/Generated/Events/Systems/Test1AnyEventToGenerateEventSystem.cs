//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by Entitas.Plugins.EventSystemGenerator.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
public sealed class Test1AnyEventToGenerateEventSystem : Entitas.ReactiveSystem<Test1Entity> {

    readonly Entitas.IGroup<Test1Entity> _listeners;
    readonly System.Collections.Generic.List<Test1Entity> _entityBuffer;
    readonly System.Collections.Generic.List<ITest1AnyEventToGenerateListener> _listenerBuffer;

    public Test1AnyEventToGenerateEventSystem(Contexts contexts) : base(contexts.test1) {
        _listeners = contexts.test1.GetGroup(Test1Matcher.Test1AnyEventToGenerateListener);
        _entityBuffer = new System.Collections.Generic.List<Test1Entity>();
        _listenerBuffer = new System.Collections.Generic.List<ITest1AnyEventToGenerateListener>();
    }

    protected override Entitas.ICollector<Test1Entity> GetTrigger(Entitas.IContext<Test1Entity> context) {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.Added(Test1Matcher.EventToGenerate)
        );
    }

    protected override bool Filter(Test1Entity entity) {
        return entity.hasEventToGenerate;
    }

    protected override void Execute(System.Collections.Generic.List<Test1Entity> entities) {
        foreach (var e in entities) {
            var component = e.eventToGenerate;
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer)) {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.test1AnyEventToGenerateListener.Value);
                foreach (var listener in _listenerBuffer) {
                    listener.OnAnyEventToGenerate(e, component.Value);
                }
            }
        }
    }
}
