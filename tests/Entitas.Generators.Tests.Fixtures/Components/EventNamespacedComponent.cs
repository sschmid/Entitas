using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext))]
    [Event(EventTarget.Any)]
    public sealed class EventNamespacedComponent : IComponent
    {
        public string Value;
    }
}
