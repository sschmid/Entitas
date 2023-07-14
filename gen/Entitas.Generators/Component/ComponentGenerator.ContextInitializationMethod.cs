using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    partial class ComponentGenerator
    {
        static void ContextInitializationMethod(SourceProductionContext spc, ContextInitializationMethodDeclaration method)
        {
            spc.AddSource(
                GeneratedPath(CombinedNamespace(method.Namespace, $"{method.Class}.{method.Name}.ContextInitialization")),
                GeneratedFileHeader(GeneratorSource(nameof(ContextInitializationMethod))) +
                $"using global::{method.FullContextPrefix};\n\n" +
                NamespaceDeclaration(method.Namespace,
                    $$"""
                    public static partial class {{method.Class}}
                    {
                        public static partial void {{method.Name}}()
                        {
                    {{ComponentIndexAssignments(method, method.Components)}}

                            global::{{method.ContextFullName}}.ComponentNames = new string[]
                            {
                    {{ComponentNames(method.Components)}}
                            };

                            global::{{method.ContextFullName}}.ComponentTypes = new global::System.Type[]
                            {
                    {{ComponentTypes(method.Components)}}
                            };
                        }
                    }

                    """));

            static string ComponentIndexAssignments(ContextInitializationMethodDeclaration method, ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join("\n", components.Select((component, i) =>
                {
                    var contextPrefix = "global::" + CombinedNamespace(component.Namespace, component.ContextAware(method.FullContextPrefix).Replace(".", string.Empty));
                    return $"        {contextPrefix}{component.Prefix}ComponentIndex.Index = new ComponentIndex({i});";
                }));
            }

            static string ComponentNames(ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join(",\n", components.Select(static component => $"            \"{component.FullName.RemoveSuffix("Component")}\""));
            }

            static string ComponentTypes(ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join(",\n", components.Select(static component => $"            typeof(global::{component.FullName})"));
            }
        }
    }
}
