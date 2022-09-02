using System;
using System.Linq;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class EventComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            var attrs = Attribute.GetCustomAttributes(type)
                .OfType<EventAttribute>()
                .ToArray();

            if (attrs.Length > 0) {
                data.IsEvent(true);
                var eventData = attrs
                    .Select(attr => new EventData(attr.eventTarget, attr.eventType, attr.priority))
                    .ToArray();

                data.SetEventData(eventData);
            } else {
                data.IsEvent(false);
            }
        }
    }

    public static class EventComponentDataExtension {

        public const string COMPONENT_EVENT = "Component.Event";
        public const string COMPONENT_EVENT_DATA = "Component.Event.Data";

        public static bool IsEvent(this ComponentData data) {
            return (bool)data[COMPONENT_EVENT];
        }

        public static void IsEvent(this ComponentData data, bool isEvent) {
            data[COMPONENT_EVENT] = isEvent;
        }

        public static EventData[] GetEventData(this ComponentData data) {
            return (EventData[])data[COMPONENT_EVENT_DATA];
        }

        public static void SetEventData(this ComponentData data, EventData[] eventData) {
            data[COMPONENT_EVENT_DATA] = eventData;
        }
    }
}
