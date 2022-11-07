using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class EventListenerInterfaceGenerator : AbstractGenerator
    {
        public override string Name => "Event (Listener Interface)";

        const string TEMPLATE =
            @"public interface I${EventListener} {
    void On${EventComponentName}${EventType}(${ContextName}Entity entity${methodParameters});
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
            .Select(eventData =>
            {
                var memberData = data.GetMemberData();
                if (memberData.Length == 0)
                {
                    memberData = new[] {new MemberData("bool", data.PrefixedComponentName())};
                }

                var fileContent = TEMPLATE
                    .Replace("${methodParameters}", data.GetEventMethodArgs(eventData, $", {memberData.GetMethodParameters(false)}"))
                    .Replace(data, contextName, eventData);

                return new CodeGenFile(
                    "Events" + Path.DirectorySeparatorChar +
                    "Interfaces" + Path.DirectorySeparatorChar +
                    "I" + data.EventListener(contextName, eventData) + ".cs",
                    fileContent,
                    GetType().FullName
                );
            }).ToArray();
    }
}
