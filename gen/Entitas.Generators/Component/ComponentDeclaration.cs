using System;
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
        public readonly string Context;
        public readonly bool IsUnique;

        // Computed
        public readonly string ComponentPrefix;
        public readonly string ContextPrefix;
        public readonly string ContextAwareComponentPrefix;

        public ComponentDeclaration(INamedTypeSymbol symbol, string context, CancellationToken cancellationToken)
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

            Context = context;
            IsUnique = symbol.GetAttributes().Any(static attribute =>
                attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.UniqueAttribute");

            // Computed
            ComponentPrefix = Name.RemoveSuffix("Component");
            ContextPrefix = context.RemoveSuffix("Context");
            ContextAwareComponentPrefix = ContextPrefix.Replace(".", string.Empty) + ComponentPrefix;

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

    class FullNameAndContextComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Context == y.Context;

        public int GetHashCode(ComponentDeclaration component) =>
            HashCode.Combine(component.FullName, component.Context);
    }

    class FullNameAndMembersAndContextComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Members.SequenceEqual(y.Members) &&
            x.Context == y.Context;

        public int GetHashCode(ComponentDeclaration component) =>
            HashCode.Combine(component.FullName, component.Members, component.Context);
    }

    class FullNameAndMembersAndContextAndIsUniqueComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Members.SequenceEqual(y.Members) &&
            x.Context == y.Context &&
            x.IsUnique == y.IsUnique;

        public int GetHashCode(ComponentDeclaration component) =>
            HashCode.Combine(component.FullName, component.Members, component.Context, component.IsUnique);
    }
}
