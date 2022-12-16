using System.Collections.Generic;
using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class EventListenerInterfaceGenerator : ICodeGenerator
    {
        public string Name => "Event (Listener Interface)";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
            @"public interface I${EventListener} {
    void On${EventComponentName}${EventType}(${Context}Entity entity${methodParameters});
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ComponentData>()
            .Where(d => d.EventData != null)
            .SelectMany(d => Generate(d))
            .ToArray();

        IEnumerable<CodeGenFile> Generate(ComponentData data) => data.EventData
            .Select(eventData =>
            {
                var memberData = data.MemberData;
                if (memberData.Length == 0)
                    memberData = new[] {new MemberData("bool", data.PrefixedComponentName())};

                return new CodeGenFile(
                    Path.Combine("Events", "Interfaces", $"I{data.EventListener(data.Context, eventData)}.cs"),
                    Template
                        .Replace("${methodParameters}", data.GetEventMethodArgs(eventData, $", {memberData.GetMethodParameters(false)}"))
                        .Replace(data, data.Context, eventData),
                    GetType().FullName
                );
            });
    }
}
