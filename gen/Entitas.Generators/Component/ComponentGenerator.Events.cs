using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void Events(SourceProductionContext spc, ComponentDeclaration component, string context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!EntitasAnalyzerConfigOptions.ComponentEvents(optionsProvider, component.SyntaxTree))
                return;

            if (component.Events.Length == 0)
                return;

            var contextPrefix = ContextPrefix(context);
            var contextAware = ContextAware(contextPrefix);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            foreach (var @event in component.Events)
            {
                @event.ContextAware(contextAware);
                var optionalComponentMethodParams = string.Empty;
                var optionalComponentValueMethodArgs = string.Empty;
                var componentDeclaration = string.Empty;
                string filter;

                if (@event.EventType == 0)
                {
                    if (component.Members.Length > 0)
                    {
                        optionalComponentMethodParams = $", {ComponentMethodParams(component)}";
                        optionalComponentValueMethodArgs = $", {ComponentValueMethodArgs(component)}";
                        componentDeclaration = $"\n            var component = entity.Get{component.Prefix}();";
                    }

                    filter = $"entity.Has{component.Prefix}()";
                }
                else
                {
                    filter = $"!entity.Has{component.Prefix}()";
                }

                if (@event.EventTarget == 1)
                {
                    filter += $" && entity.Has{@event.EventListener}()";
                }

                var content = $$"""
                    public interface {{@event.ContextAwareEventListenerInterface}}
                    {
                        void {{@event.EventMethod}}(Entity entity{{optionalComponentMethodParams}});
                    }

                    public sealed class {{@event.ContextAwareEventListenerComponent}} : global::Entitas.IComponent
                    {
                        public global::System.Collections.Generic.List<{{@event.ContextAwareEventListenerInterface}}> Value;
                    }

                    public static class {{@event.ContextAwareEventListener}}EventEntityExtension
                    {
                        public static Entity Add{{@event.EventListener}}(this Entity entity, {{@event.ContextAwareEventListenerInterface}} value)
                        {
                            var listeners = entity.Has{{@event.EventListener}}()
                                ? entity.Get{{@event.EventListener}}().Value
                                : new global::System.Collections.Generic.List<{{@event.ContextAwareEventListenerInterface}}>();
                            listeners.Add(value);
                            return entity.Replace{{@event.EventListener}}(listeners);
                        }

                        public static void Remove{{@event.EventListener}}(this Entity entity, {{@event.ContextAwareEventListenerInterface}} value, bool removeListenerWhenEmpty = true)
                        {
                            var listeners = entity.Get{{@event.EventListener}}().Value;
                            listeners.Remove(value);
                            if (removeListenerWhenEmpty && listeners.Count == 0)
                            {
                                entity.Remove{{@event.EventListener}}();
                                if (entity.IsEmpty())
                                    entity.Destroy();
                            }
                            else
                            {
                                entity.Replace{{@event.EventListener}}(listeners);
                            }
                        }
                    }


                    """;

                if (@event.EventTarget == 0)
                {
                    content += $$"""
                        public sealed class {{@event.ContextAwareEvent}}EventSystem : global::Entitas.ReactiveSystem<Entity>
                        {
                            readonly global::Entitas.IGroup<Entity> _listeners;
                            readonly global::System.Collections.Generic.List<Entity> _entityBuffer;
                            readonly global::System.Collections.Generic.List<{{@event.ContextAwareEventListenerInterface}}> _listenerBuffer;

                            public {{@event.ContextAwareEvent}}EventSystem({{context}} context) : base(context)
                            {
                                _listeners = context.GetGroup({{@event.ContextAwareEventListener}}Matcher.{{@event.EventListener}});
                                _entityBuffer = new global::System.Collections.Generic.List<Entity>();
                                _listenerBuffer = new global::System.Collections.Generic.List<{{@event.ContextAwareEventListenerInterface}}>();
                            }

                            protected override global::Entitas.ICollector<Entity> GetTrigger(global::Entitas.IContext<Entity> context)
                            {
                                return global::Entitas.CollectorContextExtension.CreateCollector(
                                    context, global::Entitas.TriggerOnEventMatcherExtension.Added({{contextAwareComponentPrefix}}Matcher.{{component.Prefix}})
                                );
                            }

                            protected override bool Filter(Entity entity)
                            {
                                return {{filter}};
                            }

                            protected override void Execute(global::System.Collections.Generic.List<Entity> entities)
                            {
                                foreach (var entity in entities)
                                {{{componentDeclaration}}
                                    foreach (var listenerEntity in _listeners.GetEntities(_entityBuffer))
                                    {
                                        _listenerBuffer.Clear();
                                        _listenerBuffer.AddRange(listenerEntity.Get{{@event.EventListener}}().Value);
                                        foreach (var listener in _listenerBuffer)
                                        {
                                            listener.{{@event.EventMethod}}(entity{{optionalComponentValueMethodArgs}});
                                        }
                                    }
                                }
                            }
                        }

                        """;
                }
                else
                {
                    content += $$"""
                        public sealed class {{@event.ContextAwareEvent}}EventSystem : global::Entitas.ReactiveSystem<Entity>
                        {
                            readonly global::System.Collections.Generic.List<{{@event.ContextAwareEventListenerInterface}}> _listenerBuffer;

                            public {{@event.ContextAwareEvent}}EventSystem({{context}} context) : base(context)
                            {
                                _listenerBuffer = new global::System.Collections.Generic.List<{{@event.ContextAwareEventListenerInterface}}>();
                            }

                            protected override global::Entitas.ICollector<Entity> GetTrigger(global::Entitas.IContext<Entity> context)
                            {
                                return global::Entitas.CollectorContextExtension.CreateCollector(
                                    context, global::Entitas.TriggerOnEventMatcherExtension.Added({{contextAwareComponentPrefix}}Matcher.{{component.Prefix}})
                                );
                            }

                            protected override bool Filter(Entity entity)
                            {
                                return {{filter}};
                            }

                            protected override void Execute(global::System.Collections.Generic.List<Entity> entities)
                            {
                                foreach (var entity in entities)
                                {{{componentDeclaration}}
                                    _listenerBuffer.Clear();
                                    _listenerBuffer.AddRange(entity.Get{{@event.EventListener}}().Value);
                                    foreach (var listener in _listenerBuffer)
                                    {
                                        listener.{{@event.EventMethod}}(entity{{optionalComponentValueMethodArgs}});
                                    }
                                }
                            }
                        }

                        """;
                }

                spc.AddSource(
                    GeneratedPath(CombinedNamespace(component.Namespace, @event.ContextAwareEventListenerComponent)),
                    GeneratedFileHeader(GeneratorSource(nameof(Events))) +
                    $"using global::{contextPrefix};\n\n" +
                    NamespaceDeclaration(component.Namespace, content));

                var eventComponent = ToEvent(component, @event);
                ComponentIndex(spc, eventComponent, context, optionsProvider);
                Matcher(spc, eventComponent, context, optionsProvider);
                EntityExtension(spc, eventComponent, context, optionsProvider);
            }
        }
    }
}
