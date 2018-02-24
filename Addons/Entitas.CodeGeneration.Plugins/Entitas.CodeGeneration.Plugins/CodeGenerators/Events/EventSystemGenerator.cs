using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class EventSystemGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Event (System)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string SYSTEM_TEMPLATE =
            @"public sealed class ${OptionalContextName}${ComponentName}${EventType}EventSystem : Entitas.ReactiveSystem<${ContextName}Entity> {

    readonly Entitas.IGroup<${ContextName}Entity> _listeners;

    public ${OptionalContextName}${ComponentName}${EventType}EventSystem(Contexts contexts) : base(contexts.${contextName}) {
        _listeners = contexts.${contextName}.GetGroup(${ContextName}Matcher.${OptionalContextName}${ComponentName}${EventType}Listener);
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
            foreach (var listenerEntity in _listeners) {
                foreach (var listener in listenerEntity.${optionalContextName}${contextDependentComponentName}${EventType}Listener.value) {
                    listener.On${ComponentName}${EventType}(e${methodArgs});
                }
            }
        }
    }
}
";

        const string ENTITY_SYSTEM_TEMPLATE =
            @"public sealed class ${OptionalContextName}${ComponentName}${EventType}EventSystem : Entitas.ReactiveSystem<${ContextName}Entity> {

    public ${OptionalContextName}${ComponentName}${EventType}EventSystem(Contexts contexts) : base(contexts.${contextName}) {
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
            foreach (var listener in e.${optionalContextName}${contextDependentComponentName}${EventType}Listener.value) {
                listener.On${ComponentName}${EventType}(e${methodArgs});
            }
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
                .SelectMany(contextName => generateSystem(contextName, data))
                .ToArray();
        }

        CodeGenFile[] generateSystem(string contextName, ComponentData data) {
            return data.GetEventData()
                .Select(eventData => {
                    var optionalContextName = data.GetContextNames().Length > 1 ? contextName : string.Empty;
                    var componentName = data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces);
                    var memberData = data.GetMemberData();

                    var eventTypeSuffix = data.GetEventTypeSuffix(eventData);
                    var methodArgs = ", " + (memberData.Length == 0
                                         ? data.GetUniquePrefix() + componentName
                                         : getMethodArgs(memberData));

                    methodArgs = data.GetArgs(eventData, methodArgs);

                    var filter = getFilter(data, eventData, componentName, optionalContextName, eventTypeSuffix);

                    var cachedAccess = memberData.Length == 0
                        ? string.Empty
                        : "var component = e." + componentName.LowercaseFirst() + ";";

                    if (eventData.eventType == EventType.Removed) {
                        methodArgs = string.Empty;
                        cachedAccess = string.Empty;
                    }

                    var template = eventData.bindToEntity
                        ? ENTITY_SYSTEM_TEMPLATE
                        : SYSTEM_TEMPLATE;

                    var fileContent = template
                        .Replace("${ContextName}", contextName)
                        .Replace("${contextName}", contextName.LowercaseFirst())
                        .Replace("${OptionalContextName}", optionalContextName)
                        .Replace("${optionalContextName}", optionalContextName == string.Empty ? string.Empty : optionalContextName.LowercaseFirst())
                        .Replace("${ComponentName}", componentName)
                        .Replace("${contextDependentComponentName}", optionalContextName == string.Empty ? componentName.LowercaseFirst() : componentName)
                        .Replace("${GroupEvent}", eventData.eventType.ToString())
                        .Replace("${filter}", filter)
                        .Replace("${cachedAccess}", cachedAccess)
                        .Replace("${EventType}", eventTypeSuffix)
                        .Replace("${methodArgs}", methodArgs);

                    return new CodeGenFile(
                        "Events" + Path.DirectorySeparatorChar +
                        "Systems" + Path.DirectorySeparatorChar +
                        optionalContextName + componentName + eventTypeSuffix + "EventSystem.cs",
                        fileContent,
                        GetType().FullName
                    );
                }).ToArray();
        }

        string getFilter(ComponentData data, EventData eventData, string componentName, string optionalContextName, string eventTypeSuffix) {
            var filter = string.Empty;
            if (data.GetMemberData().Length == 0) {
                switch (eventData.eventType) {
                    case EventType.Added:
                        filter = "entity." + data.GetUniquePrefix() + componentName;
                        break;
                    case EventType.Removed:
                        filter = "!entity." + data.GetUniquePrefix() + componentName;
                        break;
                }
            } else {
                switch (eventData.eventType) {
                    case EventType.Added:
                        filter = "entity.has" + componentName;
                        break;
                    case EventType.Removed:
                        filter = "!entity.has" + componentName;
                        break;
                }
            }

            if (eventData.bindToEntity) {
                if (filter == string.Empty) {
                    filter = "entity.has${OptionalContextName}${ComponentName}${EventType}Listener";
                } else {
                    filter += " && entity.has${OptionalContextName}${ComponentName}${EventType}Listener";
                }
            }

            return filter
                .Replace("${OptionalContextName}", optionalContextName)
                .Replace("${ComponentName}", componentName)
                .Replace("${EventType}", eventTypeSuffix);
        }

        string getMethodArgs(MemberData[] memberData) {
            var args = memberData
                .Select(info => METHOD_ARGS_TEMPLATE.Replace("${MemberName}", info.name))
                .ToArray();

            return string.Join(", ", args);
        }
    }
}
