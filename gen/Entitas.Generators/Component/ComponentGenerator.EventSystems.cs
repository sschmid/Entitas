using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void EventSystemsContextExtension(SourceProductionContext spc, ContextInitializationMethodDeclaration method, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!EntitasAnalyzerConfigOptions.ComponentEventSystemsContextExtension(optionsProvider, method.Node.SyntaxTree))
                return;

            spc.AddSource(
                GeneratedPath($"{method.ContextFullName}EventSystemsExtension"),
                GeneratedFileHeader(GeneratorSource(nameof(EventSystemsContextExtension))) +
                $"using global::{method.FullContextPrefix};\n\n" +
                NamespaceDeclaration(method.ContextNamespace,
                    $$"""
                    public static class {{method.ContextName}}EventSystemsExtension
                    {
                        public static global::Entitas.Systems CreateEventSystems(this {{method.ContextName}} context)
                        {
                            var systems = new global::Entitas.Systems();
                    {{AddEventSystems(method.Components, method.FullContextPrefix)}}
                            return systems;
                        }
                    }

                    """));

            static string AddEventSystems(ImmutableArray<ComponentDeclaration> components, string contextPrefix)
            {
                return string.Join("\n", components
                    .SelectMany(component => component.Events.Select(@event => (Component: component, Event: @event)))
                    .OrderBy(pair => pair.Event.Order)
                    .Select(pair =>
                    {
                        var (component, @event) = pair;
                        @event.ContextAware(component.ContextAware(contextPrefix));
                        return $"        systems.Add(new {CombinedNamespace(component.Namespace, @event.ContextAwareEvent)}EventSystem(context)); // order: {@event.Order}";
                    }));
            }
        }
    }
}
