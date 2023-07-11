using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext)), Context(typeof(OtherContext))]
    [Unique]
    [Event(EventTarget.Any)]
    [Event(EventTarget.Any, EventType.Removed)]
    public sealed class ScoreComponent : IComponent
    {
        public int Value;
    }
}
