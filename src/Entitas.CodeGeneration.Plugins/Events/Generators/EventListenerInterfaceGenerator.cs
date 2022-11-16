using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class EventListenerInterfaceGenerator : AbstractGenerator
    {
        public override string Name => "Event (Listener Interface)";

        const string Template =
            @"public interface I${EventListener} {
    void On${EventComponentName}${EventType}(${Context}Entity entity${methodParameters});
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.IsEvent)
            .SelectMany(d => Generate(d))
            .ToArray();

        IEnumerable<CodeGenFile> Generate(ComponentData data) =>
            data.Contexts.SelectMany(context => Generate(context, data));

        CodeGenFile[] Generate(string context, ComponentData data) => data
            .EventData
            .Select(eventData =>
            {
                var memberData = data.MemberData;
                if (memberData.Length == 0)
                    memberData = new[] {new MemberData("bool", data.PrefixedComponentName())};
                return new CodeGenFile(
                    Path.Combine("Events", "Interfaces", $"I{data.EventListener(context, eventData)}.cs"),
                    Template
                        .Replace("${methodParameters}", data.GetEventMethodArgs(eventData, $", {memberData.GetMethodParameters(false)}"))
                        .Replace(data, context, eventData),
                    GetType().FullName
                );
            }).ToArray();
    }
}
