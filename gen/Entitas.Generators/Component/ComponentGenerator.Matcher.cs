using Microsoft.CodeAnalysis;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void Matcher(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            var contextPrefix = component.ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}Matcher";
            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(Matcher))) +
                $"using global::{contextPrefix};\n" +
                $"using static global::{CombinedNamespace(component.Namespace, contextAwareComponentPrefix)}ComponentIndex;\n\n" +
                NamespaceDeclaration(component.Namespace,
                    $$"""
                    public sealed class {{className}}
                    {
                        static global::Entitas.IMatcher<Entity> _matcher;

                        public static global::Entitas.IMatcher<Entity> {{component.ComponentPrefix}}
                        {
                            get
                            {
                                if (_matcher == null)
                                {
                                    var matcher = (global::Entitas.Matcher<Entity>)global::Entitas.Matcher<Entity>.AllOf(Index.Value);
                                    matcher.componentNames = {{context}}.ComponentNames;
                                    _matcher = matcher;
                                }

                                return _matcher;
                            }
                        }
                    }

                    """));
        }
    }
}
