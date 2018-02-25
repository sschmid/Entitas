using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class EventListenertInterfaceGenerator : AbstractComponentGenerator {

        public override string name { get { return "Event (Listener Interface)"; } }

        const string TEMPLATE =
            @"public interface I${OptionalContextName}${ComponentName}${EventType}Listener {
    void On${ComponentName}${EventType}(${ContextName}Entity entity${methodParameters});
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
                .Select(eventData => {
                    var memberData = data.GetMemberData();
                    if (memberData.Length == 0) {
                        memberData = new[] { new MemberData("bool", data.PrefixedComponentName()) };
                    }

                    var fileContent = TEMPLATE
                        .Replace("${methodParameters}", data.GetEventMethodArgs(eventData, ", " + getMethodParameters(memberData)))
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

        string getMethodParameters(MemberData[] memberData) {
            return string.Join(", ", memberData
                .Select(info => info.type + " " + info.name)
                .ToArray()
            );
        }
    }
}
