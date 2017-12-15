using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentContextGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Component (Context API)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string STANDARD_COMPONENT_TEMPLATE =
@"public partial class ${ContextName}Context {

    public ${ContextName}Entity ${componentName}Entity { get { return GetGroup(${ContextName}Matcher.${ComponentName}).GetSingleEntity(); } }
    public ${ComponentType} ${componentName} { get { return ${componentName}Entity.${componentName}; } }
    public bool has${ComponentName} { get { return ${componentName}Entity != null; } }

    public ${ContextName}Entity Set${ComponentName}(${memberArgs}) {
        if (has${ComponentName}) {
            throw new Entitas.EntitasException(""Could not set ${ComponentName}!\n"" + this + "" already has an entity with ${ComponentType}!"",
                ""You should check if the context already has a ${componentName}Entity before setting it or use context.Replace${ComponentName}()."");
        }
        var entity = CreateEntity();
        entity.Add${ComponentName}(${methodArgs});
        return entity;
    }

    public void Replace${ComponentName}(${memberArgs}) {
        var entity = ${componentName}Entity;
        if (entity == null) {
            entity = Set${ComponentName}(${methodArgs});
        } else {
            entity.Replace${ComponentName}(${methodArgs});
        }
    }

    public void Remove${ComponentName}() {
        ${componentName}Entity.Destroy();
    }
}
";

        const string MEMBER_ARGS_TEMPLATE =
            @"${MemberType} new${MemberName}";

        const string METHOD_ARGS_TEMPLATE =
            @"new${MemberName}";

        const string FLAG_COMPONENT_TEMPLATE =
@"public partial class ${ContextName}Context {

    public ${ContextName}Entity ${componentName}Entity { get { return GetGroup(${ContextName}Matcher.${ComponentName}).GetSingleEntity(); } }

    public bool ${prefixedComponentName} {
        get { return ${componentName}Entity != null; }
        set {
            var entity = ${componentName}Entity;
            if (value != (entity != null)) {
                if (value) {
                    CreateEntity().${prefixedComponentName} = true;
                } else {
                    entity.Destroy();
                }
            }
        }
    }
}
";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateMethods())
                .Where(d => d.IsUnique())
                .SelectMany(generateExtensions)
                .ToArray();
        }

        CodeGenFile[] generateExtensions(ComponentData data) {
            return data.GetContextNames()
                .Select(contextName => generateExtension(contextName, data))
                .ToArray();
        }

        CodeGenFile generateExtension(string contextName, ComponentData data) {
            var memberData = data.GetMemberData();
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var template = memberData.Length == 0
                ? FLAG_COMPONENT_TEMPLATE
                : STANDARD_COMPONENT_TEMPLATE;

            var fileContent = template
                .Replace("${ContextName}", contextName)
                .Replace("${ComponentType}", data.GetTypeName())
                .Replace("${ComponentName}", componentName)
                .Replace("${componentName}", componentName.LowercaseFirst())
                .Replace("${prefixedComponentName}", data.GetUniquePrefix().LowercaseFirst() + componentName)
                .Replace("${memberArgs}", getMemberArgs(memberData))
                .Replace("${methodArgs}", getMethodArgs(memberData));

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
                    .Replace("${MemberName}", info.name.UppercaseFirst()))
                .ToArray();

            return string.Join(", ", args);
        }

        string getMethodArgs(MemberData[] memberData) {
            var args = memberData
                .Select(info => METHOD_ARGS_TEMPLATE.Replace("${MemberName}", info.name.UppercaseFirst()))
                .ToArray();

            return string.Join(", ", args);
        }
    }
}
