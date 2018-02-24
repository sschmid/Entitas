using System.Collections.Generic;
using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Serialization;

namespace Entitas.CodeGeneration.Plugins {

    public class EventSystemsGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Event (Systems)"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties { get { return _ignoreNamespacesConfig.defaultProperties; } }

        readonly IgnoreNamespacesConfig _ignoreNamespacesConfig = new IgnoreNamespacesConfig();

        const string SYSTEMS_TEMPLATE =
            @"public sealed class EventSystems : Feature {

    public EventSystems(Contexts contexts) {
${systems}
    }
}
";

        const string SYSTEM_ADD_TEMPLATE = @"        Add(new ${OptionalContextName}${ComponentName}${EventType}EventSystem(contexts)); // priority: ${priority}";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var orderedEventData = data
                .OfType<ComponentData>()
                .Where(d => d.IsEvent())
                .SelectMany(d => d.GetEventData().Select(eventData => new DataTuple { componentData = d, eventData = eventData }).ToArray())
                .OrderBy(tuple => tuple.eventData.priority)
                .ThenBy(tuple => tuple.componentData.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces))
                .ToArray();

            return new[] { generateEventSystems(orderedEventData) };
        }

        CodeGenFile generateEventSystems(DataTuple[] data) {
            var systems = generateSystemList(data);

            var fileContent = SYSTEMS_TEMPLATE
                .Replace("${systems}", systems);

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
            var optionalContextName = data.componentData.GetContextNames().Length > 1 ? contextName : string.Empty;
            var eventTypeSuffix = data.componentData.GetEventTypeSuffix(data.eventData);
            return SYSTEM_ADD_TEMPLATE
                .Replace("${OptionalContextName}", optionalContextName)
                .Replace("${ComponentName}", data.componentData.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces))
                .Replace("${EventType}", eventTypeSuffix)
                .Replace("${priority}", data.eventData.priority.ToString());
        }

        struct DataTuple {
            public ComponentData componentData;
            public EventData eventData;
        }
    }
}
