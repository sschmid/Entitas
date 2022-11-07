using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class EventEntityApiGenerator : AbstractGenerator
    {
        public override string Name => "Event (Entity API)";

        const string TEMPLATE =
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
            .Where(d => d.IsEvent())
            .SelectMany(generate)
            .ToArray();

        CodeGenFile[] generate(ComponentData data) => data
            .GetContextNames()
            .SelectMany(contextName => generate(contextName, data))
            .ToArray();

        CodeGenFile[] generate(string contextName, ComponentData data) => data
            .GetEventData()
            .Select(eventData => new CodeGenFile(
                contextName + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                contextName + data.EventListener(contextName, eventData).AddComponentSuffix() + ".cs",
                TEMPLATE.Replace(data, contextName, eventData),
                GetType().FullName
            )).ToArray();
    }
}
