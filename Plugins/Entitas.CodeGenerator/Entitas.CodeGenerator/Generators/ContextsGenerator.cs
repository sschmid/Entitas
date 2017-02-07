using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextsGenerator : ICodeGenerator {

        public bool IsEnabledByDefault { get { return true; } }

        const string CONTEXTS_TEMPLATE =
@"using Entitas;
            
public partial class Contexts : IContexts {

    public static Contexts sharedInstance {
        get {
            if(_sharedInstance == null) {
                _sharedInstance = new Contexts();
            }

            return _sharedInstance;
        }
        set { _sharedInstance = value; }
    }

    static Contexts _sharedInstance;

    public static void CreateContextObserver(IContext context) {
#if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        if(UnityEngine.Application.isPlaying) {
            var observer = new Entitas.Unity.VisualDebugging.ContextObserver(context);
            UnityEngine.Object.DontDestroyOnLoad(observer.gameObject);
        }
#endif
    }

${contextProperties}

    public IContext[] allContexts { get { return new IContext [] { ${contextList} }; } }

    public virtual void SetAllContexts() {
${contextAssignments}

${contextObservers}
    }
}
";

        const string CONTEXT_PROPERTY_TEMPLATE = @"    public ${Context}Context ${context} { get; set; }";
        const string CONTEXT_LIST_TEMPLATE = @"${context}";
        const string CONTEXT_ASSIGNMENT_TEMPLATE = @"        ${context} = new ${Context}Context();";
        const string CONTEXT_OBSERVER_TEMPLATE = @"        CreateContextObserver(${context});";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var contextNames = data
                .OfType<ContextData>()
                .Select(d => d.GetContextName())
                .OrderBy(contextName => contextName)
                .ToArray();

            return new[] { new CodeGenFile(
                "Contexts.cs",
                generateContextsClass(contextNames),
                GetType().FullName)
            };
        }

        string generateContextsClass(string[] contextNames) {
            var contextProperties = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_PROPERTY_TEMPLATE
                        .Replace("${Context}", contextName)
                        .Replace("${context}", contextName.LowercaseFirst())
                       ).ToArray());

            var contextList = string.Join(", ", contextNames
                .Select(contextName => CONTEXT_LIST_TEMPLATE
                        .Replace("${context}", contextName.LowercaseFirst())
                       ).ToArray());

            var contextAssignments = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_ASSIGNMENT_TEMPLATE
                        .Replace("${Context}", contextName)
                        .Replace("${context}", contextName.LowercaseFirst())
                       ).ToArray());

            var contextObservers = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_OBSERVER_TEMPLATE
                        .Replace("${context}", contextName.LowercaseFirst())
                       ).ToArray());

            return CONTEXTS_TEMPLATE
                .Replace("${contextProperties}", contextProperties)
                .Replace("${contextList}", contextList)
                .Replace("${contextAssignments}", contextAssignments)
                .Replace("${contextObservers}", contextObservers);
        }
    }
}
