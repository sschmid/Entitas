using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;

namespace Entitas.CodeGeneration.Plugins {

    public class EventSystemGenerator : AbstractGenerator {

        public override string name { get { return "Event (System)"; } }

        const string TEMPLATE =
            @"public sealed class ${Event}EventSystem : Entitas.ReactiveSystem<${EntityType}> {

    readonly Entitas.IGroup<${EntityType}> _listeners;

    public ${Event}EventSystem(Contexts contexts) : base(contexts.${contextName}) {
        _listeners = contexts.${contextName}.GetGroup(${MatcherType}.${EventListener});
    }

    protected override Entitas.ICollector<${EntityType}> GetTrigger(Entitas.IContext<${EntityType}> context) {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.${GroupEvent}(${MatcherType}.${ComponentName})
        );
    }

    protected override bool Filter(${EntityType} entity) {
        return ${filter};
    }

    protected override void Execute(System.Collections.Generic.List<${EntityType}> entities) {
        foreach (var e in entities) {
            ${cachedAccess}
            foreach (var listenerEntity in _listeners) {
                foreach (var listener in listenerEntity.${eventListener}.value) {
                    listener.On${ComponentName}${EventType}(e${methodArgs});
                }
            }
        }
    }
}
";

        const string BIND_TO_ENTITY_TEMPLATE =
            @"public sealed class ${Event}EventSystem : Entitas.ReactiveSystem<${EntityType}> {

    public ${Event}EventSystem(Contexts contexts) : base(contexts.${contextName}) {
    }

    protected override Entitas.ICollector<${EntityType}> GetTrigger(Entitas.IContext<${EntityType}> context) {
        return Entitas.CollectorContextExtension.CreateCollector(
            context, Entitas.TriggerOnEventMatcherExtension.${GroupEvent}(${MatcherType}.${ComponentName})
        );
    }

    protected override bool Filter(${EntityType} entity) {
        return ${filter};
    }

    protected override void Execute(System.Collections.Generic.List<${EntityType}> entities) {
        foreach (var e in entities) {
            ${cachedAccess}
            foreach (var listener in e.${eventListener}.value) {
                listener.On${ComponentName}${EventType}(e${methodArgs});
            }
        }
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.IsEvent())
                .SelectMany(generate)
                .ToArray();
        }

        CodeGenFile[] generate(ComponentData data) {
            return data.GetContextNames()
                .SelectMany(contextName => generate(contextName, data))
                .ToArray();
        }

        CodeGenFile[] generate(string contextName, ComponentData data) {
            return data.GetEventData()
                .Select(eventData => {
                    var methodArgs = data.GetEventMethodArgs(eventData, ", " + (data.GetMemberData().Length == 0
                                                                            ? data.PrefixedComponentName()
                                                                            : getMethodArgs(data.GetMemberData())));

                    var cachedAccess = data.GetMemberData().Length == 0
                        ? string.Empty
                        : "var component = e." + data.ComponentName().LowercaseFirst() + ";";

                    if (eventData.eventType == EventType.Removed) {
                        methodArgs = string.Empty;
                        cachedAccess = string.Empty;
                    }

                    var template = eventData.bindToEntity
                        ? BIND_TO_ENTITY_TEMPLATE
                        : TEMPLATE;

                    var fileContent = template
                        .Replace("${GroupEvent}", eventData.eventType.ToString())
                        .Replace("${filter}", getFilter(data, contextName, eventData))
                        .Replace("${cachedAccess}", cachedAccess)
                        .Replace("${methodArgs}", methodArgs)
                        .Replace(data, contextName, eventData);

                    return new CodeGenFile(
                        "Events" + Path.DirectorySeparatorChar +
                        "Systems" + Path.DirectorySeparatorChar +
                        data.Event(contextName, eventData) + "EventSystem.cs",
                        fileContent,
                        GetType().FullName
                    );
                }).ToArray();
        }

        string getFilter(ComponentData data, string contextName, EventData eventData) {
            var filter = string.Empty;
            if (data.GetMemberData().Length == 0) {
                switch (eventData.eventType) {
                    case EventType.Added:
                        filter = "entity." + data.PrefixedComponentName();
                        break;
                    case EventType.Removed:
                        filter = "!entity." + data.PrefixedComponentName();
                        break;
                }
            } else {
                switch (eventData.eventType) {
                    case EventType.Added:
                        filter = "entity.has" + data.ComponentName();
                        break;
                    case EventType.Removed:
                        filter = "!entity.has" + data.ComponentName();
                        break;
                }
            }

            if (eventData.bindToEntity) {
                filter += " && entity.has" + data.EventListener(contextName, eventData);
            }

            return filter;
        }

        string getMethodArgs(MemberData[] memberData) {
            return string.Join(", ", memberData
                .Select(info => "component." + info)
                .ToArray()
            );
        }
    }
}
