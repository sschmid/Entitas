using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class EventEntityApiGenerator : ICodeGenerator
    {
        public string Name => "Event (Entity API)";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
            @"public partial class ${EntityType} {

    public void Add${EventListener}(I${EventListener} value) {
        var listeners = has${EventListener}
            ? ${eventListener}.Value
            : new System.Collections.Generic.List<I${EventListener}>();
        listeners.Add(value);
        Replace${EventListener}(listeners);
    }

    public void Remove${EventListener}(I${EventListener} value, bool removeComponentWhenEmpty = true) {
        var listeners = ${eventListener}.Value;
        listeners.Remove(value);
        if (removeComponentWhenEmpty && listeners.Count == 0) {
            Remove${EventListener}();
        } else {
            Replace${EventListener}(listeners);
        }
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.EventData != null)
            .SelectMany(d => Generate(d))
            .ToArray();

        IEnumerable<CodeGenFile> Generate(ComponentData data) => data.EventData
            .Select(eventData => new CodeGenFile(
                Path.Combine(data.Context, "Components", $"{data.Context}{data.EventListener(data.Context, eventData).AddComponentSuffix()}.cs"),
                Template.Replace(data, data.Context, eventData),
                GetType().FullName));
    }
}
