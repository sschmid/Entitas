using System.Linq;
using Entitas.CodeGenerator;

namespace Entitas.Unity.VisualDebugging {

    public class ContextObserverGenerator : ICodeGenerator {

        public string name { get { return "Context Observer"; } }
#if UNITY_EDITOR
        public bool isEnabledByDefault { get { return true; } }
#else
        public bool isEnabledByDefault { get { return false; } }
#endif

        const string CONTEXTS_TEMPLATE =
@"public partial class Contexts : Entitas.IContexts {

    public void CreateContextObserver(Entitas.IContext context) {
#if(!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
        if(UnityEngine.Application.isPlaying) {
            var observer = new Entitas.Unity.VisualDebugging.ContextObserver(context);
            UnityEngine.Object.DontDestroyOnLoad(observer.gameObject);
        }
#endif
    }

    [Entitas.CodeGenerator.Api.PostConstructor]
    public void InitializeContexObservers() {
${contextObservers}
    }
}
";

        const string CONTEXT_OBSERVER_TEMPLATE = @"        CreateContextObserver(${contextName});";

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
            var contextObservers = string.Join("\n", contextNames
                                               .Select(contextName => CONTEXT_OBSERVER_TEMPLATE
                           .Replace("${contextName}", contextName.LowercaseFirst())
                       ).ToArray());

            return CONTEXTS_TEMPLATE
                .Replace("${contextObservers}", contextObservers);
        }
    }
}
