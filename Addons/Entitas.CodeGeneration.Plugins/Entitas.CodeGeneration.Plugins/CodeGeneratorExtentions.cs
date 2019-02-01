using System.CodeDom.Compiler;
using System.Linq;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public static class CodeGeneratorExtentions {

        public const string LOOKUP = "ComponentsLookup";

        const string KEYWORD_PREFIX = "@";

        public static bool ignoreNamespaces;

        static readonly CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");

        public static string ComponentName(this ComponentData data) {
            return data.GetTypeName().ToComponentName(ignoreNamespaces);
        }

        public static string ComponentNameValidLowercaseFirst(this ComponentData data) {
            return ComponentName(data).LowercaseFirst().AddPrefixIfIsKeyword();
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
            return template
                .Replace(contextName)
                .Replace("${ComponentType}", data.GetTypeName())
                .Replace("${ComponentName}", data.ComponentName())
                .Replace("${componentName}", data.ComponentName().LowercaseFirst())
                .Replace("${validComponentName}", data.ComponentNameValidLowercaseFirst())
                .Replace("${prefixedComponentName}", data.PrefixedComponentName())
                .Replace("${newMethodParameters}", GetMethodParameters(data.GetMemberData(), true))
                .Replace("${methodParameters}", GetMethodParameters(data.GetMemberData(), false))
                .Replace("${newMethodArgs}", GetMethodArgs(data.GetMemberData(), true))
                .Replace("${methodArgs}", GetMethodArgs(data.GetMemberData(), false))
                .Replace("${Index}", contextName + LOOKUP + "." + data.ComponentName());

        }

        public static string Replace(this string template, ComponentData data, string contextName, EventData eventData) {
            var eventListener = data.EventListener(contextName, eventData);
            return template
                .Replace(data, contextName)
                .Replace("${EventComponentName}", data.EventComponentName(eventData))
                .Replace("${EventListenerComponent}", eventListener.AddComponentSuffix())
                .Replace("${Event}", data.Event(contextName, eventData))
                .Replace("${EventListener}", eventListener)
                .Replace("${eventListener}", eventListener.LowercaseFirst())
                .Replace("${EventType}", GetEventTypeSuffix(eventData));
        }

        public static string PrefixedComponentName(this ComponentData data) {
            return data.GetFlagPrefix().LowercaseFirst() + data.ComponentName();
        }

        public static string Event(this ComponentData data, string contextName, EventData eventData) {
            var optionalContextName = data.GetContextNames().Length > 1 ? contextName : string.Empty;
            return optionalContextName + EventComponentName(data, eventData) + GetEventTypeSuffix(eventData);
        }

        public static string EventListener(this ComponentData data, string contextName, EventData eventData) {
            return data.Event(contextName, eventData).AddListenerSuffix();
        }

        public static string EventComponentName(this ComponentData data, EventData eventData) {
            var componentName = data.GetTypeName().ToComponentName(ignoreNamespaces);
            var shortComponentName = data.GetTypeName().ToComponentName(true);
            var eventComponentName = componentName.Replace(
                shortComponentName,
                eventData.GetEventPrefix() + shortComponentName
            );
            return eventComponentName;
        }

        public static string GetEventMethodArgs(this ComponentData data, EventData eventData, string args) {
            if (data.GetMemberData().Length == 0) {
                return string.Empty;
            }

            return eventData.eventType == EventType.Removed
                ? string.Empty
                : args;
        }

        public static string GetEventTypeSuffix(this EventData eventData) {
            return eventData.eventType == EventType.Removed ? "Removed" : string.Empty;
        }

        public static string GetEventPrefix(this EventData eventData) {
            return eventData.eventTarget == EventTarget.Any ? "Any" : string.Empty;
        }

        public static string GetMethodParameters(this MemberData[] memberData, bool newPrefix) {
            return string.Join(", ", memberData
                .Select(info => info.type + (newPrefix ? " new" + info.name.UppercaseFirst() : " " + info.name.LowercaseFirst()))
                .ToArray());
        }

        public static string GetMethodArgs(MemberData[] memberData, bool newPrefix) {
            return string.Join(", ", memberData
                .Select(info => (newPrefix ? "new" + info.name.UppercaseFirst() : info.name))
                .ToArray()
            );
        }

        public static string AddPrefixIfIsKeyword(this string name) {
            if (!provider.IsValidIdentifier(name)) {
                name = KEYWORD_PREFIX + name;
            }

            return name;
        }
    }
}
