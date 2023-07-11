using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext))]
    [Event(EventTarget.Self, EventType.Removed, 1)]
    public sealed class SelfEventNamespacedComponent : IComponent
    {
        public string Value;
    }
}
