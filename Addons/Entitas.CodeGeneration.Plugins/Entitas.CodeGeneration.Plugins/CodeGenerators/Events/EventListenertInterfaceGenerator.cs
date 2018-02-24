using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class EventListenertInterfaceGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Event (Listener Interface)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string INTERFACE_TEMPLATE =
            @"public interface I${OptionalContextName}${ComponentName}Listener {
    void On${ComponentName}${EventType}(${ContextName}Entity entity${memberArgs});
}
";

        const string MEMBER_ARGS_TEMPLATE =
            @"${MemberType} ${MemberName}";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.IsEvent())
                .SelectMany(generateInterfaces)
                .ToArray();
        }

        CodeGenFile[] generateInterfaces(ComponentData data) {
            return data.GetContextNames()
                .Select(contextName => generateInterface(contextName, data))
                .ToArray();
        }

        CodeGenFile generateInterface(string contextName, ComponentData data) {
            var optionalContextName = data.GetContextNames().Length > 1 ? contextName : string.Empty;
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var memberData = data.GetMemberData();
            if (memberData.Length == 0) {
                memberData = new[] { new MemberData("bool", data.GetUniquePrefix().LowercaseFirst() + componentName) };
            }

            var eventTypeSuffix = string.Empty;
            var memberArgs = ", " + getMemberArgs(memberData);
            if (data.GetMemberData().Length == 0) {
                switch (data.GetEventType()) {
                    case EventType.Added:
                        eventTypeSuffix = "Added";
                        memberArgs = string.Empty;
                        break;
                    case EventType.Removed:
                        eventTypeSuffix = "Removed";
                        memberArgs = string.Empty;
                        break;
                }
            } else {
                switch (data.GetEventType()) {
                    case EventType.Removed:
                        eventTypeSuffix = "Removed";
                        memberArgs = string.Empty;
                        break;
                    case EventType.AddedOrRemoved:
                        eventTypeSuffix = "AddedOrRemoved";
                        break;
                }
            }

            var fileContent = INTERFACE_TEMPLATE
                .Replace("${ContextName}", contextName)
                .Replace("${OptionalContextName}", optionalContextName)
                .Replace("${ComponentName}", componentName)
                .Replace("${ComponentName}", componentName)
                .Replace("${EventType}", eventTypeSuffix)
                .Replace("${memberArgs}", memberArgs);

            return new CodeGenFile(
                "Events" + Path.DirectorySeparatorChar +
                "Interfaces" + Path.DirectorySeparatorChar +
                "I" + optionalContextName + componentName + "Listener.cs",
                fileContent,
                GetType().FullName
            );
        }

        string getMemberArgs(MemberData[] memberData) {
            var args = memberData
                .Select(info => MEMBER_ARGS_TEMPLATE
                    .Replace("${MemberType}", info.type)
                    .Replace("${MemberName}", info.name)
                )
                .ToArray();

            return string.Join(", ", args);
        }
    }
}
