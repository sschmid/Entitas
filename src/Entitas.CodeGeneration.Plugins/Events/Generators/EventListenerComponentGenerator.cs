using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class EventListenerComponentGenerator : AbstractGenerator
    {
        public override string Name => "Event (Listener Component)";

        const string Template =
            @"[Entitas.CodeGeneration.Attributes.DontGenerate(false)]
public sealed class ${EventListenerComponent} : Entitas.IComponent {
    public System.Collections.Generic.List<I${EventListener}> value;
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
                Path.Combine("Events", "Components", $"{data.EventListener(context, eventData).AddComponentSuffix()}.cs"),
                Template.Replace(data, context, eventData),
                GetType().FullName
            )).ToArray();
    }
}
