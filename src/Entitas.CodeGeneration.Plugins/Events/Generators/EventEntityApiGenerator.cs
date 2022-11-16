using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class EventEntityApiGenerator : AbstractGenerator
    {
        public override string Name => "Event (Entity API)";

        const string Template =
            @"public partial class ${EntityType} {

    public void Add${EventListener}(I${EventListener} value) {
        var listeners = has${EventListener}
            ? ${eventListener}.value
            : new System.Collections.Generic.List<I${EventListener}>();
        listeners.Add(value);
        Replace${EventListener}(listeners);
    }

    public void Remove${EventListener}(I${EventListener} value, bool removeComponentWhenEmpty = true) {
        var listeners = ${eventListener}.value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0) {
            Remove${EventListener}();
        } else {
            Replace${EventListener}(listeners);
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
            .Select(eventData => new CodeGenFile(
                Path.Combine(context, "Components", $"{context}{data.EventListener(context, eventData).AddComponentSuffix()}.cs"),
                Template.Replace(data, context, eventData),
                GetType().FullName
            )).ToArray();
    }
}
