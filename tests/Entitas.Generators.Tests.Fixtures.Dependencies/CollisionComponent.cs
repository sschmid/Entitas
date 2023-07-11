using Entitas;
using Entitas.Generators.Attributes;
using MyApp;

namespace MyOtherFeature
{
    [Context(typeof(LibraryContext))]
    [Event(EventTarget.Self, EventType.Removed, 1)]
    public class CollisionComponent : IComponent
    {
        public int Value;
    }
}
