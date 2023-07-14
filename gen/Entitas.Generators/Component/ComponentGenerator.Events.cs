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
            var contextAware = component.ContextAware(contextPrefix);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            foreach (var @event in component.Events)
            {
                var eventStrings = new EventStrings(@event, component.ComponentPrefix, contextAware);

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
                        componentDeclaration = $"\n            var component = entity.Get{component.ComponentPrefix}();";
                    }

                    filter = $"entity.Has{component.ComponentPrefix}()";
                }
                else
                {
                    filter = $"!entity.Has{component.ComponentPrefix}()";
                }

                if (@event.EventTarget == 1)
                {
                    filter += $" && entity.Has{eventStrings.EventListener}()";
                }

                var content = $$"""
                    public interface {{eventStrings.ContextAwareEventListenerInterface}}
                    {
                        void {{eventStrings.EventMethod}}(Entity entity{{optionalComponentMethodParams}});
                    }

                    public sealed class {{eventStrings.ContextAwareEventListenerComponent}} : global::Entitas.IComponent
                    {
                        public global::System.Collections.Generic.List<{{eventStrings.ContextAwareEventListenerInterface}}> Value;
                    }

                    public static class {{eventStrings.ContextAwareEventListener}}EventEntityExtension
                    {
                        public static Entity Add{{eventStrings.EventListener}}(this Entity entity, {{eventStrings.ContextAwareEventListenerInterface}} value)
                        {
                            var listeners = entity.Has{{eventStrings.EventListener}}()
                                ? entity.Get{{eventStrings.EventListener}}().Value
                                : new global::System.Collections.Generic.List<{{eventStrings.ContextAwareEventListenerInterface}}>();
                            listeners.Add(value);
                            return entity.Replace{{eventStrings.EventListener}}(listeners);
                        }

                        public static void Remove{{eventStrings.EventListener}}(this Entity entity, {{eventStrings.ContextAwareEventListenerInterface}} value, bool destroyListenerWhenEmpty = true)
                        {
                            var listeners = entity.Get{{eventStrings.EventListener}}().Value;
                            listeners.Remove(value);
                            if (destroyListenerWhenEmpty && listeners.Count == 0)
                                entity.Destroy();
                            else
                                entity.Replace{{eventStrings.EventListener}}(listeners);
                        }
                    }


                    """;

                if (@event.EventTarget == 0)
                {
                    content += $$"""
                        public sealed class {{eventStrings.ContextAwareEvent}}EventSystem : global::Entitas.ReactiveSystem<Entity>
                        {
                            readonly global::Entitas.IGroup<Entity> _listeners;
                            readonly global::System.Collections.Generic.List<Entity> _entityBuffer;
                            readonly global::System.Collections.Generic.List<{{eventStrings.ContextAwareEventListenerInterface}}> _listenerBuffer;

                            public {{eventStrings.ContextAwareEvent}}EventSystem({{context}} context) : base(context)
                            {
                                _listeners = context.GetGroup({{eventStrings.ContextAwareEventListener}}Matcher.{{eventStrings.EventListener}});
                                _entityBuffer = new global::System.Collections.Generic.List<Entity>();
                                _listenerBuffer = new global::System.Collections.Generic.List<{{eventStrings.ContextAwareEventListenerInterface}}>();
                            }

                            protected override global::Entitas.ICollector<Entity> GetTrigger(global::Entitas.IContext<Entity> context)
                            {
                                return global::Entitas.CollectorContextExtension.CreateCollector(
                                    context, global::Entitas.TriggerOnEventMatcherExtension.Added({{contextAwareComponentPrefix}}Matcher.{{component.ComponentPrefix}})
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
                                        _listenerBuffer.AddRange(listenerEntity.Get{{eventStrings.EventListener}}().Value);
                                        foreach (var listener in _listenerBuffer)
                                        {
                                            listener.{{eventStrings.EventMethod}}(entity{{optionalComponentValueMethodArgs}});
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
                        public sealed class {{eventStrings.ContextAwareEvent}}EventSystem : global::Entitas.ReactiveSystem<Entity>
                        {
                            readonly global::System.Collections.Generic.List<{{eventStrings.ContextAwareEventListenerInterface}}> _listenerBuffer;

                            public {{eventStrings.ContextAwareEvent}}EventSystem({{context}} context) : base(context)
                            {
                                _listenerBuffer = new global::System.Collections.Generic.List<{{eventStrings.ContextAwareEventListenerInterface}}>();
                            }

                            protected override global::Entitas.ICollector<Entity> GetTrigger(global::Entitas.IContext<Entity> context)
                            {
                                return global::Entitas.CollectorContextExtension.CreateCollector(
                                    context, global::Entitas.TriggerOnEventMatcherExtension.Added({{contextAwareComponentPrefix}}Matcher.{{component.ComponentPrefix}})
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
                                    _listenerBuffer.AddRange(entity.Get{{eventStrings.EventListener}}().Value);
                                    foreach (var listener in _listenerBuffer)
                                    {
                                        listener.{{eventStrings.EventMethod}}(entity{{optionalComponentValueMethodArgs}});
                                    }
                                }
                            }
                        }

                        """;
                }

                spc.AddSource(
                    GeneratedPath(CombinedNamespace(component.Namespace, eventStrings.ContextAwareEventListenerComponent)),
                    GeneratedFileHeader(GeneratorSource(nameof(Events))) +
                    $"using global::{contextPrefix};\n\n" +
                    NamespaceDeclaration(component.Namespace, content));

                var eventComponent = ToEvent(component, eventStrings);
                OnFullNameOrContextsChanged(spc, eventComponent, context);
                OnFullNameOrMembersOrContextsChanged(spc, eventComponent, context);
            }
        }
    }
}
