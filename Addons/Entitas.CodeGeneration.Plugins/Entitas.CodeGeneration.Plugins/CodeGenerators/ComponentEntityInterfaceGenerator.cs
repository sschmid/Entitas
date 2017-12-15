using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentEntityInterfaceGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Component (Entity Interface)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string STANDARD_INTERFACE_TEMPLATE =
@"public interface ${InterfaceName} {

    ${ComponentType} ${componentName} { get; }
    bool has${ComponentName} { get; }

    void Add${ComponentName}(${memberArgs});
    void Replace${ComponentName}(${memberArgs});
    void Remove${ComponentName}();
}
";

        const string FLAG_INTERFACE_TEMPLATE =
@"public interface ${InterfaceName} {

    bool ${prefixedName} { get; set; }
}
";

        const string ENTITY_INTERFACE_EXTENSION =
            @"public partial class ${ContextName}Entity : ${InterfaceName} { }
";

        const string MEMBER_ARGS_TEMPLATE =
            @"${MemberType} new${MemberName}";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods())
                .SelectMany(generateExtensions)
                .ToArray();
        }

        CodeGenFile[] generateExtensions(ComponentData data) {
            if (data.GetContextNames().Length > 1) {
                return new[] { generateInterface(data) }.Concat(
                    data.GetContextNames().Select(contextName => generateEntityInterfaceExtension(contextName, data))
                ).ToArray();
            }

            return new CodeGenFile[0];
        }

        CodeGenFile generateInterface(ComponentData data) {
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var memberData = data.GetMemberData();
            var interfaceName = "I" + componentName.RemoveComponentSuffix();

            var template = memberData.Length == 0
                ? FLAG_INTERFACE_TEMPLATE
                : STANDARD_INTERFACE_TEMPLATE;

            var fileContent = template
                .Replace("${InterfaceName}", interfaceName)
                .Replace("${ComponentType}", data.GetTypeName())
                .Replace("${ComponentName}", componentName)
                .Replace("${componentName}", componentName.LowercaseFirst())
                .Replace("${prefixedName}", data.GetUniquePrefix().LowercaseFirst() + componentName)
                .Replace("${memberArgs}", getMemberArgs(memberData));

            return new CodeGenFile(
                "Components" + Path.DirectorySeparatorChar +
                "Interfaces" + Path.DirectorySeparatorChar +
                interfaceName + ".cs",
                fileContent,
                GetType().FullName
            );
        }

        CodeGenFile generateEntityInterfaceExtension(string contextName, ComponentData data) {
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var interfaceName = "I" + componentName.RemoveComponentSuffix();

            var fileContent = ENTITY_INTERFACE_EXTENSION
                .Replace("${InterfaceName}", "I" + componentName.RemoveComponentSuffix())
                .Replace("${ContextName}", contextName);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                contextName + componentName.AddComponentSuffix() + ".cs",
                fileContent,
                GetType().FullName
            );
        }

        string getMemberArgs(MemberData[] memberData) {
            var args = memberData
                .Select(info => MEMBER_ARGS_TEMPLATE
                    .Replace("${MemberType}", info.type)
                    .Replace("${MemberName}", info.name.UppercaseFirst())
                )
                .ToArray();

            return string.Join(", ", args);
        }
    }
}
