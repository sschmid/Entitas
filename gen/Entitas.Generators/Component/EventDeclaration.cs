using System;

namespace Entitas.Generators
{
    readonly struct EventDeclaration : IEquatable<EventDeclaration>
    {
        public readonly int EventTarget;
        public readonly int EventType;
        public readonly int Order;

        public EventDeclaration(int eventTarget, int eventType, int order = 0)
        {
            EventTarget = eventTarget;
            EventType = eventType;
            Order = order;
        }

        public bool Equals(EventDeclaration other) =>
            EventTarget == other.EventTarget &&
            EventType == other.EventType &&
            Order == other.Order;

        public override bool Equals(object? obj) => obj is EventDeclaration other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = EventTarget;
                hashCode = (hashCode * 397) ^ EventType;
                hashCode = (hashCode * 397) ^ Order;
                return hashCode;
            }
        }
    }
}
