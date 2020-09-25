using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins
{
    public class ComponentIndexGenerator : AbstractGenerator
    {
        public override string name
        {
            get { return "Component Index"; }
        }

        const string TEMPLATE = @"public static class ${ComponentName}ComponentIndex
{
    public static int Value;
}
";

        public override CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            return data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateIndex())
                .SelectMany(generate)
                .ToArray();
        }

        CodeGenFile[] generate(ComponentData data)
        {
            return data.GetContextNames()
                .Select(contextName => generate(contextName, data))
                .ToArray();
        }

        CodeGenFile generate(string contextName, ComponentData data)
        {
            var fileContent = TEMPLATE
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
