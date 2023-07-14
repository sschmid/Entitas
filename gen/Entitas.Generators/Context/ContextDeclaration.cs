using System;
using Microsoft.CodeAnalysis;

namespace Entitas.Generators
{
    readonly struct ContextDeclaration : IEquatable<ContextDeclaration>
    {
        public readonly SyntaxNode Node;
        public readonly string? Namespace;
        public readonly string FullName;
        public readonly string Name;

        public readonly string FullContextPrefix;
        public readonly string ContextPrefix;

        public ContextDeclaration(SyntaxNode node, INamedTypeSymbol symbol)
        {
            Node = node;
            Namespace = !symbol.ContainingNamespace.IsGlobalNamespace ? symbol.ContainingNamespace.ToDisplayString() : null;
            FullName = symbol.ToDisplayString();
            Name = symbol.Name;

            FullContextPrefix = FullName.RemoveSuffix("Context");
            ContextPrefix = Name.RemoveSuffix("Context");
        }

        public bool Equals(ContextDeclaration other) => FullName == other.FullName;
        public override bool Equals(object? obj) => obj is ContextDeclaration other && Equals(other);
        public override int GetHashCode() => FullName.GetHashCode();
    }
}
