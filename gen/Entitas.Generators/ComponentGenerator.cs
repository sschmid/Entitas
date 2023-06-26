using System;
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
            var componentProvider = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsComponentCandidate, CreateComponentDeclaration)
                .Where(component => component is not null)
                .Select((component, _) => component!.Value)
                .Collect();

            var contextInitializationMethodProvider = initContext.SyntaxProvider
                .CreateSyntaxProvider(IsContextInitializationMethodCandidate, CreateContextInitializationMethodDeclaration)
                .Where(method => method is not null)
                .Select((method, _) => method!.Value)
                .Collect();

            var combined = contextInitializationMethodProvider.Combine(componentProvider);
            initContext.RegisterSourceOutput(combined, Execute);
        }

        static bool IsComponentCandidate(SyntaxNode node, CancellationToken cancellationToken)
        {
            return node is ClassDeclarationSyntax { BaseList.Types.Count: > 0 } candidate
                   && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.SealedKeyword)
                   && !candidate.Modifiers.Any(SyntaxKind.PartialKeyword);
        }

        static ComponentDeclaration? CreateComponentDeclaration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var candidate = (ClassDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
            if (symbol is null)
                return null;

            // Todo: Emit diagnostics when interface is not found
            var interfaceType = context.SemanticModel.Compilation.GetTypeByMetadataName("Entitas.IComponent");
            if (interfaceType is null)
                return null;

            var isComponent = symbol.Interfaces.Any(i => i.OriginalDefinition.Equals(interfaceType, SymbolEqualityComparer.Default));
            if (!isComponent)
                return null;

            return new ComponentDeclaration(symbol, cancellationToken);
        }

        static bool IsContextInitializationMethodCandidate(SyntaxNode node, CancellationToken cancellationToken)
        {
            return node is MethodDeclarationSyntax { AttributeLists.Count: > 0 } candidate
                   && candidate.Modifiers.Any(SyntaxKind.PublicKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.StaticKeyword)
                   && candidate.Modifiers.Any(SyntaxKind.PartialKeyword)
                   && candidate.ReturnType is PredefinedTypeSyntax predefined
                   && predefined.Keyword.IsKind(SyntaxKind.VoidKeyword);
        }

        static ContextInitializationMethodDeclaration? CreateContextInitializationMethodDeclaration(GeneratorSyntaxContext context, CancellationToken cancellationToken)
        {
            var candidate = (MethodDeclarationSyntax)context.Node;
            var symbol = context.SemanticModel.GetDeclaredSymbol(candidate, cancellationToken);
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
        }

        void Execute(SourceProductionContext spc, (ImmutableArray<ContextInitializationMethodDeclaration> Methods, ImmutableArray<ComponentDeclaration> Components) arg)
        {
            foreach (var component in arg.Components)
            foreach (var context in component.Contexts)
            {
                ComponentIndex(spc, component, context);
                EntityExtension(spc, component, context);
            }

            foreach (var method in arg.Methods)
            {
                var orderedComponents = arg.Components
                    .Where(component => component.Contexts.Contains(method.Context.RemoveSuffix("Context")))
                    .OrderBy(component => component.FullName)
                    .ToImmutableArray();

                ContextInitializationMethod(spc, method, orderedComponents);
            }
        }

        static void ComponentIndex(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            var className = $"{component.FullComponentPrefix}ComponentIndex";
            spc.AddSource(
                GeneratedPath($"{context}.{className}"),
                GeneratedFileHeader(GeneratorSource(nameof(ComponentIndex))) +
                NamespaceDeclaration(context,
                    $$"""
                    public static class {{className}}
                    {
                        public static ComponentIndex Index;
                    }

                    """));
        }

        static void EntityExtension(SourceProductionContext spc, ComponentDeclaration component, string context)
        {
            var className = $"{component.FullComponentPrefix}EntityExtension";
            if (component.Members.Length > 0)
            {
                spc.AddSource(GeneratedPath($"{context}.{className}"),
                    GeneratedFileHeader(GeneratorSource(nameof(EntityExtension))) +
                    $"using static {context}.{component.FullComponentPrefix}ComponentIndex;\n\n" +
                    NamespaceDeclaration(context,
                        $$"""
                        public static class {{className}}
                        {
                            public static bool Has{{component.ComponentPrefix}}(this Entity entity)
                            {
                                return entity.HasComponent(Index.Value);
                            }

                            public static Entity Add{{component.ComponentPrefix}}(this Entity entity, {{ComponentMethodArgs(component)}})
                            {
                                var index = Index.Value;
                                var component = ({{component.FullName}})entity.CreateComponent(index, typeof({{component.FullName}}));
                        {{ComponentValueAssignments(component)}}
                                entity.AddComponent(index, component);
                                return entity;
                            }

                            public static Entity Replace{{component.ComponentPrefix}}(this Entity entity, {{ComponentMethodArgs(component)}})
                            {
                                var index = Index.Value;
                                var component = ({{component.FullName}})entity.CreateComponent(index, typeof({{component.FullName}}));
                        {{ComponentValueAssignments(component)}}
                                entity.ReplaceComponent(index, component);
                                return entity;
                            }

                            public static Entity Remove{{component.ComponentPrefix}}(this Entity entity)
                            {
                                entity.RemoveComponent(Index.Value);
                                return entity;
                            }

                            public static {{component.FullName}} Get{{component.ComponentPrefix}}(this Entity entity)
                            {
                                return ({{component.FullName}})entity.GetComponent(Index.Value);
                            }

                            public static void Deconstruct(this {{component.FullName}} component, {{ComponentDeconstructMethodArgs(component)}})
                            {
                        {{ComponentDeconstructValueAssignments(component)}}
                            }
                        }

                        """));

                static string ComponentDeconstructMethodArgs(ComponentDeclaration component)
                {
                    return string.Join(", ", component.Members.Select(member => $"out {member.Type} {member.ValidLowerFirstName}"));
                }

                static string ComponentDeconstructValueAssignments(ComponentDeclaration component)
                {
                    return string.Join("\n", component.Members.Select(member =>
                        $$"""
                                {{member.ValidLowerFirstName}} = component.{{member.Name}};
                        """));
                }

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
                spc.AddSource(GeneratedPath($"{context}.{className}"),
                    GeneratedFileHeader(GeneratorSource(nameof(EntityExtension))) +
                    $"using static {context}.{component.FullComponentPrefix}ComponentIndex;\n\n" +
                    NamespaceDeclaration(context,
                        $$"""
                        public static class {{className}}
                        {
                            static readonly {{component.FullName}} Single{{component.Name}} = new {{component.FullName}}();

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

                            public static {{component.FullName}} Get{{component.ComponentPrefix}}(this Entity entity)
                            {
                                return ({{component.FullName}})entity.GetComponent(Index.Value);
                            }
                        }

                        """));
            }
        }

        static void ContextInitializationMethod(SourceProductionContext spc, ContextInitializationMethodDeclaration method, ImmutableArray<ComponentDeclaration> components)
        {
            spc.AddSource(
                GeneratedPath($"{method.Context}.ContextInitializationMethod"),
                GeneratedFileHeader(GeneratorSource(nameof(ContextInitializationMethod))) +
                $"using {method.Context.RemoveSuffix("Context")};\n\n" +
                NamespaceDeclaration(method.Namespace,
                    $$"""
                    public static partial class {{method.Class}}
                    {
                        public static partial void {{method.Name}}()
                        {
                    {{ComponentIndexAssignments(components)}}

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

            static string ComponentIndexAssignments(ImmutableArray<ComponentDeclaration> components)
            {
                return string.Join("\n", components.Select((component, i) =>
                    $"        {component.FullComponentPrefix}ComponentIndex.Index = new ComponentIndex({i});"));
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

        public readonly struct ComponentDeclaration : IEquatable<ComponentDeclaration>
        {
            public readonly string? Namespace;
            public readonly string FullName;
            public readonly string Name;

            public readonly ImmutableArray<MemberDeclaration> Members;
            public readonly ImmutableArray<string> Contexts;

            public readonly Location Location;

            public readonly string FullComponentPrefix;
            public readonly string ComponentPrefix;

            public ComponentDeclaration(INamedTypeSymbol symbol, CancellationToken cancellationToken)
            {
                Namespace = !symbol.ContainingNamespace.IsGlobalNamespace ? symbol.ContainingNamespace.ToDisplayString() : null;
                FullName = symbol.ToDisplayString();
                Name = symbol.Name;

                Members = symbol.GetMembers()
                    .Where(member => member.DeclaredAccessibility == Accessibility.Public
                                     && !member.IsStatic
                                     && member.CanBeReferencedByName
                                     && (member is IFieldSymbol || IsAutoProperty(member, cancellationToken)))
                    .Select<ISymbol, MemberDeclaration?>(member => member switch
                    {
                        IFieldSymbol field => new MemberDeclaration(field),
                        IPropertySymbol property => new MemberDeclaration(property),
                        _ => null
                    })
                    .Where(member => member is not null)
                    .Select(member => member!.Value)
                    .ToImmutableArray();

                Contexts = symbol.GetAttributes()
                    .Select(attribute => attribute.AttributeClass?.ToDisplayString())
                    .Where(attribute => attribute?.EndsWith(".Context") ?? false)
                    .Select(attribute => attribute!.RemoveSuffix(".Context"))
                    .ToImmutableArray();

                Location = symbol.Locations.FirstOrDefault() ?? Location.None;

                FullComponentPrefix = FullName.Replace(".", string.Empty).RemoveSuffix("Component");
                ComponentPrefix = Name.RemoveSuffix("Component");

                static bool IsAutoProperty(ISymbol symbol, CancellationToken cancellationToken)
                {
                    return symbol is IPropertySymbol { SetMethod: not null, GetMethod: not null } property
                           && !property.GetMethod.DeclaringSyntaxReferences.First()
                               .GetSyntax(cancellationToken).DescendantNodes().Any(node => node is MethodDeclarationSyntax)
                           && !property.SetMethod.DeclaringSyntaxReferences.First()
                               .GetSyntax(cancellationToken).DescendantNodes().Any(node => node is MethodDeclarationSyntax);
                }
            }

            public bool Equals(ComponentDeclaration other) =>
                Namespace == other.Namespace &&
                FullName == other.FullName &&
                Name == other.Name &&
                Members.SequenceEqual(other.Members) &&
                Contexts.SequenceEqual(other.Contexts);

            public override bool Equals(object? obj) => obj is ComponentDeclaration other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(Namespace, FullName, Name, Members, Contexts);
        }

        public readonly struct MemberDeclaration : IEquatable<MemberDeclaration>
        {
            public readonly string Type;
            public readonly string Name;

            public readonly string ValidLowerFirstName;

            public MemberDeclaration(IFieldSymbol field) : this(field.Type.ToDisplayString(), field) { }

            public MemberDeclaration(IPropertySymbol property) : this(property.Type.ToDisplayString(), property) { }

            public MemberDeclaration(string type, ISymbol symbol)
            {
                Type = type;
                Name = symbol.Name;
                ValidLowerFirstName = ToValidLowerFirst(Name);
            }

            static string ToValidLowerFirst(string value)
            {
                var lowerFirst = char.ToLower(value[0]) + value[1..];
                return SyntaxFacts.GetKeywordKind(lowerFirst) != SyntaxKind.None
                    ? $"@{lowerFirst}"
                    : lowerFirst;
            }

            public bool Equals(MemberDeclaration other) => Type == other.Type && Name == other.Name;
            public override bool Equals(object? obj) => obj is MemberDeclaration other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(Type, Name);
        }

        public readonly struct ContextInitializationMethodDeclaration : IEquatable<ContextInitializationMethodDeclaration>
        {
            public readonly string? Namespace;
            public readonly string Class;
            public readonly string Name;
            public readonly string Context;

            public readonly Location Location;

            public ContextInitializationMethodDeclaration(IMethodSymbol symbol, AttributeData attribute)
            {
                Namespace = !symbol.ContainingNamespace.IsGlobalNamespace ? symbol.ContainingNamespace.ToDisplayString() : null;
                Class = symbol.ContainingType.Name;
                Name = symbol.Name;
                Context = attribute.AttributeClass!.ToDisplayString().RemoveSuffix(".ContextInitialization") + "Context";

                Location = symbol.Locations.FirstOrDefault() ?? Location.None;
            }

            public bool Equals(ContextInitializationMethodDeclaration other) =>
                Namespace == other.Namespace &&
                Class == other.Class &&
                Name == other.Name &&
                Context == other.Context;

            public override bool Equals(object? obj) => obj is ContextInitializationMethodDeclaration other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(Namespace, Class, Name, Context);
        }
    }
}
