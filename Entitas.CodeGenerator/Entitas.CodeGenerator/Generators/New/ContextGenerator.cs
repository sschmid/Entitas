using System.IO;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextGenerator : ICodeGenerator {

        const string contextTemplate =
@"using Entitas;
using Entitas.Api;

public sealed partial class ${Context}Context : Context<${Context}Entity> {

    public ${Context}Context()
        : base(
            ${Lookup}.TOTAL_COMPONENTS,
            0,
            new ContextInfo(
                ""${Context} Context"",
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
                contextTemplate
                    .Replace("${Context}", contextName)
                    .Replace("${Lookup}", contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP),
                GetType().FullName
            );
        }
    }
}
