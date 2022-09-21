using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class EventListenerComponentGenerator : AbstractGenerator
    {
        public override string Name => "Event (Listener Component)";

        const string TEMPLATE =
            @"[Entitas.CodeGeneration.Attributes.DontGenerate(false)]
public sealed class ${EventListenerComponent} : Entitas.IComponent {
    public System.Collections.Generic.List<I${EventListener}> value;
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
                "Events" + Path.DirectorySeparatorChar +
                "Components" + Path.DirectorySeparatorChar +
                data.EventListener(contextName, eventData).AddComponentSuffix() + ".cs",
                TEMPLATE.Replace(data, contextName, eventData),
                GetType().FullName
            )).ToArray();
    }
}
