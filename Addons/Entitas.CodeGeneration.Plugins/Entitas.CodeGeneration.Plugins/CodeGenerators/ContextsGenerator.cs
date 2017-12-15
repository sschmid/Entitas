using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextsGenerator : ICodeGenerator {

        public string name { get { return "Contexts"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string CONTEXTS_TEMPLATE =
@"public partial class Contexts : Entitas.IContexts {

    public static Contexts sharedInstance {
        get {
            if (_sharedInstance == null) {
                _sharedInstance = new Contexts();
            }

            return _sharedInstance;
        }
        set { _sharedInstance = value; }
    }

    static Contexts _sharedInstance;

${contextProperties}

    public Entitas.IContext[] allContexts { get { return new Entitas.IContext [] { ${contextList} }; } }

    public Contexts() {
${contextAssignments}

        var postConstructors = System.Linq.Enumerable.Where(
            GetType().GetMethods(),
            method => System.Attribute.IsDefined(method, typeof(Entitas.CodeGeneration.Attributes.PostConstructorAttribute))
        );

        foreach (var postConstructor in postConstructors) {
            postConstructor.Invoke(this, null);
        }
    }

    public void Reset() {
        var contexts = allContexts;
        for (int i = 0; i < contexts.Length; i++) {
            contexts[i].Reset();
        }
    }
}
";

        const string CONTEXT_PROPERTY_TEMPLATE = @"    public ${ContextName}Context ${contextName} { get; set; }";
        const string CONTEXT_LIST_TEMPLATE = @"${contextName}";
        const string CONTEXT_ASSIGNMENT_TEMPLATE = @"        ${contextName} = new ${ContextName}Context();";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var contextNames = data
                .OfType<ContextData>()
                .Select(d => d.GetContextName())
                .OrderBy(contextName => contextName)
                .ToArray();

            return new[] {
                new CodeGenFile(
                    "Contexts.cs",
                    generateContextsClass(contextNames),
                    GetType().FullName)
            };
        }

        string generateContextsClass(string[] contextNames) {
            var contextProperties = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_PROPERTY_TEMPLATE
                    .Replace("${ContextName}", contextName)
                    .Replace("${contextName}", contextName.LowercaseFirst())
                ).ToArray());

            var contextList = string.Join(", ", contextNames
                .Select(contextName => CONTEXT_LIST_TEMPLATE
                    .Replace("${contextName}", contextName.LowercaseFirst())
                ).ToArray());

            var contextAssignments = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_ASSIGNMENT_TEMPLATE
                    .Replace("${ContextName}", contextName)
                    .Replace("${contextName}", contextName.LowercaseFirst())
                ).ToArray());

            return CONTEXTS_TEMPLATE
                .Replace("${contextProperties}", contextProperties)
                .Replace("${contextList}", contextList)
                .Replace("${contextAssignments}", contextAssignments);
        }
    }
}
