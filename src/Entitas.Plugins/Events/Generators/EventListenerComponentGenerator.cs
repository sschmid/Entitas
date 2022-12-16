using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class EventListenerComponentGenerator : ICodeGenerator
    {
        public string Name => "Event (Listener Component)";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
            @"[Entitas.Plugins.Attributes.DontGenerate(false)]
public sealed class ${EventListenerComponent} : Entitas.IComponent {
    public System.Collections.Generic.List<I${EventListener}> Value;
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.EventData != null)
            .SelectMany(d => Generate(d))
            .ToArray();

        IEnumerable<CodeGenFile> Generate(ComponentData data) => data.EventData
            .Select(eventData => new CodeGenFile(
                Path.Combine("Events", "Components", $"{data.EventListener(data.Context, eventData).AddComponentSuffix()}.cs"),
                Template.Replace(data, data.Context, eventData),
                GetType().FullName));
    }
}
