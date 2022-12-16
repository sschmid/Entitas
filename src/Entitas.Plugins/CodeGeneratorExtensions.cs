using System.CodeDom.Compiler;
using System.Linq;
using DesperateDevs.Extensions;
using Entitas.Plugins.Attributes;

namespace Entitas.Plugins
{
    public static class CodeGeneratorExtensions
    {
        public const string ComponentLookup = "ComponentsLookup";

        public const string KeywordPrefix = "@";

        static readonly CodeDomProvider CodeDomProvider = CodeDomProvider.CreateProvider("C#");

        public static string Replace(this string template, string context) => template
            .Replace("${Context}", context)
            .Replace("${Context.Name}", context)
            .Replace("${context}", context.ToLowerFirst())
            .Replace("${ContextType}", context.AddContextSuffix())
            .Replace("${Context.Type}", context.AddContextSuffix())
            .Replace("${EntityType}", context.AddEntitySuffix())
            .Replace("${Entity.Type}", context.AddEntitySuffix())
            .Replace("${Context.Entity.Type}", context.AddEntitySuffix())
            .Replace("${MatcherType}", context.AddMatcherSuffix())
            .Replace("${Matcher.Type}", context.AddMatcherSuffix())
            .Replace("${Context.Matcher.Type}", context.AddMatcherSuffix())
            .Replace("${Lookup}", context + ComponentLookup);

        public static string Replace(this string template, ComponentData data, string context)
        {
            var componentName = data.Name;
            var memberData = data.MemberData;
            return template
                .Replace(context)
                .Replace("${ComponentType}", data.Type)
                .Replace("${Component.Type}", data.Type)
                .Replace("${ComponentName}", componentName)
                .Replace("${Component.Name}", componentName)
                .Replace("${componentName}", componentName.ToLowerFirst())
                .Replace("${validComponentName}", data.ValidLowerFirstName)
                .Replace("${Component.ValidLowerFirstName}", data.ValidLowerFirstName)
                .Replace("${prefixedComponentName}", data.PrefixedComponentName())
                .Replace("${PrefixedComponentName}", data.PrefixedComponentName())
                .Replace("${newMethodParameters}", GetMethodParameters(memberData, true))
                .Replace("${methodParameters}", GetMethodParameters(memberData, false))
                .Replace("${newMethodArgs}", GetMethodArgs(memberData, true))
                .Replace("${methodArgs}", GetMethodArgs(memberData, false))
                .Replace("${Index}", $"{context}{ComponentLookup}.{componentName}");
        }

        public static string Replace(this string template, ComponentData data, string context, EventData eventData)
        {
            var eventListener = data.EventListener(context, eventData);
            return template
                .Replace(data, context)
                .Replace("${EventComponentName}", data.EventComponentName(eventData))
                .Replace("${EventListenerComponent}", eventListener.AddComponentSuffix())
                .Replace("${Event}", data.Event(context, eventData))
                .Replace("${EventListener}", eventListener)
                .Replace("${eventListener}", eventListener.ToLowerFirst())
                .Replace("${EventType}", GetEventTypeSuffix(eventData));
        }

        public static string PrefixedComponentName(this ComponentData data) =>
            data.FlagPrefix + data.Name;

        public static string Event(this ComponentData data, string context, EventData eventData) =>
            context + EventComponentName(data, eventData) + GetEventTypeSuffix(eventData);

        public static string EventListener(this ComponentData data, string context, EventData eventData) =>
            data.Event(context, eventData).AddListenerSuffix();

        public static string EventComponentName(this ComponentData data, EventData eventData)
        {
            var componentName = data.Name;
            var shortComponentName = data.Name;
            var eventComponentName = componentName.Replace(
                shortComponentName,
                eventData.GetEventPrefix() + shortComponentName
            );
            return eventComponentName;
        }

        public static string GetEventMethodArgs(this ComponentData data, EventData eventData, string args)
        {
            if (data.MemberData.Length == 0)
                return string.Empty;

            return eventData.EventType == EventType.Removed
                ? string.Empty
                : args;
        }

        public static string GetEventTypeSuffix(this EventData eventData) =>
            eventData.EventType == EventType.Removed ? "Removed" : string.Empty;

        public static string GetEventPrefix(this EventData eventData) =>
            eventData.EventTarget == EventTarget.Any ? "Any" : string.Empty;

        public static string GetMethodParameters(this MemberData[] memberData, bool newPrefix) => string.Join(", ", memberData
            .Select(info => info.Type + (newPrefix ? $" new{info.Name.ToUpperFirst()}" : $" {info.Name.ToLowerFirst()}")));

        public static string GetMethodArgs(MemberData[] memberData, bool newPrefix) => string.Join(", ", memberData
            .Select(info => newPrefix ? $"new{info.Name.ToUpperFirst()}" : info.Name));

        public static string AddPrefixIfIsKeyword(this string name) => CodeDomProvider.IsValidIdentifier(name)
            ? name
            : KeywordPrefix + name;
    }
}
