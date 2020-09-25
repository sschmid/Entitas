using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentMatcherApiGenerator : AbstractGenerator {

        public override string name { get { return "Component Matcher"; } }

        const string TEMPLATE = @"public sealed class ${ComponentName}Matcher : Entitas.Matcher<${ContextName}Entity>
{
    public static ${ComponentName}Matcher Instance => _instance ?? (_instance = new ${ComponentName}Matcher());

    static ${ComponentName}Matcher _instance;

    ${ComponentName}Matcher()
    {
        _allOfIndices = distinctIndices(new[] {${ComponentName}ComponentIndex.Value});
    }
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateIndex())
                .SelectMany(generate)
                .ToArray();
        }

        CodeGenFile[] generate(ComponentData data) {
            return data.GetContextNames()
                .Select(context => generate(context, data))
                .ToArray();
        }

        CodeGenFile generate(string contextName, ComponentData data) {
            var fileContent = TEMPLATE
                .Replace("${componentNames}", contextName + CodeGeneratorExtentions.LOOKUP + ".componentNames")
                .Replace(data, contextName)
                .WrapInNamespace(data.GetNamespace(), contextName);

            return new CodeGenFile(
                data.GetTypeName().ToFileName(contextName),
                fileContent,
                GetType().FullName
            );
        }
    }
}
