using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void CleanupSystems(SourceProductionContext spc, ContextInitializationMethodDeclaration method, ImmutableArray<ComponentDeclaration> components, AnalyzerConfigOptionsProvider optionsProvider)
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
                            var systems = new global::Entitas.Systems();
                    {{AddCleanupSystems(components, method.FullContextPrefix)}}
                            return systems;
                        }
                    }

                    """));

            static string AddCleanupSystems(ImmutableArray<ComponentDeclaration> components, string contextPrefix)
            {
                return string.Join("\n", components.Select(component =>
                {
                    var cleanupSystemPrefix = component.CleanupMode == 0 ? "Remove" : "Destroy";
                    return $"        systems.Add(new global::{CombinedNamespace(component.Namespace, cleanupSystemPrefix)}{component.ContextAwareComponentPrefix(contextPrefix)}CleanupSystem(context));";
                }));
            }
        }
    }
}
