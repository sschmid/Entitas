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

        const string SYSTEM_ADD_TEMPLATE = @"        Add(new ${ContextName}${ComponentName}EventSystem(contexts)); // priority: ${priority}";

        public void Configure(Preferences preferences) {
            _ignoreNamespacesConfig.Configure(preferences);
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var orderedEventData = data
                .OfType<ComponentData>()
                .Where(d => d.IsEvent())
                .OrderBy(d => d.GetPriority())
                .ThenBy(d => d.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces))
                .ToArray();

            return new[] { generateEventSystems(orderedEventData) };
        }

        CodeGenFile generateEventSystems(ComponentData[] data) {
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

        string generateSystemList(ComponentData[] data) {
            return string.Join("\n", data
                .SelectMany(generateSystemListForData)
                .ToArray());
        }

        string[] generateSystemListForData(ComponentData data) {
            return data.GetContextNames()
                .Select(contextName => generateAddSystem(contextName, data))
                .ToArray();
        }

        string generateAddSystem(string contextName, ComponentData data) {
            return SYSTEM_ADD_TEMPLATE
                .Replace("${ContextName}", contextName)
                .Replace("${ComponentName}", data.GetTypeName().ToComponentName(_ignoreNamespacesConfig.ignoreNamespaces))
                .Replace("${priority}", data.GetPriority().ToString());
        }
    }
}
