using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void EntityExtension(SourceProductionContext spc, ComponentDeclaration component, string context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!EntitasAnalyzerConfigOptions.ComponentEntityExtension(optionsProvider, component.SyntaxTree))
                return;

            var contextPrefix = ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}EntityExtension";
            var index = $"{contextAwareComponentPrefix}ComponentIndex.Index.Value";
            string content;
            if (component.Members.Length > 0)
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.Prefix}}(this global::{{contextPrefix}}.Entity entity)
                        {
                            return entity.HasComponent({{index}});
                        }

                        public static global::{{contextPrefix}}.Entity Add{{component.Prefix}}(this global::{{contextPrefix}}.Entity entity, {{ComponentMethodParams(component)}})
                        {
                            var index = {{index}};
                            var componentPool = entity.GetComponentPool(index);
                            var component = componentPool.Count > 0
                                ? ({{component.Name}})componentPool.Pop()
                                : new {{component.Name}}();
                    {{ComponentValueAssignments(component)}}
                            entity.AddComponent(index, component);
                            return entity;
                        }

                        public static global::{{contextPrefix}}.Entity Replace{{component.Prefix}}(this global::{{contextPrefix}}.Entity entity, {{ComponentMethodParams(component)}})
                        {
                            var index = {{index}};
                            var componentPool = entity.GetComponentPool(index);
                            var component = componentPool.Count > 0
                                ? ({{component.Name}})componentPool.Pop()
                                : new {{component.Name}}();
                    {{ComponentValueAssignments(component)}}
                            entity.ReplaceComponent(index, component);
                            return entity;
                        }

                        public static global::{{contextPrefix}}.Entity Remove{{component.Prefix}}(this global::{{contextPrefix}}.Entity entity)
                        {
                            entity.RemoveComponent({{index}});
                            return entity;
                        }

                        public static {{component.Name}} Get{{component.Prefix}}(this global::{{contextPrefix}}.Entity entity)
                        {
                            return ({{component.Name}})entity.GetComponent({{index}});
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

                        public static bool Has{{component.Prefix}}(this global::{{contextPrefix}}.Entity entity)
                        {
                            return entity.HasComponent({{index}});
                        }

                        public static global::{{contextPrefix}}.Entity Add{{component.Prefix}}(this global::{{contextPrefix}}.Entity entity)
                        {
                            entity.AddComponent({{index}}, Single{{component.Name}});
                            return entity;
                        }

                        public static global::{{contextPrefix}}.Entity Replace{{component.Prefix}}(this global::{{contextPrefix}}.Entity entity)
                        {
                            entity.ReplaceComponent({{index}}, Single{{component.Name}});
                            return entity;
                        }

                        public static global::{{contextPrefix}}.Entity Remove{{component.Prefix}}(this global::{{contextPrefix}}.Entity entity)
                        {
                            entity.RemoveComponent({{index}});
                            return entity;
                        }

                        public static {{component.Name}} Get{{component.Prefix}}(this global::{{contextPrefix}}.Entity entity)
                        {
                            return ({{component.Name}})entity.GetComponent({{index}});
                        }
                    }

                    """;
            }

            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(EntityExtension))) +
                NamespaceDeclaration(component.Namespace, content));
        }
    }
}
