using Microsoft.CodeAnalysis;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void Events(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            if (component.Events.Length == 0)
                return;

            var contextPrefix = component.ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            foreach (var @event in component.Events)
            {
                var eventStrings = new EventStrings(@event, component.ComponentPrefix, contextAwareComponentPrefix);
                spc.AddSource(
                    GeneratedPath(CombinedNamespace(component.Namespace, eventStrings.EventListenerComponent)),
                    GeneratedFileHeader(GeneratorSource(nameof(Events))) +
                    $"using global::{contextPrefix};\n\n" +
                    NamespaceDeclaration(component.Namespace,
                        $$"""
                        public interface {{eventStrings.EventListenerInterface}}
                        {
                            void {{eventStrings.EventMethod}}(Entity entity, {{ComponentMethodParams(component)}});
                        }

                        public sealed class {{eventStrings.EventListenerComponent}} : global::Entitas.IComponent
                        {
                            public global::System.Collections.Generic.List<{{eventStrings.EventListenerInterface}}> Value;
                        }

                        public static class {{eventStrings.EventListener}}EventEntityExtension
                        {
                            public static void Add{{eventStrings.EventPrefix}}(this Entity entity, {{eventStrings.EventListenerInterface}} value)
                            {
                                var listeners = entity.Has{{eventStrings.EventPrefix}}()
                                    ? entity.Get{{eventStrings.EventPrefix}}().Value
                                    : new global::System.Collections.Generic.List<{{eventStrings.EventListenerInterface}}>();
                                listeners.Add(value);
                                entity.Replace{{eventStrings.EventPrefix}}(listeners);
                            }

                            public static void Remove{{eventStrings.EventPrefix}}(this Entity entity, {{eventStrings.EventListenerInterface}} value, bool removeComponentWhenEmpty = true)
                            {
                                var listeners = entity.Get{{eventStrings.EventPrefix}}().Value;
                                listeners.Remove(value);
                                if (removeComponentWhenEmpty && listeners.Count == 0)
                                {
                                    entity.Remove{{eventStrings.EventPrefix}}();
                                }
                                else
                                {
                                    entity.Replace{{eventStrings.EventPrefix}}(listeners);
                                }
                            }
                        }
                        """));

                var eventComponent = ToEvent(component, eventStrings);
                OnFullNameOrContextsChanged(spc, eventComponent, context);
                OnFullNameOrMembersOrContextsChanged(spc, eventComponent, context);
            }
        }
    }
}
