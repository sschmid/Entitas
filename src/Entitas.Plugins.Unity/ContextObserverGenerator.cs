using System.Collections.Generic;
using System.Linq;
using Jenny;

namespace Entitas.Plugins.Unity
{
    public class ContextObserverGenerator : ICodeGenerator
    {
        public string Name => "Context Observer";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string ContextsTemplate =
            @"public partial class Contexts
{
#if (!ENTITAS_DISABLE_VISUAL_DEBUGGING && UNITY_EDITOR)
    [Entitas.Plugins.Attributes.ContextsPostConstructor]
    public void InitializeContextObservers()
    {
        try
        {
${ContextObservers}
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError(e);
        }
    }

    public void CreateContextObserver(Entitas.IContext context)
    {
        if (UnityEngine.Application.isPlaying)
        {
            var observer = new Entitas.Unity.ContextObserver(context);
            UnityEngine.Object.DontDestroyOnLoad(observer.GameObject);
        }
    }
#endif
}
";

        const string ContextObserverTemplate = @"            CreateContextObserver(${Context.Name});";

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            var contextData = data
                .OfType<ContextData>()
                .OrderBy(d => d.Name);

            return new[]
            {
                new CodeGenFile(
                    "Contexts.cs",
                    GenerateContextsClass(contextData),
                    GetType().FullName)
            };
        }

        string GenerateContextsClass(IEnumerable<ContextData> data)
        {
            var contextObservers = string.Join("\n", data.Select(d => d
                .ReplacePlaceholders(ContextObserverTemplate)));

            return ContextsTemplate
                .Replace("${ContextObservers}", contextObservers);
        }
    }
}
