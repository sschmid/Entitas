using System.CodeDom.Compiler;
using System.Linq;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public static class CodeGeneratorExtentions {

        public const string LOOKUP = "ComponentsLookup";

        public static bool ignoreNamespaces;
        
        public static readonly CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");
        private const string KEYWORD_PREFIX = "@";

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
            
            var lowercaseFirst = componentName.LowercaseFirst();
            var validName      = lowercaseFirst.AddKeywordPrefixIfInvalid();

            return template
                .Replace(contextName)
                .Replace("${ComponentType}", data.GetTypeName())
                .Replace("${ComponentName}", componentName)
                .Replace("${componentName}", lowercaseFirst)
                .Replace("${componentNameValid}", validName)
                .Replace("${prefixedComponentName}", data.PrefixedComponentName())
                .Replace("${newMethodParameters}", GetMethodParameters(data.GetMemberData(), true))
                .Replace("${methodParameters}", GetMethodParameters(data.GetMemberData(), false))
                .Replace("${newMethodArgs}", GetMethodArgs(data.GetMemberData(), true))
                .Replace("${methodArgs}", GetMethodArgs(data.GetMemberData(), false))
                .Replace("${Index}", contextName + LOOKUP + "." + data.ComponentName());

        }

        public static string Replace(this string template, ComponentData data, string contextName, EventData eventData) {
            var eventListener = data.EventListener(contextName, eventData);
            var lowerEventListener = data.GetContextNames().Length > 1
                ? contextName.LowercaseFirst() + data.ComponentName() + GetEventTypeSuffix(eventData).AddListenerSuffix()
                : data.ComponentName().LowercaseFirst() + GetEventTypeSuffix(eventData).AddListenerSuffix();

            return template
                .Replace(data, contextName)
                .Replace("${EventListenerComponent}", eventListener.AddComponentSuffix())
                .Replace("${Event}", data.Event(contextName, eventData))
                .Replace("${EventListener}", eventListener)
                .Replace("${eventListener}", lowerEventListener)
                .Replace("${EventType}", GetEventTypeSuffix(eventData));
        }

        public static string PrefixedComponentName(this ComponentData data) {
            return data.GetFlagPrefix().LowercaseFirst() + data.ComponentName();
        }

        public static string Event(this ComponentData data, string contextName, EventData eventData) {
            var optionalContextName = data.GetContextNames().Length > 1 ? contextName : string.Empty;
            return optionalContextName + data.ComponentName() + GetEventTypeSuffix(eventData);
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

        public static string GetEventTypeSuffix(this EventData eventData) {
            return eventData.eventType == EventType.Removed ? "Removed" : string.Empty;
        }

        public static string GetMethodParameters(this MemberData[] memberData, bool newPrefix) {
            return string.Join(", ", memberData
                .Select(info => info.type + (newPrefix ? " new" + info.name.UppercaseFirst() : " " + info.name))
                .ToArray());
        }

        public static string GetMethodArgs(MemberData[] memberData, bool newPrefix) {
            return string.Join(", ", memberData
                .Select(info => (newPrefix ? "new" + info.name.UppercaseFirst() : info.name))
                .ToArray()
            );
        }
        
        public static string AddKeywordPrefixIfInvalid(this string name)
        {
            if (!provider.IsValidIdentifier(name)){
                name = KEYWORD_PREFIX + name;
            }

            return name;
        }
    }
}
