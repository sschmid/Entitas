using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class ContextsGenerator : ICodeGenerator
    {
        public string Name => "Contexts";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
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
            method => System.Attribute.IsDefined(method, typeof(Entitas.Plugins.Attributes.PostConstructorAttribute))
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

        const string ContextPropertyTemplate = @"    public ${ContextType} ${context} { get; set; }";
        const string ContextListTemplate = @"${context}";
        const string ContextAssignmentTemplate = @"        ${context} = new ${ContextType}();";

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var contexts = data
                .OfType<ContextData>()
                .Select(d => d.Name)
                .OrderBy(context => context)
                .ToArray();

            return new[]
            {
                new CodeGenFile(
                    "Contexts.cs",
                    Generate(contexts),
                    GetType().FullName)
            };
        }

        string Generate(string[] contexts)
        {
            var contextPropertiesList = string.Join("\n", contexts
                .Select(context => ContextPropertyTemplate.Replace(context)));

            var contextList = string.Join(", ", contexts
                .Select(context => ContextListTemplate.Replace(context)));

            var contextAssignmentsList = string.Join("\n", contexts
                .Select(context => ContextAssignmentTemplate.Replace(context)));

            return Template
                .Replace("${contextPropertiesList}", contextPropertiesList)
                .Replace("${contextList}", contextList)
                .Replace("${contextAssignmentsList}", contextAssignmentsList);
        }
    }
}
