using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Entitas.Generators
{
    readonly struct ComponentDeclaration
    {
        public readonly string? Namespace;
        public readonly string FullName;
        public readonly string Name;
        public readonly ImmutableArray<MemberDeclaration> Members;
        public readonly ImmutableArray<string> Contexts;
        public readonly bool IsUnique;

        // Computed
        public readonly string ComponentPrefix;

        internal static ImmutableArray<string> GetContexts(INamedTypeSymbol symbol)
        {
            return symbol.GetAttributes()
                .Where(static attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.ContextAttribute")
                .Select(static attribute => attribute.ConstructorArguments.SingleOrDefault())
                .Where(static arg => arg.Type?.ToDisplayString() == "System.Type" && arg.Value is INamedTypeSymbol)
                .Select(static arg => ((INamedTypeSymbol)arg.Value!).ToDisplayString())
                .Distinct()
                .ToImmutableArray();
        }

        public ComponentDeclaration(INamedTypeSymbol symbol, CancellationToken cancellationToken)
        {
            Namespace = !symbol.ContainingNamespace.IsGlobalNamespace
                ? symbol.ContainingNamespace.ToDisplayString()
                : null;

            FullName = symbol.ToDisplayString();
            Name = symbol.Name;

            Members = symbol.GetMembers()
                .Where(member => member.DeclaredAccessibility == Accessibility.Public
                                 && !member.IsStatic
                                 && member.CanBeReferencedByName
                                 && (member is IFieldSymbol || IsAutoProperty(member, cancellationToken)))
                .Select<ISymbol, MemberDeclaration?>(static member => member switch
                {
                    IFieldSymbol field => new MemberDeclaration(field),
                    IPropertySymbol property => new MemberDeclaration(property),
                    _ => null
                })
                .OfType<MemberDeclaration>()
                .ToImmutableArray();

            Contexts = GetContexts(symbol);

            IsUnique = symbol.GetAttributes().Any(static attribute =>
                attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.UniqueAttribute");

            // Computed
            ComponentPrefix = Name.RemoveSuffix("Component");

            static bool IsAutoProperty(ISymbol symbol, CancellationToken cancellationToken)
            {
                return symbol is IPropertySymbol { SetMethod: not null, GetMethod: not null } property
                       && !property.GetMethod?.DeclaringSyntaxReferences.FirstOrDefault()?
                           .GetSyntax(cancellationToken).DescendantNodes().Any(static node => node is MethodDeclarationSyntax) == true
                       && !property.SetMethod?.DeclaringSyntaxReferences.FirstOrDefault()?
                           .GetSyntax(cancellationToken).DescendantNodes().Any(static node => node is MethodDeclarationSyntax) == true;
            }
        }
    }

    class FullNameAndContextsComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Contexts.SequenceEqual(y.Contexts);

        public int GetHashCode(ComponentDeclaration obj)
        {
            unchecked
            {
                return (obj.FullName.GetHashCode() * 397) ^ obj.Contexts.GetHashCode();
            }
        }
    }

    class FullNameAndMembersAndContextsComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Members.SequenceEqual(y.Members) &&
            x.Contexts.SequenceEqual(y.Contexts);

        public int GetHashCode(ComponentDeclaration obj)
        {
            unchecked
            {
                var hashCode = obj.FullName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Members.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Contexts.GetHashCode();
                return hashCode;
            }
        }
    }

    class FullNameAndMembersAndContextsAndIsUniqueComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Members.SequenceEqual(y.Members) &&
            x.Contexts.SequenceEqual(y.Contexts) &&
            x.IsUnique == y.IsUnique;

        public int GetHashCode(ComponentDeclaration obj)
        {
            unchecked
            {
                var hashCode = obj.FullName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Members.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Contexts.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.IsUnique.GetHashCode();
                return hashCode;
            }
        }
    }
}
