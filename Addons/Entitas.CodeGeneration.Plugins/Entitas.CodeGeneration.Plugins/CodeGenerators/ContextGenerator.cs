using System.IO;
using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextGenerator : ICodeGenerator {

        public string name { get { return "Context"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string CONTEXT_TEMPLATE =
@"public sealed partial class ${ContextName}Context : Entitas.Context<${ContextName}Entity> {

    public ${ContextName}Context()
        : base(
            ${Lookup}.TotalComponents,
            0,
            new Entitas.ContextInfo(
                ""${ContextName}"",
                ${Lookup}.componentNames,
                ${Lookup}.componentTypes
            ),
            (entity) =>

#if (ENTITAS_FAST_AND_UNSAFE)
                new Entitas.UnsafeAERC()
#else
                new Entitas.SafeAERC(entity)
#endif

        ) {
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            return data
                .OfType<ContextData>()
                .Select(generateContextClass)
                .ToArray();
        }

        CodeGenFile generateContextClass(ContextData data) {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + "Context.cs",
                CONTEXT_TEMPLATE
                    .Replace("${ContextName}", contextName)
                    .Replace("${Lookup}", contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP),
                GetType().FullName
            );
        }
    }
}
