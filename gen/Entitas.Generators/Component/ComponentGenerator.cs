using System.Collections.Immutable;
using System.Linq;
using System.Threading;
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
            var allComponents = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsComponentCandidate, CreateComponentDeclarations)
                .SelectMany((components, _) => components);

            var fullNameOrContextChanged = allComponents.WithComparer(new FullNameAndContextComparer());
            initContext.RegisterSourceOutput(fullNameOrContextChanged, OnFullNameOrContextChanged);

            var fullNameOrMembersOrContextChanged = allComponents.WithComparer(new FullNameAndMembersAndContextComparer());
            initContext.RegisterSourceOutput(fullNameOrMembersOrContextChanged, OnFullNameOrMembersOrContextChanged);

            var contextInitializationMethods = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsContextInitializationMethodCandidate, CreateContextInitializationMethodDeclaration)
                .Where(method => method is not null)
                .Select((method, _) => method!.Value);

            var componentsOrderChanged = fullNameOrContextChanged
                .Collect()
                .Select((components, _) => components
                    .OrderBy(component => component.FullName)
                    .ToImmutableArray());

            var lookupChanged = contextInitializationMethods.Combine(componentsOrderChanged);
            initContext.RegisterSourceOutput(lookupChanged, OnLookupChanged);
        }

        static bool IsComponentCandidate(SyntaxNode node, CancellationToken _)
        {
            return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                   && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.SealedKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.PartialKeyword);
        }

        static ImmutableArray<ComponentDeclaration> CreateComponentDeclarations(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
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
                .Where(attribute => attribute?.HasAttributeSuffix(".Context") ?? false)
                .Select(attribute => attribute!.RemoveAttributeSuffix(".Context"))
                .Distinct()
                .Select(attribute => new ComponentDeclaration(symbol, attribute, cancellationToken))
                .ToImmutableArray();
        }

        static bool IsContextInitializationMethodCandidate(SyntaxNode node, CancellationToken _)
        {
            return node is MethodDeclarationSyntax { AttributeLists.Count: > 0 } candidate
                   && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.PartialKeyword)
                   && candidate.ReturnType is PredefinedTypeSyntax predefined
                   && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword);
        }

        static ContextInitializationMethodDeclaration? CreateContextInitializationMethodDeclaration(GeneratorSyntaxContext syntaxContext, CancellationToken cancellationToken)
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
                    .HasAttributeSuffix(".ContextInitialization") ?? false);

            if (attribute is null)
                return null;

            return new ContextInitializationMethodDeclaration(symbol, attribute);
        }

        static void OnFullNameOrContextChanged(SourceProductionContext spc, ComponentDeclaration component)
        {
            ComponentIndex(spc, component);
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

        static void OnFullNameOrMembersOrContextChanged(SourceProductionContext spc, ComponentDeclaration component)
        {
            EntityExtension(spc, component);
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

        static void OnLookupChanged(SourceProductionContext spc, (ContextInitializationMethodDeclaration Left, ImmutableArray<ComponentDeclaration> Right) pair)
        {
            var (method, components) = pair;
            var componentsForContext = components
                .Where(component => component.Context == method.FullContextPrefix)
                .ToImmutableArray();

            ContextInitializationMethod(spc, method, componentsForContext);
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
