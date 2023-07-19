using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Entitas.Generators
{
    public readonly struct ComponentDeclaration
    {
        public readonly SyntaxTree? SyntaxTree;
        public readonly string? Namespace;
        public readonly string FullName;

        public readonly string Name;

        public readonly ImmutableArray<MemberDeclaration> Members;
        public readonly ImmutableArray<string> Contexts;
        public readonly bool IsUnique;
        public readonly int CleanupMode;
        public readonly ImmutableArray<EventDeclaration> Events;
        public readonly string FullPrefix;
        public readonly string Prefix;

        public ComponentDeclaration(SyntaxTree? syntaxTree, INamedTypeSymbol symbol, ImmutableArray<string> contexts, CancellationToken cancellationToken)
        {
            SyntaxTree = syntaxTree;
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

            Contexts = contexts;
            IsUnique = symbol.GetAttributes().Any(static attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.UniqueAttribute");

            var cleanupMode = symbol
                .GetAttributes()
                .FirstOrDefault(static attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.CleanupAttribute")?
                .ConstructorArguments.FirstOrDefault();

            CleanupMode = cleanupMode?.Type?.ToDisplayString() == "Entitas.Generators.Attributes.CleanupMode" && cleanupMode.Value.Value is int mode ? mode : -1;

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

            FullPrefix = FullName.Replace(".", string.Empty).RemoveSuffix("Component");
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
            SyntaxTree = component.SyntaxTree;
            Namespace = component.Namespace;
            FullName = fullName;
            Name = name;
            Members = members;
            Contexts = component.Contexts;
            IsUnique = false;
            CleanupMode = -1;
            Events = ImmutableArray<EventDeclaration>.Empty;
            FullPrefix = prefix;
            Prefix = prefix;
        }

        public ComponentDeclaration ToEvent(string fullName, string name, ImmutableArray<MemberDeclaration> members, string componentPrefix)
        {
            return new ComponentDeclaration(this, fullName, name, members, componentPrefix);
        }

        public string ContextAwareComponentPrefix(string contextPrefix)
        {
            return contextPrefix.Replace(".", string.Empty) + Prefix;
        }
    }

    public class FullNameAndContextsComparer : IEqualityComparer<ComponentDeclaration>
    {
        public static readonly FullNameAndContextsComparer Instance = new FullNameAndContextsComparer();

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

    public class FullNameAndMembersAndContextsComparer : IEqualityComparer<ComponentDeclaration>
    {
        readonly IEqualityComparer<MemberDeclaration> _memberComparer;

        public FullNameAndMembersAndContextsComparer(IEqualityComparer<MemberDeclaration> memberComparer)
        {
            _memberComparer = memberComparer;
        }

        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Members.SequenceEqual(y.Members, _memberComparer) &&
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

    public class FullNameAndMembersAndContextsAndIsUniqueComparer : IEqualityComparer<ComponentDeclaration>
    {
        readonly IEqualityComparer<MemberDeclaration> _memberComparer;

        public FullNameAndMembersAndContextsAndIsUniqueComparer(IEqualityComparer<MemberDeclaration> memberComparer)
        {
            _memberComparer = memberComparer;
        }

        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Members.SequenceEqual(y.Members, _memberComparer) &&
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

    public class FullNameAndContextsAndCleanupModeComparer : IEqualityComparer<ComponentDeclaration>
    {
        public static readonly FullNameAndContextsAndCleanupModeComparer Instance = new FullNameAndContextsAndCleanupModeComparer();

        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Contexts.SequenceEqual(y.Contexts) &&
            x.CleanupMode == y.CleanupMode;

        public int GetHashCode(ComponentDeclaration obj)
        {
            unchecked
            {
                var hashCode = obj.FullName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Contexts.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.CleanupMode.GetHashCode();
                return hashCode;
            }
        }
    }

    public class FullNameAndMembersAndContextsAndEventsComparer : IEqualityComparer<ComponentDeclaration>
    {
        readonly IEqualityComparer<MemberDeclaration> _memberComparer;
        readonly IEqualityComparer<EventDeclaration> _eventComparer;

        public FullNameAndMembersAndContextsAndEventsComparer(IEqualityComparer<MemberDeclaration> memberComparer, IEqualityComparer<EventDeclaration> eventComparer)
        {
            _memberComparer = memberComparer;
            _eventComparer = eventComparer;
        }

        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Members.SequenceEqual(y.Members, _memberComparer) &&
            x.Contexts.SequenceEqual(y.Contexts) &&
            x.Events.SequenceEqual(y.Events, _eventComparer);

        public int GetHashCode(ComponentDeclaration obj)
        {
            unchecked
            {
                var hashCode = obj.FullName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Members.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Contexts.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Events.GetHashCode();
                return hashCode;
            }
        }
    }

    public class FullNameAndContextsAndEventsComparer : IEqualityComparer<ComponentDeclaration>
    {
        readonly IEqualityComparer<EventDeclaration> _eventComparer;

        public FullNameAndContextsAndEventsComparer(IEqualityComparer<EventDeclaration> eventComparer)
        {
            _eventComparer = eventComparer;
        }

        public bool Equals(ComponentDeclaration x, ComponentDeclaration y) =>
            x.FullName == y.FullName &&
            x.Contexts.SequenceEqual(y.Contexts) &&
            x.Events.SequenceEqual(y.Events, _eventComparer);

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

    public class ImmutableArrayComparer : IEqualityComparer<ImmutableArray<ComponentDeclaration>>
    {
        readonly IEqualityComparer<ComponentDeclaration> _comparer;

        public ImmutableArrayComparer(IEqualityComparer<ComponentDeclaration> comparer)
        {
            _comparer = comparer;
        }

        public bool Equals(ImmutableArray<ComponentDeclaration> x, ImmutableArray<ComponentDeclaration> y) =>
            x.SequenceEqual(y, _comparer);

        public int GetHashCode(ImmutableArray<ComponentDeclaration> obj) => obj.GetHashCode();
    }
}
