using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Entitas.Generators.Templates;

namespace Entitas.Generators
{
    [Generator(LanguageNames.CSharp)]
    public class ComponentGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext initContext)
        {
            var allComponents = initContext.SyntaxProvider.CreateSyntaxProvider(
                    static (node, _) => node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                                        && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                                        && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                                        && candidate.Modifiers.Any(SyntaxKind.SealedKeyword)
                                        && !candidate.Modifiers.Any(SyntaxKind.PartialKeyword),
                    static ImmutableArray<ComponentDeclaration> (syntaxContext, cancellationToken) =>
                    {
                        var candidate = (ClassDeclarationSyntax)syntaxContext.Node;
                        var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
                        if (symbol is null)
                            return ImmutableArray<ComponentDeclaration>.Empty;

                        var interfaceType = syntaxContext.SemanticModel.Compilation.GetTypeByMetadataName("Entitas.IComponent");
                        if (interfaceType is null)
                            return ImmutableArray<ComponentDeclaration>.Empty;

                        var isComponent = symbol.Interfaces.Any(i => i.OriginalDefinition.Equals(interfaceType, SymbolEqualityComparer.Default));
                        if (!isComponent)
                            return ImmutableArray<ComponentDeclaration>.Empty;

                        return symbol.GetAttributes()
                            .Select(attribute => attribute.AttributeClass?.ToDisplayString())
                            .Where(attribute => attribute?.EndsWith(".Context") | attribute?.EndsWith(".ContextAttribute") ?? false)
                            .Select(attribute =>
                            {
                                const string suffix = ".Context";
                                return attribute!.EndsWith(suffix, StringComparison.Ordinal)
                                    ? attribute.Substring(0, attribute.Length - suffix.Length)
                                    : attribute!.RemoveSuffix(".ContextAttribute");
                            })
                            .Distinct()
                            .Select(attribute => new ComponentDeclaration(symbol, attribute.RemoveSuffix(".Context"), cancellationToken))
                            .ToImmutableArray();
                    })
                .SelectMany((components, _) => components);

            var componentIndexesProvider = allComponents.WithComparer(new ComponentIndexComparer());
            initContext.RegisterSourceOutput(componentIndexesProvider, ComponentIndex);

            var entityExtensionsProvider = allComponents.WithComparer(new EntityExtensionComparer());
            initContext.RegisterSourceOutput(entityExtensionsProvider, EntityExtension);

            var contextInitializationMethodsProvider = initContext.SyntaxProvider.CreateSyntaxProvider(
                    static (node, _) => node is MethodDeclarationSyntax { AttributeLists.Count: > 0 } candidate
                                        && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                                        && candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                                        && candidate.Modifiers.Any(SyntaxKind.PartialKeyword)
                                        && candidate.ReturnType is PredefinedTypeSyntax predefined
                                        && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword),
                    static ContextInitializationMethodDeclaration? (syntaxContext, cancellationToken) =>
                    {
                        var candidate = (MethodDeclarationSyntax)syntaxContext.Node;
                        var symbol = syntaxContext.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
                        if (symbol is null)
                            return null;

                        if (!symbol.ContainingType.IsStatic || symbol.ContainingType.DeclaredAccessibility != Accessibility.Public)
                            return null;

                        var attribute = symbol
                            .GetAttributes()
                            .SingleOrDefault(attribute => attribute.AttributeClass?.ToDisplayString()
                                .EndsWith(".ContextInitialization") ?? false);

                        if (attribute is null)
                            return null;

                        return new ContextInitializationMethodDeclaration(symbol, attribute);
                    })
                .Where(method => method is not null)
                .Select((method, _) => method!.Value);

            var contextInitializationMethodComponentsProvider = allComponents
                .WithComparer(new ContextInitializationMethodComparer())
                .Collect()
                .Select((components, _) => components
                    .OrderBy(component => component.FullName)
                    .ToImmutableArray());

            initContext.RegisterSourceOutput(contextInitializationMethodsProvider.Combine(contextInitializationMethodComponentsProvider), (spc, pair) =>
            {
                var (method, components) = pair;
                ContextInitializationMethod(spc, method, components
                    .Where(component => component.Context == method.FullContextPrefix)
                    .ToImmutableArray());
            });
        }

        static void ComponentIndex(SourceProductionContext spc, ComponentDeclaration component)
        {
            var fileName = $"{component.FullName}.{component.Context}.ComponentIndex";
            var className = $"{component.Context.Replace(".", string.Empty)}{component.ComponentPrefix}ComponentIndex";

            spc.AddSource(
                GeneratedPath(fileName),
                GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                $"using {component.Context};\n\n" +
                NamespaceDeclaration(component.Namespace,
                    $$"""
                    public static class {{className}}
                    {
                        public static ComponentIndex Index;
                    }

                    """));
        }

        static void EntityExtension(SourceProductionContext spc, ComponentDeclaration component)
        {
            var contextPrefix = component.Context.Replace(".", string.Empty);
            var className = $"{contextPrefix}{component.ComponentPrefix}EntityExtension";

            string content;
            if (component.Members.Length > 0)
            {
                content = $$"""
                    public static class {{className}}
                    {
                        public static bool Has{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return entity.HasComponent(Index.Value);
                        }

                        public static Entity Add{{component.ComponentPrefix}}(this Entity entity, {{ComponentMethodArgs(component)}})
                        {
                            var index = Index.Value;
                            var component = ({{component.Name}})entity.CreateComponent(index, typeof({{component.Name}}));
                    {{ComponentValueAssignments(component)}}
                            entity.AddComponent(index, component);
                            return entity;
                        }

                        public static Entity Replace{{component.ComponentPrefix}}(this Entity entity, {{ComponentMethodArgs(component)}})
                        {
                            var index = Index.Value;
                            var component = ({{component.Name}})entity.CreateComponent(index, typeof({{component.Name}}));
                    {{ComponentValueAssignments(component)}}
                            entity.ReplaceComponent(index, component);
                            return entity;
                        }

                        public static Entity Remove{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.RemoveComponent(Index.Value);
                            return entity;
                        }

                        public static {{component.Name}} Get{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return ({{component.Name}})entity.GetComponent(Index.Value);
                        }
                    }

                    """;


                static string ComponentMethodArgs(ComponentDeclaration component)
                {
                    return string.Join(", ", component.Members.Select(member => $"{member.Type} {member.ValidLowerFirstName}"));
                }

                static string ComponentValueAssignments(ComponentDeclaration component)
                {
                    return string.Join("\n", component.Members.Select(member =>
                        $$"""
                                component.{{member.Name}} = {{member.ValidLowerFirstName}};
                        """));
                }
            }
            else
            {
                content = $$"""
                    public static class {{className}}
                    {
                        static readonly {{component.Name}} Single{{component.Name}} = new {{component.Name}}();

                        public static bool Has{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return entity.HasComponent(Index.Value);
                        }

                        public static Entity Add{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.AddComponent(Index.Value, Single{{component.Name}});
                            return entity;
                        }

                        public static Entity Replace{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.ReplaceComponent(Index.Value, Single{{component.Name}});
                            return entity;
                        }

                        public static Entity Remove{{component.ComponentPrefix}}(this Entity entity)
                        {
                            entity.RemoveComponent(Index.Value);
                            return entity;
                        }

                        public static {{component.Name}} Get{{component.ComponentPrefix}}(this Entity entity)
                        {
                            return ({{component.Name}})entity.GetComponent(Index.Value);
                        }
                    }

                    """;
            }

            spc.AddSource(
                GeneratedPath($"{component.FullName}.{component.Context}.EntityExtension"),
                GeneratedFileHeader(GeneratorSource(nameof(EntityExtension))) +
                $"using {component.Context};\n" +
                $"using static {CombinedNamespace(component.Namespace, contextPrefix)}{component.ComponentPrefix}ComponentIndex;\n\n" +
                NamespaceDeclaration(component.Namespace, content));
        }

        static void ContextInitializationMethod(SourceProductionContext spc, ContextInitializationMethodDeclaration method, ImmutableArray<ComponentDeclaration> components)
        {
            spc.AddSource(
                GeneratedPath($"{method.FullContextPrefix}.ContextInitializationMethod"),
                GeneratedFileHeader(GeneratorSource(nameof(ContextInitializationMethod))) +
                $"using {method.FullContextPrefix};\n\n" +
                NamespaceDeclaration(method.Namespace,
                    $$"""
                    public static partial class {{method.Class}}
                    {
                        public static partial void {{method.Name}}()
                        {
                    {{ComponentIndexAssignments(method, components)}}

                            {{method.Context}}.ComponentNames = new string[]
                            {
                    {{ComponentNames(components)}}
                            };

                            {{method.Context}}.ComponentTypes = new System.Type[]
                            {
                    {{ComponentTypes(components)}}
                            };
                        }
                    }

                    """));

            static string ComponentIndexAssignments(ContextInitializationMethodDeclaration method, ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join("\n", components.Select((component, i) =>
                {
                    var contextPrefix = CombinedNamespace(component.Namespace, method.FullContextPrefix.Replace(".", string.Empty));
                    return $"        {contextPrefix}{component.ComponentPrefix}ComponentIndex.Index = new ComponentIndex({i});";
                }));
            }

            static string ComponentNames(ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join(",\n", components.Select(component => $"            \"{component.FullName.RemoveSuffix("Component")}\""));
            }

            static string ComponentTypes(ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join(",\n", components.Select(component => $"            typeof({component.FullName})"));
            }
        }

        static string GeneratorSource(string source)
        {
            return $"{typeof(ComponentGenerator).FullName}.{source}";
        }
    }
}
