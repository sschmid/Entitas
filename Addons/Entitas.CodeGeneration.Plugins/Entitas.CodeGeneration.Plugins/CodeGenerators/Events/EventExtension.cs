using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public static class EventExtension {

        public static string GetEventTypeSuffix(this ComponentData data, EventData eventData) {
            if (data.GetMemberData().Length == 0) {
                switch (eventData.eventType) {
                    case EventType.Added:
                        return "Added";
                    case EventType.Removed:
                        return "Removed";
                }
            } else {
                switch (eventData.eventType) {
                    case EventType.Removed:
                        return "Removed";
                    case EventType.AddedOrRemoved:
                        return "AddedOrRemoved";
                }
            }

            return string.Empty;
        }

        public static string GetArgs(this ComponentData data, EventData eventData, string args) {
            if (data.GetMemberData().Length == 0) {
                switch (eventData.eventType) {
                    case EventType.Added:
                        return string.Empty;
                    case EventType.Removed:
                        return string.Empty;
                }
            } else {
                switch (eventData.eventType) {
                    case EventType.Removed:
                        return string.Empty;
                }
            }

            return args;
        }
    }
}
