using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void ContextExtension(SourceProductionContext spc, ComponentDeclaration component, string context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!component.IsUnique)
                return;

            if (!EntitasAnalyzerConfigOptions.ComponentContextExtension(optionsProvider, component.SyntaxTree))
                return;

            var contextPrefix = ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}ContextExtension";
            string content;
            if (component.Members.Length > 0)
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.Prefix}}(this global::{{context}} context)
                        {
                            return context.Get{{component.Prefix}}Entity() != null;
                        }

                        public static global::{{contextPrefix}}.Entity Set{{component.Prefix}}(this global::{{context}} context, {{ComponentMethodParams(component)}})
                        {
                            if (context.Has{{component.Prefix}}())
                            {
                                throw new global::Entitas.EntitasException(
                                    $"Could not set {{component.Prefix}}!\n{context} already has an entity with {{component.FullName}}!",
                                    "You should check if the context already has a {{component.Prefix}}Entity before setting it or use context.Replace{{component.Prefix}}()."
                                );
                            }

                            return context.CreateEntity().Add{{component.Prefix}}({{ComponentMethodArgs(component)}});
                        }

                        public static global::{{contextPrefix}}.Entity Replace{{component.Prefix}}(this global::{{context}} context, {{ComponentMethodParams(component)}})
                        {
                            var entity = context.Get{{component.Prefix}}Entity();
                            if (entity == null)
                                entity = context.CreateEntity().Add{{component.Prefix}}({{ComponentMethodArgs(component)}});
                            else
                                entity.Replace{{component.Prefix}}({{ComponentMethodArgs(component)}});

                            return entity;
                        }

                        public static void Remove{{component.Prefix}}(this global::{{context}} context)
                        {
                            context.Get{{component.Prefix}}Entity().Destroy();
                        }

                        public static global::{{contextPrefix}}.Entity Get{{component.Prefix}}Entity(this global::{{context}} context)
                        {
                            return context.GetGroup({{contextAwareComponentPrefix}}Matcher.{{component.Prefix}}).GetSingleEntity();
                        }

                        public static {{component.Name}} Get{{component.Prefix}}(this global::{{context}} context)
                        {
                            return context.Get{{component.Prefix}}Entity().Get{{component.Prefix}}();
                        }
                    }

                    """;
            }
            else
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.Prefix}}(this global::{{context}} context)
                        {
                            return context.Get{{component.Prefix}}Entity() != null;
                        }

                        public static global::{{contextPrefix}}.Entity Set{{component.Prefix}}(this global::{{context}} context)
                        {
                            return context.Get{{component.Prefix}}Entity() ?? context.CreateEntity().Add{{component.Prefix}}();
                        }

                        public static void Unset{{component.Prefix}}(this global::{{context}} context)
                        {
                            context.Get{{component.Prefix}}Entity()?.Destroy();
                        }

                        public static global::{{contextPrefix}}.Entity Get{{component.Prefix}}Entity(this global::{{context}} context)
                        {
                            return context.GetGroup({{contextAwareComponentPrefix}}Matcher.{{component.Prefix}}).GetSingleEntity();
                        }
                    }

                    """;
            }

            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(ContextExtension))) +
                NamespaceDeclaration(component.Namespace, content));
        }
    }
}
