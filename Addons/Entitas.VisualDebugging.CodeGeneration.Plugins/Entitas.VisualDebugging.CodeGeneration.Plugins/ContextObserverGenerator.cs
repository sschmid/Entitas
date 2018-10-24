using System.Linq;
using DesperateDevs.CodeGeneration;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Plugins;

namespace Entitas.VisualDebugging.CodeGeneration.Plugins {

    public class ContextObserverGenerator : ICodeGenerator {

        public string name { get { return "Context Observer"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        const string CONTEXTS_TEMPLATE =
@"public partial class Contexts {

#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    public void InitializeContextObservers() {
        try {
${contextObservers}
        } catch(System.Exception) {
        }
    }

    public void CreateContextObserver(Entitas.IContext context) {
        if (UnityEngine.Application.isPlaying) {
            var observer = new Entitas.VisualDebugging.Unity.ContextObserver(context);
            UnityEngine.Object.DontDestroyOnLoad(observer.gameObject);
        }
    }

#endif
}
";

        const string CONTEXT_OBSERVER_TEMPLATE = @"            CreateContextObserver(${contextName});";

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
            var contextObservers = string.Join("\n", contextNames
                .Select(contextName => CONTEXT_OBSERVER_TEMPLATE
                    .Replace("${contextName}", contextName.LowercaseFirst())
                ).ToArray());

            return CONTEXTS_TEMPLATE
                .Replace("${contextObservers}", contextObservers);
        }
    }
}
