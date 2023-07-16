using System;
using Microsoft.CodeAnalysis;

namespace Entitas.Generators
{
    readonly struct ContextInitializationMethodDeclaration : IEquatable<ContextInitializationMethodDeclaration>
    {
        public readonly SyntaxTree SyntaxTree;
        public readonly string? Namespace;
        public readonly string Class;
        public readonly string Name;
        public readonly string? ContextNamespace;
        public readonly string ContextFullName;
        public readonly string ContextName;
        public readonly string FullContextPrefix;

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

        public bool Equals(ContextInitializationMethodDeclaration other) =>
            Namespace == other.Namespace &&
            Class == other.Class &&
            Name == other.Name &&
            ContextFullName == other.ContextFullName;

        public override bool Equals(object? obj) => obj is ContextInitializationMethodDeclaration other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Namespace != null ? Namespace.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Class.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();
                hashCode = (hashCode * 397) ^ ContextFullName.GetHashCode();
                return hashCode;
            }
        }
    }
}
