using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ContextGenerator
    {
        static void Entity(SourceProductionContext spc, ContextDeclaration context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!EntitasAnalyzerConfigOptions.ContextEntity(optionsProvider, context.SyntaxTree))
                return;

            spc.AddSource(ContextAwarePath(context, "Entity"),
                GeneratedFileHeader(GeneratorSource(nameof(Entity))) +
                NamespaceDeclaration(context.FullContextPrefix,
                    """
                    public sealed class Entity : global::Entitas.Entity { }

                    """));
        }
    }
}
