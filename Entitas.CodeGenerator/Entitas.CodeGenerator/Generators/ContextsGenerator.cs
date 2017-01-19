using System;
using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextsGenerator : ICodeGenerator {

        const string CLASS_TEMPLATE = @"namespace Entitas {{

    public partial class Contexts {{
{0}
        public Context[] allContexts {{ get {{ return new [] {{ {1} }}; }} }}

{2}

        public void SetAllContexts() {{
{3}
        }}
    }}
}}
";

        const string CREATE_CONTEXT_TEMPLATE = @"
        public static Context Create{1}Context() {{
            return CreateContext(""{0}"", {2}.TotalComponents, {2}.componentNames, {2}.componentTypes);
        }}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var contextNames = data.Where(d => d.ContainsKey(ContextDataProvider.CONTEXT_NAME))
                                   .Select(d => d.GetContextName())
                                   .ToArray();

            var createContextMethods = contextNames.Aggregate(string.Empty, (acc, contextName) =>
                acc + string.Format(CREATE_CONTEXT_TEMPLATE, contextName, contextName, contextName + ComponentIndicesGenerator.COMPONENT_LOOKUP)
            );

            var allContextsList = string.Join(", ", contextNames.Select(contextName => contextName.LowercaseFirst()).ToArray());
            var contextFields = string.Join("\n", contextNames.Select(contextName =>
                "        public Context " + contextName.LowercaseFirst() + ";").ToArray());

            var setAllContexts = string.Join("\n", contextNames.Select(contextName =>
                "            " + contextName.LowercaseFirst() + " = Create" + contextName + "Context();").ToArray());

            return new[] { new CodeGenFile(
                "Contexts",
                string.Format(CLASS_TEMPLATE, createContextMethods, allContextsList, contextFields, setAllContexts),
                GetType().FullName
            )};
        }
    }
}
