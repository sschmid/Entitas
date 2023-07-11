using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext))]
    [Event(EventTarget.Any, EventType.Added, 1)]
    public sealed class AnyEventNamespacedComponent : IComponent
    {
        public string Value;
    }
}
