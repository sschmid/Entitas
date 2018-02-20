using System;
using System.Linq;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class EventComponentDataProvider : IComponentDataProvider {

        public void Provide(Type type, ComponentData data) {
            var attr = Attribute.GetCustomAttributes(type)
                .OfType<EventAttribute>()
                .SingleOrDefault();

            if (attr != null) {
                data.IsEvent(true);
                data.SetEventBindToEntity(attr.bindToEntity);
                data.SetEventType(attr.eventType);
                data.SetEventPriority(attr.priority);
            } else {
                data.IsEvent(false);
            }
        }
    }

    public static class EventComponentDataExtension {

        public const string COMPONENT_EVENT = "Component.Event";
        public const string COMPONENT_EVENT_BIND_TO_ENTITY = "Component.Event.BindToEntity";
        public const string COMPONENT_EVENT_TYPE = "Component.Event.Type";
        public const string COMPONENT_EVENT_PRIORITY = "Component.Event.Priority";

        public static bool IsEvent(this ComponentData data) {
            return (bool)data[COMPONENT_EVENT];
        }

        public static void IsEvent(this ComponentData data, bool isEvent) {
            data[COMPONENT_EVENT] = isEvent;
        }

        public static bool GetEventBindToEntity(this ComponentData data) {
            return (bool)data[COMPONENT_EVENT_BIND_TO_ENTITY];
        }

        public static void SetEventBindToEntity(this ComponentData data, bool bindToEntity) {
            data[COMPONENT_EVENT_BIND_TO_ENTITY] = bindToEntity;
        }

        public static EventType GetEventType(this ComponentData data) {
            return (EventType)data[COMPONENT_EVENT_TYPE];
        }

        public static void SetEventType(this ComponentData data, EventType eventType) {
            data[COMPONENT_EVENT_TYPE] = eventType;
        }

        public static int GetEventPriority(this ComponentData data) {
            return (int)data[COMPONENT_EVENT_PRIORITY];
        }

        public static void SetEventPriority(this ComponentData data, int priority) {
            data[COMPONENT_EVENT_PRIORITY] = priority;
        }
    }
}
