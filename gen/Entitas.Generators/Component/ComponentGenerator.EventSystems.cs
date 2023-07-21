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
            if (!EntitasAnalyzerConfigOptions.ComponentEventSystemsContextExtension(optionsProvider, method.SyntaxTree))
                return;

            spc.AddSource(
                GeneratedPath($"{method.ContextFullName}EventSystemsExtension"),
                GeneratedFileHeader(GeneratorSource(nameof(EventSystemsContextExtension))) +
                NamespaceDeclaration(method.ContextNamespace,
                    $$"""
                    public static class {{method.ContextName}}EventSystemsExtension
                    {
                        public static global::Entitas.Systems CreateEventSystems(this {{method.ContextName}} context)
                        {
                    {{AddEventSystems(method.Components, method.FullContextPrefix)}}
                        }
                    }

                    """));

            static string AddEventSystems(ImmutableArray<ComponentDeclaration> components, string contextPrefix)
            {
                var events = components
                    .SelectMany(component => component.Events.Select(@event => (Component: component, Event: @event)))
                    .ToImmutableArray();

                return events.Length == 0
                    ? "        return null;"
                    : $$"""
                            var systems = new global::Entitas.Systems();
                    {{string.Join("\n", events
                        .OrderBy(pair => pair.Event.Order)
                        .Select(pair =>
                        {
                            var (component, @event) = pair;
                            @event.ContextAware(ContextAware(contextPrefix));
                            return $"        systems.Add(new global::{CombinedNamespace(component.Namespace, @event.ContextAwareEvent)}EventSystem(context)); // Order: {@event.Order}";
                        }))}}
                            return systems;
                    """;
            }
        }
    }
}
