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
            @"public sealed partial class ${Context.Type} : Entitas.Context<${Context.Entity.Type}>
{
    public ${Context.Type}() : base(
        ${Lookup}.TotalComponents,
        0,
        new Entitas.ContextInfo(
            ""${Context.Name}"",
            ${Lookup}.ComponentNames,
            ${Lookup}.ComponentTypes
        ),
        entity =>
#if (ENTITAS_FAST_AND_UNSAFE)
            new Entitas.UnsafeAERC(),
#else
            new Entitas.SafeAERC(entity),
#endif
        () => new ${Context.Entity.Type}()
    ) { }
}
";

        public CodeGenFile[] Generate(CodeGeneratorData[] data) => data
            .OfType<ContextData>()
            .Select(d => Generate(d))
            .ToArray();

        CodeGenFile Generate(ContextData data) => new CodeGenFile(
            Path.Combine(data.Name, $"{data.Type}.cs"),
            data.ReplacePlaceholders(Template)
                // TODO remove Lookup
                .Replace(data.Name),
            GetType().FullName
        );
    }
}
