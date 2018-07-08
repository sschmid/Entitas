using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class EventListenerComponentGenerator : AbstractGenerator {

        public override string name { get { return "Event (Listener Component)"; } }

        const string TEMPLATE =
            @"[Entitas.CodeGeneration.Attributes.DontGenerate(false)]
public sealed class ${EventListenerComponent} : Entitas.IComponent {
    public System.Collections.Generic.List<I${EventListener}> value;
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
                .Select(eventData => new CodeGenFile(
                    "Events" + Path.DirectorySeparatorChar +
                    "Components" + Path.DirectorySeparatorChar +
                    data.EventListener(contextName, eventData).AddComponentSuffix() + ".cs",
                    TEMPLATE.Replace(data, contextName, eventData),
                    GetType().FullName
                )).ToArray();
        }
    }
}
