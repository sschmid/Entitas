using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ContextGenerator
    {
        static void Matcher(SourceProductionContext spc, ContextDeclaration context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!EntitasAnalyzerConfigOptions.ContextMatcher(optionsProvider, context.SyntaxTree))
                return;

            spc.AddSource(ContextAwarePath(context, "Matcher"),
                GeneratedFileHeader(GeneratorSource(nameof(Matcher))) +
                NamespaceDeclaration(context.FullContextPrefix,
                    """
                    public static class Matcher
                    {
                        public static global::Entitas.IAllOfMatcher<Entity> AllOf(params int[] indexes)
                        {
                            return global::Entitas.Matcher<Entity>.AllOf(indexes);
                        }

                        public static global::Entitas.IAllOfMatcher<Entity> AllOf(params global::Entitas.IMatcher<Entity>[] matchers)
                        {
                            return global::Entitas.Matcher<Entity>.AllOf(matchers);
                        }

                        public static global::Entitas.IAnyOfMatcher<Entity> AnyOf(params int[] indexes)
                        {
                            return global::Entitas.Matcher<Entity>.AnyOf(indexes);
                        }

                        public static global::Entitas.IAnyOfMatcher<Entity> AnyOf(params global::Entitas.IMatcher<Entity>[] matchers)
                        {
                            return global::Entitas.Matcher<Entity>.AnyOf(matchers);
                        }
                    }

                    """));
        }
    }
}
