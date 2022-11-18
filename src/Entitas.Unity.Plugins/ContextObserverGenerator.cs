using System.Linq;
using Jenny;
using DesperateDevs.Extensions;
using Entitas.CodeGeneration.Plugins;

namespace Entitas.Unity.Plugins
{
    public class ContextObserverGenerator : ICodeGenerator
    {
        public string Name => "Context Observer";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string ContextsTemplate =
            @"public partial class Contexts {

#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)

    [Entitas.CodeGeneration.Attributes.PostConstructor]
    public void InitializeContextObservers() {
        try {
${contextObservers}
        } catch(System.Exception e) {
            UnityEngine.Debug.LogError(e);
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

        const string ContextObserverTemplate = @"            CreateContextObserver(${context});";

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
                    GenerateContextsClass(contexts),
                    GetType().FullName)
            };
        }

        string GenerateContextsClass(string[] contexts)
        {
            var contextObservers = string.Join("\n", contexts
                .Select(context => ContextObserverTemplate
                    .Replace("${context}", context.ToLowerFirst())));

            return ContextsTemplate
                .Replace("${contextObservers}", contextObservers);
        }
    }
}
