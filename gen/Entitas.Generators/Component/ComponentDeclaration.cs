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
        public readonly SyntaxNode? Node;
        public readonly string? Namespace;
        public readonly string FullName;
        public readonly string Name;
        public readonly ImmutableArray<MemberDeclaration> Members;
        public readonly ImmutableArray<string> Contexts;
        public readonly bool IsUnique;
        public readonly ImmutableArray<EventDeclaration> Events;
        public readonly string Prefix;

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

        public ComponentDeclaration(SyntaxNode? node, INamedTypeSymbol symbol, CancellationToken cancellationToken)
        {
            Node = node;
            Namespace = !symbol.ContainingNamespace.IsGlobalNamespace ? symbol.ContainingNamespace.ToDisplayString() : null;
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
            IsUnique = symbol.GetAttributes().Any(static attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.UniqueAttribute");

            var prefix = Name.RemoveSuffix("Component");

            Events = symbol.GetAttributes()
                .Where(static attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.EventAttribute")
                .Select(static attribute => attribute.ConstructorArguments)
                .Select<ImmutableArray<TypedConstant>, EventDeclaration?>(args =>
                {
                    var eventTarget = args.Length > 0 && args[0].Type?.ToDisplayString() == "Entitas.Generators.Attributes.EventTarget" && args[0].Value is int eventTargetValue ? eventTargetValue : -1;
                    if (eventTarget == -1)
                        return null;

                    var eventType = args.Length > 1 && args[1].Type?.ToDisplayString() == "Entitas.Generators.Attributes.EventType" && args[1].Value is int eventTypeValue ? eventTypeValue : 0;
                    var order = args.Length > 2 && args[2].Type?.ToDisplayString() == "int" && args[2].Value is int orderValue ? orderValue : 0;
                    return new EventDeclaration(eventTarget, eventType, order, prefix);
                })
                .OfType<EventDeclaration>()
                .ToImmutableArray();

            Prefix = prefix;

            static bool IsAutoProperty(ISymbol symbol, CancellationToken cancellationToken)
            {
                return symbol is IPropertySymbol { SetMethod: not null, GetMethod: not null } property
                       && !property.GetMethod?.DeclaringSyntaxReferences.FirstOrDefault()?
                           .GetSyntax(cancellationToken).DescendantNodes().Any(static node => node is MethodDeclarationSyntax) == true
                       && !property.SetMethod?.DeclaringSyntaxReferences.FirstOrDefault()?
                           .GetSyntax(cancellationToken).DescendantNodes().Any(static node => node is MethodDeclarationSyntax) == true;
            }
        }

        ComponentDeclaration(ComponentDeclaration component, string fullName, string name, ImmutableArray<MemberDeclaration> members, string prefix)
        {
            Node = component.Node;
            Namespace = component.Namespace;
            FullName = fullName;
            Name = name;
            Members = members;
            Contexts = component.Contexts;
            IsUnique = false;
            Events = ImmutableArray<EventDeclaration>.Empty;
            Prefix = prefix;
        }

        internal ComponentDeclaration ToEvent(string fullName, string name, ImmutableArray<MemberDeclaration> members, string componentPrefix)
        {
            return new ComponentDeclaration(this, fullName, name, members, componentPrefix);
        }

        internal string ContextPrefix(string context)
        {
            return context.RemoveSuffix("Context");
        }

        internal string ContextAware(string contextPrefix)
        {
            return contextPrefix.Replace(".", string.Empty);
        }

        internal string ContextAwareComponentPrefix(string contextPrefix)
        {
            return ContextAware(contextPrefix) + Prefix;
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

    class FullNameAndContextsAndEventsComparer : IEqualityComparer<ComponentDeclaration>
    {
        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Contexts.SequenceEqual(y.Contexts) &&
            x.Events.SequenceEqual(y.Events);

        public int GetHashCode(ComponentDeclaration obj)
        {
            unchecked
            {
                var hashCode = obj.FullName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Contexts.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Events.GetHashCode();
                return hashCode;
            }
        }
    }
}
