using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class EventSystemGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Event (System)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string SYSTEM_TEMPLATE =
@"public sealed class ${OptionalContextName}${ComponentName}EventSystem : Entitas.ReactiveSystem<${ContextName}Entity> {

    readonly Entitas.IGroup<${ContextName}Entity> _listeners;

    public ${OptionalContextName}${ComponentName}EventSystem(Contexts contexts) : base(contexts.${contextName}) {
        _listeners = contexts.${contextName}.GetGroup(${ContextName}Matcher.${OptionalContextName}${ComponentName}Listener);
    }

    protected override Entitas.ICollector<${ContextName}Entity> GetTrigger(Entitas.IContext<${ContextName}Entity> context) {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.${GroupEvent}(${ContextName}Matcher.${ComponentName})
        );
    }

    protected override bool Filter(${ContextName}Entity entity) {
        return ${filter};
    }

    protected override void Execute(System.Collections.Generic.List<${ContextName}Entity> entities) {
        foreach (var e in entities) {
            ${cachedAccess}
            foreach (var listener in _listeners) {
                listener.${optionalContextName}${contextDependentComponentName}Listener.value.On${ComponentName}(e, ${methodArgs});
            }
        }
    }
}
";

        const string ENTITY_SYSTEM_TEMPLATE =
@"public sealed class ${OptionalContextName}${ComponentName}EventSystem : Entitas.ReactiveSystem<${ContextName}Entity> {

    public ${OptionalContextName}${ComponentName}EventSystem(Contexts contexts) : base(contexts.${contextName}) {
    }

    protected override Entitas.ICollector<${ContextName}Entity> GetTrigger(Entitas.IContext<${ContextName}Entity> context) {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.${GroupEvent}(${ContextName}Matcher.${ComponentName})
        );
    }

    protected override bool Filter(${ContextName}Entity entity) {
        return ${filter} && entity.has${OptionalContextName}${ComponentName}Listener;
    }

    protected override void Execute(System.Collections.Generic.List<${ContextName}Entity> entities) {
        foreach (var e in entities) {
            ${cachedAccess}
            e.${optionalContextName}${contextDependentComponentName}Listener.value.On${ComponentName}(e, ${methodArgs});
        }
    }
}
";

        const string METHOD_ARGS_TEMPLATE =
            @"component.${MemberName}";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.IsEvent())
                .SelectMany(generateSystems)
                .ToArray();
        }

        CodeGenFile[] generateSystems(ComponentData data) {
            return data.GetContextNames()
                .Select(contextName => generateSystem(contextName, data))
                .ToArray();
        }

        CodeGenFile generateSystem(string contextName, ComponentData data) {
            var optionalContextName = data.GetContextNames().Length > 1 ? contextName : string.Empty;
            var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
            var memberData = data.GetMemberData();

            var groupEvent = memberData.Length == 0
                ? "AddedOrRemoved"
                : "Added";

            var filter = memberData.Length == 0
                ? "true"
                : "entity.has" + componentName;

            var cachedAccess = memberData.Length == 0
                ? "var " + data.GetUniquePrefix() + componentName + " = e." + data.GetUniquePrefix() + componentName + ";"
                : "var component = e." + componentName.LowercaseFirst() + ";";

            var methodArgs = memberData.Length == 0
                ? data.GetUniquePrefix() + componentName
                : getMethodArgs(memberData);

            var template = data.GetBindToEntity()
                ? ENTITY_SYSTEM_TEMPLATE
                : SYSTEM_TEMPLATE;

            var fileContent = template
                .Replace("${ContextName}", contextName)
                .Replace("${contextName}", contextName.LowercaseFirst())
                .Replace("${OptionalContextName}", optionalContextName)
                .Replace("${optionalContextName}", optionalContextName == string.Empty ? string.Empty: optionalContextName.LowercaseFirst())
                .Replace("${ComponentName}", componentName)
                .Replace("${contextDependentComponentName}", optionalContextName == string.Empty ? componentName.LowercaseFirst() : componentName)
                .Replace("${GroupEvent}", groupEvent)
                .Replace("${filter}", filter)
                .Replace("${cachedAccess}", cachedAccess)
                .Replace("${methodArgs}", methodArgs);

            return new CodeGenFile(
                "Events" + Path.DirectorySeparatorChar +
                "Systems" + Path.DirectorySeparatorChar +
                optionalContextName + componentName + "EventSystem.cs",
                fileContent,
                GetType().FullName
            );
        }

        string getMethodArgs(MemberData[] memberData) {
            var args = memberData
                .Select(info => METHOD_ARGS_TEMPLATE.Replace("${MemberName}", info.name))
                .ToArray();

            return string.Join(", ", args);
        }
    }
}
