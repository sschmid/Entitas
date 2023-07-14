using Microsoft.CodeAnalysis;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void EntityExtension(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            var contextPrefix = component.ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}EntityExtension";
            string content;
            if (component.Members.Length > 0)
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.Prefix}}(this Entity entity)
                        {
                            return entity.HasComponent(Index.Value);
                        }

                        public static Entity Add{{component.Prefix}}(this Entity entity, {{ComponentMethodParams(component)}})
                        {
                            var index = Index.Value;
                            var componentPool = entity.GetComponentPool(index);
                            var component = componentPool.Count > 0
                                ? ({{component.Name}})componentPool.Pop()
                                : new {{component.Name}}();
                    {{ComponentValueAssignments(component)}}
                            entity.AddComponent(index, component);
                            return entity;
                        }

                        public static Entity Replace{{component.Prefix}}(this Entity entity, {{ComponentMethodParams(component)}})
                        {
                            var index = Index.Value;
                            var componentPool = entity.GetComponentPool(index);
                            var component = componentPool.Count > 0
                                ? ({{component.Name}})componentPool.Pop()
                                : new {{component.Name}}();
                    {{ComponentValueAssignments(component)}}
                            entity.ReplaceComponent(index, component);
                            return entity;
                        }

                        public static Entity Remove{{component.Prefix}}(this Entity entity)
                        {
                            entity.RemoveComponent(Index.Value);
                            return entity;
                        }

                        public static {{component.Name}} Get{{component.Prefix}}(this Entity entity)
                        {
                            return ({{component.Name}})entity.GetComponent(Index.Value);
                        }
                    }

                    """;
            }
            else
            {
                content = $$"""
                    public static class {{className}}
                    {
                        static readonly {{component.Name}} Single{{component.Name}} = new {{component.Name}}();

                        public static bool Has{{component.Prefix}}(this Entity entity)
                        {
                            return entity.HasComponent(Index.Value);
                        }

                        public static Entity Add{{component.Prefix}}(this Entity entity)
                        {
                            entity.AddComponent(Index.Value, Single{{component.Name}});
                            return entity;
                        }

                        public static Entity Replace{{component.Prefix}}(this Entity entity)
                        {
                            entity.ReplaceComponent(Index.Value, Single{{component.Name}});
                            return entity;
                        }

                        public static Entity Remove{{component.Prefix}}(this Entity entity)
                        {
                            entity.RemoveComponent(Index.Value);
                            return entity;
                        }

                        public static {{component.Name}} Get{{component.Prefix}}(this Entity entity)
                        {
                            return ({{component.Name}})entity.GetComponent(Index.Value);
                        }
                    }

                    """;
            }

            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(EntityExtension))) +
                $"using global::{contextPrefix};\n" +
                $"using static global::{CombinedNamespace(component.Namespace, contextAwareComponentPrefix)}ComponentIndex;\n\n" +
                NamespaceDeclaration(component.Namespace, content));
        }
    }
}
