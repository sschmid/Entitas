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
        public readonly string ComponentPrefix;
        public readonly string ContextAwareComponentPrefix;
        public readonly bool IsUnique;

        public ComponentDeclaration(INamedTypeSymbol symbol, string context, CancellationToken cancellationToken)
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

            Context = context;

            ComponentPrefix = Name.RemoveSuffix("Component");
            ContextAwareComponentPrefix = Context.Replace(".", string.Empty) + ComponentPrefix;

            IsUnique = symbol.GetAttributes().Any(attribute =>
                attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.UniqueAttribute");

            static bool IsAutoProperty(ISymbol symbol, CancellationToken cancellationToken)
            {
                return symbol is IPropertySymbol { SetMethod: not null, GetMethod: not null } property
                       && !property.GetMethod?.DeclaringSyntaxReferences.FirstOrDefault()?
                           .GetSyntax(cancellationToken).DescendantNodes().Any(node => node is MethodDeclarationSyntax) == true
                       && !property.SetMethod?.DeclaringSyntaxReferences.FirstOrDefault()?
                           .GetSyntax(cancellationToken).DescendantNodes().Any(node => node is MethodDeclarationSyntax) == true;
            }
        }
    }

    class FullNameAndContextComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y)
        {
            return
                x.FullName == y.FullName &&
                x.Context == y.Context;
        }

        public int GetHashCode(ComponentDeclaration component)
        {
            return HashCode.Combine(component.FullName, component.Context);
        }
    }

    class FullNameAndMembersAndContextComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y)
        {
            return
                x.FullName == y.FullName &&
                x.Members.SequenceEqual(y.Members) &&
                x.Context == y.Context;
        }

        public int GetHashCode(ComponentDeclaration component)
        {
            return HashCode.Combine(component.FullName, component.Members, component.Context);
        }
    }

    class FullNameAndMembersAndContextAndIsUniqueComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y)
        {
            return
                x.FullName == y.FullName &&
                x.Members.SequenceEqual(y.Members) &&
                x.Context == y.Context &&
                x.IsUnique == y.IsUnique;
        }

        public int GetHashCode(ComponentDeclaration component)
        {
            return HashCode.Combine(component.FullName, component.Members, component.Context, component.IsUnique);
        }
    }

    class FullNameAndContextCompilationComparer : IEqualityComparer<ImmutableArray<ComponentDeclaration>>
    {
        readonly FullNameAndContextComparer _comparer = new FullNameAndContextComparer();

        public bool Equals(ImmutableArray<ComponentDeclaration> x, ImmutableArray<ComponentDeclaration> y)
        {
            return x.SequenceEqual(y, _comparer);
        }

        public int GetHashCode(ImmutableArray<ComponentDeclaration> components)
        {
            return components.GetHashCode();
        }
    }
}
