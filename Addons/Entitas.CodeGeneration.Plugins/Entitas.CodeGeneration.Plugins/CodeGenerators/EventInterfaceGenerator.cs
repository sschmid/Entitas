using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class EventInterfaceGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Event (Interface)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string STANDARD_INTERFACE_TEMPLATE =
@"public interface I${ContextName}${ComponentName}Event {

    void On${ComponentName}Changed(${memberArgs});
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
                .Where(d => d.GetEventData() != null)
                .SelectMany(generateExtensions)
                .ToArray();
        }

        CodeGenFile[] generateExtensions(ComponentData data) {
            return data.GetContextNames()
                .Select(contextName => generateExtension(contextName, data))
                .ToArray();
        }

        CodeGenFile generateExtension(string contextName, ComponentData data) {
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var memberData = data.GetMemberData();

            if (memberData.Length == 0) {
                memberData = new[] { new MemberData("bool", data.GetUniquePrefix().LowercaseFirst() + componentName) };
            }

            if (data.GetEventData().bindToEntity) {
                memberData = memberData
                    .Concat(new[] { new MemberData(contextName + "Entity", "entity") })
                    .ToArray();
            }

            var fileContent = STANDARD_INTERFACE_TEMPLATE
                .Replace("${ContextName}", contextName)
                .Replace("${ComponentName}", componentName)
                .Replace("${memberArgs}", getMemberArgs(memberData));

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Events" + Path.DirectorySeparatorChar +
                "Interfaces" + Path.DirectorySeparatorChar +
                "I" + contextName + componentName + "Event.cs",
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
