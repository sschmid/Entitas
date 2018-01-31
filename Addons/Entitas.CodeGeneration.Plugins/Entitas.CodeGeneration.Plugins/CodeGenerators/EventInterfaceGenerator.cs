using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class EventInterfaceGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Event (Listener Interface)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string INTERFACE_TEMPLATE =
@"public interface I${ComponentName}Listener {

    void On${ComponentName}(${memberArgs});
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
                .Select(generateExtension)
                .ToArray();
        }

        CodeGenFile generateExtension(ComponentData data) {
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var memberData = data.GetMemberData();

            if (memberData.Length == 0) {
                memberData = new[] { new MemberData("bool", data.GetUniquePrefix().LowercaseFirst() + componentName) };
            }

            var fileContent = INTERFACE_TEMPLATE
                .Replace("${ComponentName}", componentName)
                .Replace("${memberArgs}", getMemberArgs(memberData));

            return new CodeGenFile(
                "Events" + Path.DirectorySeparatorChar +
                "Interfaces" + Path.DirectorySeparatorChar +
                "I" + componentName + "Listener.cs",
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
