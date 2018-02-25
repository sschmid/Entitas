using System.Linq;
using DesperateDevs.Utils;

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

                // Context
                .Replace("${ContextType}", contextName.AddContextSuffix())

                // Entity
                .Replace("${EntityType}", contextName.AddEntitySuffix())

                // Matcher
                .Replace("${MatcherType}", contextName.AddMatcherSuffix())

                // Component
                .Replace("${ComponentType}", data.GetTypeName())
                .Replace("${ComponentName}", componentName)
                .Replace("${componentName}", componentName.LowercaseFirst())

                // Unique component
                .Replace("${prefixedComponentName}", data.GetUniquePrefix().LowercaseFirst() + componentName)
                .Replace("${Context}", contextName)
                .Replace("${componentName}", componentName.LowercaseFirst())
                .Replace("${methodParameters}", getMethodParameters(data.GetMemberData()))
                .Replace("${methodArgs}", getMethodArgs(data.GetMemberData()));
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
