using System;

namespace Entitas.Generators.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class EventAttribute : Attribute
    {
        public readonly EventTarget EventTarget;
        public readonly EventType EventType;
        public readonly int Order;

        public EventAttribute(EventTarget eventTarget, EventType eventType = EventType.Added, int order = 0)
        {
            EventTarget = eventTarget;
            EventType = eventType;
            Order = order;
        }
    }

    public enum EventTarget
    {
        Any,
        Self
    }

    public enum EventType
    {
        Added,
        Removed
    }
}
