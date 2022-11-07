using System.Linq;
using Jenny;

namespace Entitas.CodeGeneration.Plugins
{
    public class ContextsGenerator : ICodeGenerator
    {
        public string Name => "Contexts";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string TEMPLATE =
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

${contextPropertiesList}

    public Entitas.IContext[] allContexts { get { return new Entitas.IContext [] { ${contextList} }; } }

    public Contexts() {
${contextAssignmentsList}

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

        const string CONTEXT_PROPERTY_TEMPLATE = @"    public ${ContextType} ${contextName} { get; set; }";
        const string CONTEXT_LIST_TEMPLATE = @"${contextName}";
        const string CONTEXT_ASSIGNMENT_TEMPLATE = @"        ${contextName} = new ${ContextType}();";

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var contextNames = data
                .OfType<ContextData>()
                .Select(d => d.GetContextName())
                .OrderBy(contextName => contextName)
                .ToArray();

            return new[]
            {
                new CodeGenFile(
                    "Contexts.cs",
                    generate(contextNames),
                    GetType().FullName)
            };
        }

        string generate(string[] contextNames)
        {
            var contextPropertiesList = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_PROPERTY_TEMPLATE.Replace(contextName)));

            var contextList = string.Join(", ", contextNames
                .Select(contextName => CONTEXT_LIST_TEMPLATE.Replace(contextName)));

            var contextAssignmentsList = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_ASSIGNMENT_TEMPLATE.Replace(contextName)));

            return TEMPLATE
                .Replace("${contextPropertiesList}", contextPropertiesList)
                .Replace("${contextList}", contextList)
                .Replace("${contextAssignmentsList}", contextAssignmentsList);
        }
    }
}
