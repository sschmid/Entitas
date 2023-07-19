using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void Matcher(SourceProductionContext spc, ComponentDeclaration component, string context, AnalyzerConfigOptionsProvider optionsProvider)
        {
            if (!EntitasAnalyzerConfigOptions.ComponentMatcher(optionsProvider, component.SyntaxTree))
                return;

            var contextPrefix = ContextPrefix(context);
            var contextAwareComponentPrefix = component.ContextAwareComponentPrefix(contextPrefix);
            var className = $"{contextAwareComponentPrefix}Matcher";
            var index = $"{contextAwareComponentPrefix}ComponentIndex.Index.Value";
            spc.AddSource(
                GeneratedPath(CombinedNamespace(component.Namespace, className)),
                GeneratedFileHeader(GeneratorSource(nameof(Matcher))) +
                NamespaceDeclaration(component.Namespace,
                    $$"""
                    public sealed class {{className}}
                    {
                        static global::Entitas.IMatcher<global::{{contextPrefix}}.Entity> _matcher;

                        public static global::Entitas.IMatcher<global::{{contextPrefix}}.Entity> {{component.Prefix}}
                        {
                            get
                            {
                                if (_matcher == null)
                                {
                                    var matcher = (global::Entitas.Matcher<global::{{contextPrefix}}.Entity>)global::Entitas.Matcher<global::{{contextPrefix}}.Entity>.AllOf({{index}});
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
