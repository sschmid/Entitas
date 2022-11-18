using System.IO;
using System.Linq;
using Jenny;

namespace Entitas.Plugins
{
    public class ContextGenerator : ICodeGenerator
    {
        public string Name => "Context";
        public int Order => 0;
        public bool RunInDryMode => true;

        const string Template =
            @"public sealed partial class ${ContextType} : Entitas.Context<${EntityType}> {

    public ${ContextType}()
        : base(
            ${Lookup}.TotalComponents,
            0,
            new Entitas.ContextInfo(
                ""${Context}"",
                ${Lookup}.componentNames,
                ${Lookup}.componentTypes
            ),
            (entity) =>

#if (ENTITAS_FAST_AND_UNSAFE)
                new Entitas.UnsafeAERC(),
#else
                new Entitas.SafeAERC(entity),
#endif
            () => new ${EntityType}()
        ) {
    }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ContextData>()
            .Select(d => Generate(d))
            .ToArray();

        CodeGenFile Generate(ContextData data)
        {
            var context = data.Name;
            return new CodeGenFile(
                Path.Combine(context, $"{context.AddContextSuffix()}.cs"),
                Template.Replace(context),
                GetType().FullName
            );
        }
    }
}
