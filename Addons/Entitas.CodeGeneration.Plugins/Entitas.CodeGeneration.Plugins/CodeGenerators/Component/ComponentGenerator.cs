using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentGenerator : ICodeGenerator {

        public string name { get { return "Component"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string COMPONENT_TEMPLATE =
            @"[Entitas.CodeGeneration.Attributes.DontGenerate(false)]
public sealed class ${FullComponentName} : Entitas.IComponent {
    public ${Type} value;
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateComponent())
                .Select(generate)
                .ToArray();
        }

        CodeGenFile generate(ComponentData data) {
            var fullComponentName = data.GetTypeName().RemoveDots();
            return new CodeGenFile(
                "Components" + Path.DirectorySeparatorChar +
                fullComponentName + ".cs",
                COMPONENT_TEMPLATE
                    .Replace("${FullComponentName}", fullComponentName)
                    .Replace("${Type}", data.GetObjectTypeName()),
                GetType().FullName
            );
        }
    }
}
