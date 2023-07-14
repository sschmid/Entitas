using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void EventSystemsContextExtension(SourceProductionContext spc, ContextInitializationMethodDeclaration method)
        {
            spc.AddSource(
                GeneratedPath($"{method.ContextFullName}.EventSystemsExtension"),
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
                        var contextAware = component.ContextAware(contextPrefix);
                        var eventStrings = new EventStrings(@event, component.Prefix, contextAware);
                        return $"        systems.Add(new {CombinedNamespace(component.Namespace, eventStrings.ContextAwareEvent)}EventSystem(context)); // order: {@event.Order}";
                    }));
            }
        }
    }
}
