using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class EventListenerComponentGenerator : AbstractGenerator {

        public override string name { get { return "Event (Listener Component)"; } }

        const string TEMPLATE = @"[Entitas.CodeGeneration.Attributes.DontGenerate(false)]
public sealed class ${EventListenerComponent} : Entitas.IComponent
{
    public System.Collections.Generic.List<I${EventListener}> Value;
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
                .Select(eventData =>
                {
                    var fileContent = TEMPLATE
                        .Replace(data, contextName, eventData)
                        .WrapInNamespace(data.GetNamespace(), contextName);

                    return new CodeGenFile(
                        data.GetTypeName().ToFileName(contextName),
                        fileContent,
                        GetType().FullName
                    );
                }).ToArray();
        }
    }
}
