using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class EventSystemsGenerator : AbstractGenerator {

        public override string name { get { return "Event (Systems)"; } }

        const string TEMPLATE =
            @"public sealed class EventSystems : Feature {

    public EventSystems(Contexts contexts) {
${systemsList}
    }
}
";

        const string SYSTEM_ADD_TEMPLATE = @"        Add(new ${Event}EventSystem(contexts)); // priority: ${priority}";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var orderedEventData = data
                .OfType<ComponentData>()
                .Where(d => d.IsEvent())
                .SelectMany(d => d.GetEventData().Select(eventData => new DataTuple { componentData = d, eventData = eventData }).ToArray())
                .OrderBy(tuple => tuple.eventData.priority)
                .ThenBy(tuple => tuple.componentData.ComponentName())
                .ToArray();

            return new[] { generate(orderedEventData) };
        }

        CodeGenFile generate(DataTuple[] data) {

            var fileContent = TEMPLATE.Replace("${systemsList}", generateSystemList(data));

            return new CodeGenFile(
                "Events" + Path.DirectorySeparatorChar +
                "EventSystems.cs",
                fileContent,
                GetType().FullName
            );
        }

        string generateSystemList(DataTuple[] data) {
            return string.Join("\n", data
                .SelectMany(generateSystemListForData)
                .ToArray());
        }

        string[] generateSystemListForData(DataTuple data) {
            return data.componentData.GetContextNames()
                .Select(contextName => generateAddSystem(contextName, data))
                .ToArray();
        }

        string generateAddSystem(string contextName, DataTuple data) {
            return SYSTEM_ADD_TEMPLATE
                .Replace(data.componentData, contextName, data.eventData)
                .Replace("${priority}", data.eventData.priority.ToString())
                .Replace("${Event}", data.componentData.Event(contextName, data.eventData));
        }

        struct DataTuple {
            public ComponentData componentData;
            public EventData eventData;
        }
    }
}
