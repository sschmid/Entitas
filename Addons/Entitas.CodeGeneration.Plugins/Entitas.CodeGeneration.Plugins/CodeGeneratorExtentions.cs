using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Linq;
using System.Text;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins
{
    public static class CodeGeneratorExtentions
    {
        public const string LOOKUP = "ComponentsLookup";

        const string KEYWORD_PREFIX = "@";

        static readonly CodeDomProvider provider = CodeDomProvider.CreateProvider("C#");

        public static string ComponentNameValidLowercaseFirst(this ComponentData data)
        {
            return data.GetTypeName().ToComponentName().LowercaseFirst().AddPrefixIfIsKeyword();
        }

        public static string ComponentNameWithContext(this ComponentData data, string contextName)
        {
            return contextName + data.GetTypeName().ToComponentName();
        }

        public static string Replace(this string template, string contextName)
        {
            return template
                .Replace("${ContextName}", contextName)
                .Replace("${contextName}", contextName.LowercaseFirst())
                .Replace("${ContextType}", contextName.AddContextSuffix())
                .Replace("${EntityType}", contextName.AddEntitySuffix())
                .Replace("${MatcherType}", contextName.AddMatcherSuffix())
                .Replace("${Lookup}", contextName + LOOKUP);
        }

        public static string Replace(this string template, ComponentData data, string contextName)
        {
            return template
                .Replace(contextName)
                .Replace("${ComponentType}", data.GetTypeName().ToComponentName().AddComponentSuffix())
                .Replace("${ComponentName}", data.GetTypeName().ToComponentName())
                .Replace("${componentName}", data.GetTypeName().ToComponentName().LowercaseFirst())
                .Replace("${validComponentName}", data.ComponentNameValidLowercaseFirst())
                .Replace("${prefixedComponentName}", data.PrefixedComponentName())
                .Replace("${newMethodParameters}", GetMethodParameters(data.GetMemberData(), true))
                .Replace("${methodParameters}", GetMethodParameters(data.GetMemberData(), false))
                .Replace("${newMethodArgs}", GetMethodArgs(data.GetMemberData(), true))
                .Replace("${methodArgs}", GetMethodArgs(data.GetMemberData(), false))
                .Replace("${Index}", contextName + LOOKUP + "." + data.GetTypeName().ToComponentName());
        }

        public static string Replace(this string template, ComponentData data, string contextName, EventData eventData)
        {
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

        public static string PrefixedComponentName(this ComponentData data)
        {
            return data.GetFlagPrefix().LowercaseFirst() + data.GetTypeName().ToComponentName();
        }

        public static string Event(this ComponentData data, string contextName, EventData eventData)
        {
            // var optionalContextName = data.GetContextNames().Length > 1 ? contextName : string.Empty;
            // return optionalContextName + EventComponentName(data, eventData) + GetEventTypeSuffix(eventData);
            return EventComponentName(data, eventData) + GetEventTypeSuffix(eventData);
        }

        public static string EventListener(this ComponentData data, string contextName, EventData eventData)
        {
            return data.Event(contextName, eventData).AddListenerSuffix();
        }

        public static string EventComponentName(this ComponentData data, EventData eventData)
        {
            var componentName = data.GetTypeName().ToComponentName();
            var shortComponentName = data.GetTypeName().ToComponentName();
            var eventComponentName = componentName.Replace(
                shortComponentName,
                eventData.GetEventPrefix() + shortComponentName
            );
            return eventComponentName;
        }

        public static string GetEventMethodArgs(this ComponentData data, EventData eventData, string args)
        {
            if (data.GetMemberData().Length == 0)
            {
                return string.Empty;
            }

            return eventData.eventType == EventType.Removed
                ? string.Empty
                : args;
        }

        public static string GetEventTypeSuffix(this EventData eventData)
        {
            return eventData.eventType == EventType.Removed ? "Removed" : string.Empty;
        }

        public static string GetEventPrefix(this EventData eventData)
        {
            return eventData.eventTarget == EventTarget.Any ? "Any" : string.Empty;
        }

        public static string GetMethodParameters(this MemberData[] memberData, bool newPrefix)
        {
            return string.Join(", ", memberData
                .Select(info => info.type + (newPrefix ? " new" + info.name.UppercaseFirst() : " " + info.name.LowercaseFirst()))
                .ToArray());
        }

        public static string GetMethodArgs(MemberData[] memberData, bool newPrefix)
        {
            return string.Join(", ", memberData
                .Select(info => (newPrefix ? "new" + info.name.UppercaseFirst() : info.name))
                .ToArray()
            );
        }

        public static string AddPrefixIfIsKeyword(this string name)
        {
            if (!provider.IsValidIdentifier(name))
            {
                name = KEYWORD_PREFIX + name;
            }

            return name;
        }

        public static string WrapInNamespace(this string str, params string[] namespaces)
        {
            if (namespaces != null)
            {
                namespaces = namespaces.Where(n => n != null).ToArray();
                var ns = string.Join(".", namespaces);
                if (!string.IsNullOrEmpty(ns))
                {
                    var lines = str.Split('\n');
                    var sb = new StringBuilder();
                    const string indent = "    ";
                    foreach (var line in lines)
                    {
                        if (line.Length > 0)
                            sb.Append(indent).Append(line).Append('\n');
                        else
                            sb.Append('\n');
                    }

                    return "namespace " + ns + "\n{\n" + sb.ToString().TrimEnd() + "\n}\n";
                }
            }

            return str;
        }

        public static string ToFileName(this string fullComponentTypeName, string contextName)
        {
            return Path.Combine("_" + contextName, fullComponentTypeName.ToComponentName() + ".cs");
        }
    }
}
