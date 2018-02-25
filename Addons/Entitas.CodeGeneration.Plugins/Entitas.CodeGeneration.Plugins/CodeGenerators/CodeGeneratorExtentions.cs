using System.Linq;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public static class CodeGeneratorExtentions {

        public static bool ignoreNamespaces;

        public static string ComponentName(this ComponentData data) {
            return data.GetTypeName().ToComponentName(ignoreNamespaces);
        }

        public static string ComponentNameWithContext(this ComponentData data, string contextName) {
            return contextName + data.ComponentName();
        }

        public static string Replace(this string template, ComponentData data, string contextName) {
            var componentName = data.ComponentName();
            return template
                .Replace("${ContextType}", contextName.AddContextSuffix())
                .Replace("${EntityType}", contextName.AddEntitySuffix())
                .Replace("${MatcherType}", contextName.AddMatcherSuffix())
                .Replace("${ComponentType}", data.GetTypeName())
                .Replace("${ComponentName}", componentName)
                .Replace("${componentName}", componentName.LowercaseFirst())
                .Replace("${prefixedComponentName}", data.GetUniquePrefix().LowercaseFirst() + componentName)
                .Replace("${methodParameters}", getMethodParameters(data.GetMemberData()))
                .Replace("${methodArgs}", getMethodArgs(data.GetMemberData()));
        }

        public static string Replace(this string template, ComponentData data, string contextName, EventData eventData) {
            var eventListener = data.EventListener(contextName, eventData);
            var lowerEventListener = data.GetContextNames().Length > 1
                ? contextName.LowercaseFirst() + data.ComponentName() + "Listener"
                : data.ComponentName().LowercaseFirst() + "Listener";

            return template
                .Replace(data, contextName)
                .Replace("${EventListenerComponent}", eventListener.AddComponentSuffix())
                .Replace("${EventListener}", eventListener)
                .Replace("${eventListener}", lowerEventListener);
        }

        public static string EventListener(this ComponentData data, string contextName, EventData eventData) {
            var optionalContextName = data.GetContextNames().Length > 1 ? contextName : string.Empty;
            var eventTypeSuffix = eventData.eventType == EventType.Removed ? "Removed" : string.Empty;
            return optionalContextName + data.ComponentName() + eventTypeSuffix + "Listener";
        }

        static string getEventMethodArgs(this ComponentData data, EventData eventData, string args) {
            if (data.GetMemberData().Length == 0) {
                return string.Empty;
            }

            return eventData.eventType == EventType.Removed
                ? string.Empty
                : args;
        }

        static string getMethodParameters(MemberData[] memberData) {
            return string.Join(", ", memberData
                .Select(info => info.type + " new" + info.name.UppercaseFirst())
                .ToArray());
        }

        static string getMethodArgs(MemberData[] memberData) {
            return string.Join(", ", memberData
                .Select(info => "new" + info.name.UppercaseFirst())
                .ToArray()
            );
        }
    }
}
