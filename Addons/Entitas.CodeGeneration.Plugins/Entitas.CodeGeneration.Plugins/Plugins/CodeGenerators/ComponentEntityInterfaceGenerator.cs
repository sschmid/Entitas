using System.Collections.Generic;
using System.IO;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentEntityInterfaceGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Component (Entity Interface)"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string INTERFACE_TEMPLATE =
@"public interface ${InterfaceName} {

    ${ComponentType} ${componentName} { get; }
    bool has${ComponentName} { get; }

    void Add${ComponentName}(${memberArgs});
    void Replace${ComponentName}(${memberArgs});
    void Remove${ComponentName}();
}
";

        const string ENTITY_INTERFACE_EXTENSION =
@"public partial class ${ContextName}Entity : ${InterfaceName} { }";

        const string MEMBER_ARGS_TEMPLATE =
@"${MemberType} new${MemberName}";

        public void Configure(Properties properties) {
            _ignoreNamespacesConfig.Configure(properties);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods())
                .SelectMany(d => generateExtensions(d))
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
            var componentName = data.GetFullTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var memberData = data.GetMemberData();
            var interfaceName = "I" + componentName.RemoveComponentSuffix();

            var fileContent = INTERFACE_TEMPLATE
                .Replace("${InterfaceName}", interfaceName)
                .Replace("${ComponentType}", data.GetFullTypeName())
                .Replace("${ComponentName}", componentName)
                .Replace("${componentName}", componentName.LowercaseFirst())
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
            var componentName = data.GetFullTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
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
