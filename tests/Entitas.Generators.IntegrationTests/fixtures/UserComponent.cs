#nullable disable

using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyFeature
{
    [Context(typeof(MainContext)), Context(typeof(OtherContext))]
    [Unique]
    [Event(EventTarget.Any, EventType.Added, 1)]
    [Event(EventTarget.Any, EventType.Removed, 2)]
    [Event(EventTarget.Self, EventType.Added, 3)]
    [Event(EventTarget.Self, EventType.Removed, 4)]
    public sealed class UserComponent : IComponent
    {
        [EntityIndex(true)]
        public string Name;

        [EntityIndex(false)]
        public int Age;
    }
}
