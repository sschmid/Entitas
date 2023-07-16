using System.Collections.Generic;

namespace Entitas.Generators
{
    public struct EventDeclaration
    {
        public readonly int EventTarget;
        public readonly int EventType;
        public readonly int Order;

        public readonly string EventTargetPrefix;
        public readonly string EventTypeSuffix;
        public readonly string EventPrefix;
        public readonly string EventListener;
        public readonly string EventMethod;

        public string ContextAwareEvent = null!;
        public string ContextAwareEventListener = null!;
        public string ContextAwareEventListenerInterface = null!;
        public string ContextAwareEventListenerComponent = null!;

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
    }

    public class EventTargetAndEventTypeComparer : IEqualityComparer<EventDeclaration>
    {
        public bool Equals(EventDeclaration x, EventDeclaration y) =>
            x.EventTarget == y.EventTarget &&
            x.EventType == y.EventType;

        public int GetHashCode(EventDeclaration obj)
        {
            unchecked
            {
                var hashCode = obj.EventTarget.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.EventType.GetHashCode();
                return hashCode;
            }
        }
    }

    public class EventTargetAndEventTypeAndOrderComparer : IEqualityComparer<EventDeclaration>
    {
        public bool Equals(EventDeclaration x, EventDeclaration y) =>
            x.EventTarget == y.EventTarget &&
            x.EventType == y.EventType &&
            x.Order == y.Order;

        public int GetHashCode(EventDeclaration obj)
        {
            unchecked
            {
                var hashCode = obj.EventTarget.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.EventType.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Order.GetHashCode();
                return hashCode;
            }
        }
    }
}
