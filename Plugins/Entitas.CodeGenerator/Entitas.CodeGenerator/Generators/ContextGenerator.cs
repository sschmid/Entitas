using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextGenerator : ICodeGenerator {

        const string CONTEXT_TEMPLATE =
@"using Entitas;

public sealed partial class ${Context}Context : Context<${Context}Entity> {

    public ${Context}Context()
        : base(
            ${Lookup}.TotalComponents,
            0,
            new ContextInfo(
                ""${Context}"",
                ${Lookup}.componentNames,
                ${Lookup}.componentTypes
            )
        ) {
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(d => generateContextClass(d))
                .ToArray();
        }

        CodeGenFile generateContextClass(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Context.cs",
                CONTEXT_TEMPLATE
                    .Replace("${Context}", contextName)
                    .Replace("${Lookup}", contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP),
                GetType().FullName
            );
        }
    }
}
