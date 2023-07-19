using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void ComponentIndex(SourceProductionContext spc, ComponentDeclaration component, string context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!EntitasAnalyzerConfigOptions.ComponentComponentIndex(optionsProvider, component.SyntaxTree))
                return;

            var contextPrefix = ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}ComponentIndex";
            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                NamespaceDeclaration(component.Namespace,
                    $$"""
                    public static class {{className}}
                    {
                        public static global::{{contextPrefix}}.ComponentIndex Index;
                    }

                    """));
        }
    }
}
