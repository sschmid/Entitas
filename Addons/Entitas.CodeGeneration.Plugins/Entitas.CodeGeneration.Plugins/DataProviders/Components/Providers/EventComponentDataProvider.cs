using System;
using System.Linq;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class EventComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            data.SetEventData(getEventData(type));
        }

        EventData getEventData(Type type) {
            var attr = Attribute.GetCustomAttributes(type)
                .OfType<EventAttribute>()
                .SingleOrDefault();

            if (attr != null) {
                return new EventData(attr.bindToEntity, attr.priority);
            }

            return null;
        }
    }

    public static class EventComponentDataDataExtension {

        public const string COMPONENT_EVENT = "Component.Event";

        public static EventData GetEventData(this ComponentData data) {
            return (EventData)data[COMPONENT_EVENT];
        }

        public static void SetEventData(this ComponentData data, EventData eventData) {
            data[COMPONENT_EVENT] = eventData;
        }
    }
}
