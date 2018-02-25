using System.Linq;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public static class CodeGeneratorExtentions {

        public const string LOOKUP = "ComponentsLookup";

        public static bool ignoreNamespaces;

        public static string ComponentName(this ComponentData data) {
            return data.GetTypeName().ToComponentName(ignoreNamespaces);
        }

        public static string ComponentNameWithContext(this ComponentData data, string contextName) {
            return contextName + data.ComponentName();
        }

        public static string Replace(this string template, string contextName) {
            return template
                .Replace("${ContextName}", contextName)
                .Replace("${contextName}", contextName.LowercaseFirst())
                .Replace("${ContextType}", contextName.AddContextSuffix())
                .Replace("${EntityType}", contextName.AddEntitySuffix())
                .Replace("${MatcherType}", contextName.AddMatcherSuffix())
                .Replace("${Lookup}", contextName + LOOKUP);
        }

        public static string Replace(this string template, ComponentData data, string contextName) {
            var componentName = data.ComponentName();
            return template
                .Replace(contextName)
                .Replace("${ComponentType}", data.GetTypeName())
                .Replace("${ComponentName}", componentName)
                .Replace("${componentName}", componentName.LowercaseFirst())
                .Replace("${prefixedComponentName}", data.PrefixedComponentName())
                .Replace("${newMethodParameters}", getMethodParameters(data.GetMemberData(), true))
                .Replace("${methodParameters}", getMethodParameters(data.GetMemberData(), false))
                .Replace("${methodArgs}", getMethodArgs(data.GetMemberData()))
                .Replace("${Index}", contextName + LOOKUP + "." + data.ComponentName());
        }

        public static string Replace(this string template, ComponentData data, string contextName, EventData eventData) {
            var eventListener = data.EventListener(contextName, eventData);
            var lowerEventListener = data.GetContextNames().Length > 1
                ? contextName.LowercaseFirst() + data.ComponentName() + "Listener"
                : data.ComponentName().LowercaseFirst() + "Listener";

            return template
                .Replace(data, contextName)
                .Replace("${EventListenerComponent}", eventListener.AddComponentSuffix())
                .Replace("${Event}", data.Event(contextName, eventData))
                .Replace("${EventListener}", eventListener)
                .Replace("${eventListener}", lowerEventListener)
                .Replace("${EventType}", getEventTypeSuffix(eventData));
        }

        public static string PrefixedComponentName(this ComponentData data) {
            return data.GetUniquePrefix().LowercaseFirst() + data.ComponentName();
        }

        public static string Event(this ComponentData data, string contextName, EventData eventData) {
            var optionalContextName = data.GetContextNames().Length > 1 ? contextName : string.Empty;
            return optionalContextName + data.ComponentName() + getEventTypeSuffix(eventData);
        }

        public static string EventListener(this ComponentData data, string contextName, EventData eventData) {
            return data.Event(contextName, eventData) + "Listener";
        }

        public static string GetEventMethodArgs(this ComponentData data, EventData eventData, string args) {
            if (data.GetMemberData().Length == 0) {
                return string.Empty;
            }

            return eventData.eventType == EventType.Removed
                ? string.Empty
                : args;
        }

        static string getEventTypeSuffix(EventData eventData) {
            return eventData.eventType == EventType.Removed ? "Removed" : string.Empty;
        }

        static string getMethodParameters(MemberData[] memberData, bool newPrefix) {
            return string.Join(", ", memberData
                .Select(info => info.type + (newPrefix ? " new" : " ") + info.name.UppercaseFirst())
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
