using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ContextGenerator
    {
        static void Context(SourceProductionContext spc, ContextDeclaration context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!EntitasAnalyzerConfigOptions.ContextContext(optionsProvider, context.SyntaxTree))
                return;

            spc.AddSource(GeneratedPath(context.FullName),
                GeneratedFileHeader(GeneratorSource(nameof(Context))) +
                NamespaceDeclaration(context.Namespace,
                    $$"""
                    public sealed partial class {{context.Name}} : global::Entitas.Context<{{context.ContextPrefix}}.Entity>
                    {
                        public static string[] ComponentNames;
                        public static global::System.Type[] ComponentTypes;

                        public {{context.Name}}()
                            : base(
                                ComponentTypes.Length,
                                0,
                                new global::Entitas.ContextInfo(
                                    "{{context.FullName}}",
                                    ComponentNames,
                                    ComponentTypes
                                ),
                                entity =>
                    #if (ENTITAS_FAST_AND_UNSAFE)
                                    new global::Entitas.UnsafeAERC(),
                    #else
                                    new global::Entitas.SafeAERC(entity),
                    #endif
                                () => new {{context.ContextPrefix}}.Entity()
                            ) { }
                    }

                    """));
        }
    }
}
