using Microsoft.CodeAnalysis;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void ContextExtension(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            if (!component.IsUnique)
                return;

            var contextPrefix = component.ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}ContextExtension";
            string content;
            if (component.Members.Length > 0)
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() != null;
                        }

                        public static Entity Set{{component.ComponentPrefix}}(this {{context}} context, {{ComponentMethodParams(component)}})
                        {
                            if (context.Has{{component.ComponentPrefix}}())
                            {
                                throw new global::Entitas.EntitasException(
                                    $"Could not set {{component.ComponentPrefix}}!\n{context} already has an entity with {{component.FullName}}!",
                                    "You should check if the context already has a {{component.ComponentPrefix}}Entity before setting it or use context.Replace{{component.ComponentPrefix}}()."
                                );
                            }

                            return context.CreateEntity().Add{{component.ComponentPrefix}}({{ComponentMethodArgs(component)}});
                        }

                        public static Entity Replace{{component.ComponentPrefix}}(this {{context}} context, {{ComponentMethodParams(component)}})
                        {
                            var entity = context.Get{{component.ComponentPrefix}}Entity();
                            if (entity == null)
                                entity = context.CreateEntity().Add{{component.ComponentPrefix}}({{ComponentMethodArgs(component)}});
                            else
                                entity.Replace{{component.ComponentPrefix}}({{ComponentMethodArgs(component)}});

                            return entity;
                        }

                        public static void Remove{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            context.Get{{component.ComponentPrefix}}Entity().Destroy();
                        }

                        public static Entity Get{{component.ComponentPrefix}}Entity(this {{context}} context)
                        {
                            return context.GetGroup({{contextAwareComponentPrefix}}Matcher.{{component.ComponentPrefix}}).GetSingleEntity();
                        }

                        public static {{component.Name}} Get{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity().Get{{component.ComponentPrefix}}();
                        }
                    }

                    """;
            }
            else
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() != null;
                        }

                        public static Entity Set{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            return context.Get{{component.ComponentPrefix}}Entity() ?? context.CreateEntity().Add{{component.ComponentPrefix}}();
                        }

                        public static void Unset{{component.ComponentPrefix}}(this {{context}} context)
                        {
                            context.Get{{component.ComponentPrefix}}Entity()?.Destroy();
                        }

                        public static Entity Get{{component.ComponentPrefix}}Entity(this {{context}} context)
                        {
                            return context.GetGroup({{contextAwareComponentPrefix}}Matcher.{{component.ComponentPrefix}}).GetSingleEntity();
                        }
                    }

                    """;
            }

            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(ContextExtension))) +
                $"using global::{contextPrefix};\n\n" +
                NamespaceDeclaration(component.Namespace, content));
        }
    }
}
