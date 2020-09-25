using System.Linq;
using DesperateDevs.CodeGeneration;

namespace Entitas.CodeGeneration.Plugins
{
    public class ContextGenerator : ICodeGenerator
    {
        public string name
        {
            get { return "Context"; }
        }

        public int priority
        {
            get { return 0; }
        }

        public bool runInDryMode
        {
            get { return true; }
        }

        const string TEMPLATE =
            @"public class ${ContextType} : Entitas.Context<${EntityType}> {

    public ${ContextType}()
        : base(
            999,
            0,
            new Entitas.ContextInfo(
                ""${ContextName}"",
                null,
                null
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

        public CodeGenFile[] Generate(CodeGeneratorData[] data)
        {
            return data
                .OfType<ContextData>()
                .Select(generate)
                .ToArray();
        }

        CodeGenFile generate(ContextData data)
        {
            var contextName = data.GetContextName();
            return new CodeGenFile(
                "Context".ToFileName(contextName),
                TEMPLATE.Replace(contextName),
                GetType().FullName
            );
        }
    }
}
