using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class EventSystemsGenerator : AbstractGenerator
    {
        public override string Name => "Event (Systems)";

        const string Template =
            @"public sealed class ${Context}EventSystems : Feature {

    public ${Context}EventSystems(Contexts contexts) {
${systemsList}
    }
}
";

        const string SystemAddTemplate = @"        Add(new ${Event}EventSystem(contexts)); // Event.Order: ${order}";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) => Generate(data
            .OfType<ComponentData>()
            .Where(d => d.IsEvent)
            .ToArray());

        CodeGenFile[] Generate(ComponentData[] data)
        {
            var contextToComponentData = data
                .Aggregate(new Dictionary<string, List<ComponentData>>(), (dict, d) =>
                {
                    var contexts = d.Contexts;
                    foreach (var context in contexts)
                    {
                        if (!dict.ContainsKey(context))
                            dict.Add(context, new List<ComponentData>());

                        dict[context].Add(d);
                    }

                    return dict;
                });

            var contextToDataTuple = new Dictionary<string, List<DataTuple>>();
            foreach (var key in contextToComponentData.Keys.ToArray())
            {
                var orderedEventData = contextToComponentData[key]
                    .SelectMany(d => d.EventData.Select(eventData => new DataTuple {ComponentData = d, EventData = eventData}).ToArray())
                    .OrderBy(tuple => tuple.EventData.Order)
                    .ThenBy(tuple => tuple.ComponentData.Name)
                    .ToList();

                contextToDataTuple.Add(key, orderedEventData);
            }

            return Generate(contextToDataTuple);
        }

        CodeGenFile[] Generate(Dictionary<string, List<DataTuple>> contextToDataTuple) => contextToDataTuple
            .Select(kvp => GenerateSystem(kvp.Key, kvp.Value.ToArray())).ToArray();

        CodeGenFile GenerateSystem(string context, DataTuple[] data) => new CodeGenFile(
            Path.Combine("Events", $"{context}EventSystems.cs"),
            Template
                .Replace("${systemsList}", GenerateSystemList(context, data))
                .Replace(context),
            GetType().FullName
        );

        string GenerateSystemList(string context, DataTuple[] data) =>
            string.Join("\n", data.SelectMany(tuple => GenerateSystemListForData(context, tuple)));

        string[] GenerateSystemListForData(string context, DataTuple data) => data.ComponentData
            .Contexts
            .Where(ctx => ctx == context)
            .Select(ctx => GenerateAddSystem(ctx, data))
            .ToArray();

        string GenerateAddSystem(string context, DataTuple data) => SystemAddTemplate
            .Replace(data.ComponentData, context, data.EventData)
            .Replace("${order}", data.EventData.Order.ToString())
            .Replace("${Event}", data.ComponentData.Event(context, data.EventData));

        struct DataTuple
        {
            public ComponentData ComponentData;
            public EventData EventData;
        }
    }
}
