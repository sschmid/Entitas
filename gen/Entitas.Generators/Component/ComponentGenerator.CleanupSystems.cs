using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void CleanupSystems(SourceProductionContext spc, ContextInitializationMethodDeclaration method, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!EntitasAnalyzerConfigOptions.ComponentCleanupSystems(optionsProvider, method.SyntaxTree))
                return;

            spc.AddSource(
                GeneratedPath($"{method.ContextFullName}CleanupSystemsExtension"),
                GeneratedFileHeader(GeneratorSource(nameof(CleanupSystems))) +
                NamespaceDeclaration(method.ContextNamespace,
                    $$"""
                    public static class {{method.ContextName}}CleanupSystemsExtension
                    {
                        public static global::Entitas.Systems CreateCleanupSystems(this {{method.ContextName}} context)
                        {
                    {{AddCleanupSystems(method.Components, method.FullContextPrefix)}}
                        }
                    }

                    """));

            static string AddCleanupSystems(ImmutableArray<ComponentDeclaration> components, string contextPrefix)
            {
                return components.Length == 0
                    ? "        return null;"
                    : $$"""
                            var systems = new global::Entitas.Systems();
                    {{string.Join("\n", components.Select(component =>
                    {
                        var cleanupSystemPrefix = component.CleanupMode == 0 ? "Remove" : "Destroy";
                        return $"        systems.Add(new global::{CombinedNamespace(component.Namespace, cleanupSystemPrefix)}{component.ContextAwareComponentPrefix(contextPrefix)}CleanupSystem(context));";
                    }))}}
                            return systems;
                    """;
            }
        }
    }
}
