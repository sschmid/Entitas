using Microsoft.CodeAnalysis;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void ComponentIndex(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            var contextPrefix = component.ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}ComponentIndex";
            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                $"using global::{contextPrefix};\n\n" +
                NamespaceDeclaration(component.Namespace,
                    $$"""
                    public static class {{className}}
                    {
                        public static ComponentIndex Index;
                    }

                    """));
        }
    }
}
