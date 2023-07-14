using System;

namespace Entitas.Generators
{
    struct EventDeclaration : IEquatable<EventDeclaration>
    {
        public readonly int EventTarget;
        public readonly int EventType;
        public readonly int Order;

        public readonly string EventTargetPrefix;
        public readonly string EventTypeSuffix;
        public readonly string EventPrefix;
        public readonly string EventListener;

        public string ContextAwareEvent;
        public string ContextAwareEventListener;
        public string EventMethod;
        public string ContextAwareEventListenerInterface;
        public string ContextAwareEventListenerComponent;

        public EventDeclaration(int eventTarget, int eventType, int order, string componentPrefix)
        {
            EventTarget = eventTarget;
            EventType = eventType;
            Order = order;

            EventTargetPrefix = eventTarget == 0 ? "Any" : string.Empty;
            EventTypeSuffix = eventType == 0 ? "Added" : "Removed";
            EventPrefix = $"{EventTargetPrefix}{componentPrefix}{EventTypeSuffix}"; // e.g. AnyPositionAdded
            EventListener = $"{EventPrefix}Listener"; // e.g. AnyPositionAddedListener
            EventMethod = $"On{EventPrefix}"; // e.g. OnAnyPositionAdded
        }

        public void ContextAware(string contextAware)
        {
            ContextAwareEvent = $"{contextAware}{EventPrefix}"; // e.g. MyAppMainAnyPositionAdded
            ContextAwareEventListener = $"{ContextAwareEvent}Listener"; // e.g. MyAppMainAnyPositionAddedListener
            ContextAwareEventListenerInterface = $"I{ContextAwareEventListener}"; // e.g. IMyAppMainAnyPositionAddedListener
            ContextAwareEventListenerComponent = $"{ContextAwareEventListener}Component"; // e.g. MyAppMainAnyPositionAddedListenerComponent
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
