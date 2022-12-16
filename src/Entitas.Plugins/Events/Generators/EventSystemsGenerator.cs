using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class EventSystemsGenerator : ICodeGenerator
    {
        public string Name => "Event (Systems)";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
            @"public sealed class ${Context.Name}EventSystems : Feature
{
    public ${Context.Name}EventSystems(Contexts contexts) {
${SystemsList}
    }
}
";

        const string SystemAddTemplate = @"        Add(new ${Event}EventSystem(contexts)); // Event.Order: ${Order}";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.EventData != null)
            .GroupBy(d => d.Context)
            .ToDictionary(
                group => group.Key,
                group => group
                    .SelectMany(c => c.EventData.Select(e => new DataTuple {ComponentData = c, EventData = e}))
                    .OrderBy(tuple => tuple.EventData.Order)
                    .ThenBy(tuple => tuple.ComponentData.Name))
            .Select(kvp => GenerateSystem(kvp.Key, kvp.Value))
            .ToArray();

        CodeGenFile GenerateSystem(string context, IEnumerable<DataTuple> data) => new CodeGenFile(
            Path.Combine("Events", $"{context}EventSystems.cs"),
            Template
                .Replace("${SystemsList}", GenerateSystemList(data))
                .Replace(context),
            GetType().FullName
        );

        string GenerateSystemList(IEnumerable<DataTuple> data) =>
            string.Join("\n", data.Select(tuple => GenerateSystemList(tuple)));

        string GenerateSystemList(DataTuple data) => data.ComponentData
            .ReplacePlaceholders(SystemAddTemplate)
            .Replace(data.ComponentData, data.ComponentData.Context, data.EventData)
            .Replace("${Order}", data.EventData.Order.ToString())
            .Replace("${Event}", data.ComponentData.Event(data.ComponentData.Context, data.EventData));
    }

    struct DataTuple
    {
        public ComponentData ComponentData;
        public EventData EventData;
    }
}
