using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Entitas.Generators
{
    public struct ContextInitializationMethodDeclaration
    {
        public readonly SyntaxTree SyntaxTree;
        public readonly string? Namespace;
        public readonly string Class;
        public readonly string Name;
        public readonly string? ContextNamespace;
        public readonly string ContextFullName;
        public readonly string ContextName;
        public readonly string FullContextPrefix;

        public ImmutableArray<ComponentDeclaration> Components;

        public ContextInitializationMethodDeclaration(SyntaxTree syntaxTree, IMethodSymbol symbol, ISymbol contextSymbol)
        {
            SyntaxTree = syntaxTree;
            Namespace = !symbol.ContainingNamespace.IsGlobalNamespace
                ? symbol.ContainingNamespace.ToDisplayString()
                : null;

            Class = symbol.ContainingType.Name;
            Name = symbol.Name;

            ContextNamespace = !contextSymbol.ContainingNamespace.IsGlobalNamespace
                ? symbol.ContainingNamespace.ToDisplayString()
                : null;
            ContextFullName = contextSymbol.ToDisplayString();
            ContextName = contextSymbol.Name;

            FullContextPrefix = ContextFullName.RemoveSuffix("Context");
        }
    }

    public class NamespaceAndClassAndNameAndContextFullNameComparer : IEqualityComparer<ContextInitializationMethodDeclaration>
    {
        public static readonly NamespaceAndClassAndNameAndContextFullNameComparer Instance = new NamespaceAndClassAndNameAndContextFullNameComparer();

        public bool Equals(ContextInitializationMethodDeclaration x, ContextInitializationMethodDeclaration y) =>
            x.Namespace == y.Namespace &&
            x.Class == y.Class &&
            x.Name == y.Name &&
            x.ContextFullName == y.ContextFullName;

        public int GetHashCode(ContextInitializationMethodDeclaration obj)
        {
            unchecked
            {
                var hashCode = obj.Namespace != null ? obj.Namespace.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ obj.Class.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Name.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.ContextFullName.GetHashCode();
                return hashCode;
            }
        }
    }

    public class NamespaceAndClassAndNameAndContextFullNameAndComponentsComparer : IEqualityComparer<ContextInitializationMethodDeclaration>
    {
        readonly IEqualityComparer<ComponentDeclaration> _componentComparer;

        public NamespaceAndClassAndNameAndContextFullNameAndComponentsComparer(IEqualityComparer<ComponentDeclaration> componentComparer)
        {
            _componentComparer = componentComparer;
        }

        public bool Equals(ContextInitializationMethodDeclaration x, ContextInitializationMethodDeclaration y) =>
            x.Namespace == y.Namespace &&
            x.Class == y.Class &&
            x.Name == y.Name &&
            x.ContextFullName == y.ContextFullName &&
            x.Components.SequenceEqual(y.Components, _componentComparer);

        public int GetHashCode(ContextInitializationMethodDeclaration obj)
        {
            unchecked
            {
                var hashCode = (obj.Namespace != null ? obj.Namespace.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.Class.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Name.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.ContextFullName.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Components.GetHashCode();
                return hashCode;
            }
        }
    }
}
