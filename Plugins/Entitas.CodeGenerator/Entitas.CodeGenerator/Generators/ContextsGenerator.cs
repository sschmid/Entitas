using System.Linq;

namespace Entitas.CodeGenerator {

    public class ContextsGenerator : ICodeGenerator {

        const string CONTEXTS_TEMPLATE =
@"using Entitas;
            
public partial class Contexts {

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

    public static IContext<TEntity> CreateContext<TEntity>(string name,
                                                           int totalComponents,
                                                           string[] componentNames,
                                                           System.Type[] componentTypes)
        where TEntity : class, IEntity, new() {
        var context = new Context<TEntity>(totalComponents, 0, new ContextInfo(
            name, componentNames, componentTypes));

#if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        if(UnityEngine.Application.isPlaying) {
            var observer = new Entitas.Unity.VisualDebugging.ContextObserver(context);
            UnityEngine.Object.DontDestroyOnLoad(observer.gameObject);
        }
#endif

        return context;
    }

${contextMethodTemplate}
${contextProperties}

    public IContext[] allContexts { get { return new IContext [] { ${contextList} }; } }

    public virtual void SetAllContexts() {
${contextAssignments}
    }
}
";

        const string CONTEXT_METHOD_TEMPLATE =
@"    public static IContext<${Context}Entity> Create${Context}Context() {
        return CreateContext<${Context}Entity>(""${Context}"", ${Lookup}.TotalComponents, ${Lookup}.componentNames, ${Lookup}.componentTypes);
    }
";

        const string CONTEXT_PROPERTY_TEMPLATE = @"    public IContext<${Context}Entity> ${context} { get; set; }";
        const string CONTEXT_LIST_TEMPLATE = @"${context}";
        const string CONTEXT_ASSIGNMENT_TEMPLATE = @"        ${context} = Create${Context}Context();";

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
            var contextMethods = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_METHOD_TEMPLATE
                        .Replace("${Context}", contextName)
                        .Replace("${Lookup}", contextName + ComponentsLookupGenerator.COMPONENTS_LOOKUP)
                       ).ToArray());

            var contextProperties = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_PROPERTY_TEMPLATE
                        .Replace("${Context}", contextName)
                        .Replace("${context}", contextName.LowercaseFirst())
                       ).ToArray());

            var contextList = string.Join(", ", contextNames
                .Select(contextName => CONTEXT_LIST_TEMPLATE.Replace("${context}", contextName.LowercaseFirst())).ToArray());

            var contextAssignments = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_ASSIGNMENT_TEMPLATE
                        .Replace("${Context}", contextName)
                        .Replace("${context}", contextName.LowercaseFirst())
                       ).ToArray());

            return CONTEXTS_TEMPLATE
                .Replace("${contextMethodTemplate}", contextMethods)
                .Replace("${contextProperties}", contextProperties)
                .Replace("${contextList}", contextList)
                .Replace("${contextAssignments}", contextAssignments);
        }
    }
}
