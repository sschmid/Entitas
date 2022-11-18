using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;
using Entitas.Plugins.Attributes;

namespace Entitas.Plugins
{
    public class EventSystemGenerator : AbstractGenerator
    {
        public override string Name => "Event (System)";

        const string AnyTargetTemplate =
            @"public sealed class ${Event}EventSystem : Entitas.ReactiveSystem<${EntityType}> {

    readonly Entitas.IGroup<${EntityType}> _listeners;
    readonly System.Collections.Generic.List<${EntityType}> _entityBuffer;
    readonly System.Collections.Generic.List<I${EventListener}> _listenerBuffer;

    public ${Event}EventSystem(Contexts contexts) : base(contexts.${context}) {
        _listeners = contexts.${context}.GetGroup(${MatcherType}.${EventListener});
        _entityBuffer = new System.Collections.Generic.List<${EntityType}>();
        _listenerBuffer = new System.Collections.Generic.List<I${EventListener}>();
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
            foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer)) {
                _listenerBuffer.Clear();
                _listenerBuffer.AddRange(listenerEntity.${eventListener}.value);
                foreach (var listener in _listenerBuffer) {
                    listener.On${EventComponentName}${EventType}(e${methodArgs});
                }
            }
        }
    }
}
";

        const string SelfTargetTemplate =
            @"public sealed class ${Event}EventSystem : Entitas.ReactiveSystem<${EntityType}> {

    readonly System.Collections.Generic.List<I${EventListener}> _listenerBuffer;

    public ${Event}EventSystem(Contexts contexts) : base(contexts.${context}) {
        _listenerBuffer = new System.Collections.Generic.List<I${EventListener}>();
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
            _listenerBuffer.Clear();
            _listenerBuffer.AddRange(e.${eventListener}.value);
            foreach (var listener in _listenerBuffer) {
                listener.On${ComponentName}${EventType}(e${methodArgs});
            }
        }
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.IsEvent)
            .SelectMany(d => Generate(d))
            .ToArray();

        IEnumerable<CodeGenFile> Generate(ComponentData data) => data
            .Contexts.SelectMany(context => Generate(context, data));

        CodeGenFile[] Generate(string context, ComponentData data) => data
            .EventData
            .Select(eventData =>
            {
                var methodArgs = data.GetEventMethodArgs(eventData, ", " + (data.MemberData.Length == 0
                    ? data.PrefixedComponentName()
                    : GetMethodArgs(data.MemberData)));

                var cachedAccess = data.MemberData.Length == 0
                    ? string.Empty
                    : $"var component = e.{data.Type.ToValidLowerFirst()};";

                if (eventData.EventType == EventType.Removed)
                {
                    methodArgs = string.Empty;
                    cachedAccess = string.Empty;
                }

                var template = eventData.EventTarget == EventTarget.Self
                    ? SelfTargetTemplate
                    : AnyTargetTemplate;

                var fileContent = template
                    .Replace("${GroupEvent}", eventData.EventType.ToString())
                    .Replace("${filter}", GetFilter(data, context, eventData))
                    .Replace("${cachedAccess}", cachedAccess)
                    .Replace("${methodArgs}", methodArgs)
                    .Replace(data, context, eventData);

                return new CodeGenFile(
                    Path.Combine("Events", "Systems", $"{data.Event(context, eventData)}EventSystem.cs"),
                    fileContent,
                    GetType().FullName
                );
            }).ToArray();

        string GetFilter(ComponentData data, string context, EventData eventData)
        {
            var filter = string.Empty;
            if (data.MemberData.Length == 0)
            {
                filter = eventData.EventType switch
                {
                    EventType.Added => $"entity.{data.PrefixedComponentName()}",
                    EventType.Removed => $"!entity.{data.PrefixedComponentName()}",
                    _ => filter
                };
            }
            else
            {
                filter = eventData.EventType switch
                {
                    EventType.Added => $"entity.has{data.Type.ToComponentName()}",
                    EventType.Removed => $"!entity.has{data.Type.ToComponentName()}",
                    _ => filter
                };
            }

            if (eventData.EventTarget == EventTarget.Self)
                filter += $" && entity.has{data.EventListener(context, eventData)}";

            return filter;
        }

        string GetMethodArgs(MemberData[] memberData) =>
            string.Join(", ", memberData.Select(info => $"component.{info.Name}"));
    }
}
