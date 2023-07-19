using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void CleanupSystem(SourceProductionContext spc, ComponentDeclaration component, string context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (component.CleanupMode == -1)
                return;

            if (!EntitasAnalyzerConfigOptions.ComponentCleanupSystems(optionsProvider, component.SyntaxTree))
                return;

            var cleanupSystemPrefix = component.CleanupMode == 0 ? "Remove" : "Destroy";
            var cleanupAction = component.CleanupMode == 0 ? $"Remove{component.Prefix}" : "Destroy";

            var contextPrefix = ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{cleanupSystemPrefix}{contextAwareComponentPrefix}CleanupSystem";
            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(CleanupSystem))) +
                NamespaceDeclaration(component.Namespace,
                    $$"""
                    public sealed class {{className}} : global::Entitas.ICleanupSystem
                    {
                        readonly global::Entitas.IGroup<global::{{contextPrefix}}.Entity> _group;
                        readonly global::System.Collections.Generic.List<global::{{contextPrefix}}.Entity> _buffer = new global::System.Collections.Generic.List<global::{{contextPrefix}}.Entity>();

                        public {{className}}(global::{{context}} context)
                        {
                            _group = context.GetGroup({{contextAwareComponentPrefix}}Matcher.{{component.Prefix}});
                        }

                        public void Cleanup()
                        {
                            foreach (var entity in _group.GetEntities(_buffer))
                            {
                                entity.{{cleanupAction}}();
                            }
                        }
                    }

                    """));
        }
    }
}
