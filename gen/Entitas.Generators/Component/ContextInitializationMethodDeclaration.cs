using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace Entitas.Generators
{
    readonly struct ContextInitializationMethodDeclaration : IEquatable<ContextInitializationMethodDeclaration>
    {
        public readonly string? Namespace;
        public readonly string Class;
        public readonly string Name;
        public readonly string Context;
        public readonly ImmutableArray<ComponentDeclaration> Components;

        // Computed
        public readonly string FullContextPrefix;

        readonly FullNameAndContextComparer _comparer = new FullNameAndContextComparer();

        public ContextInitializationMethodDeclaration(IMethodSymbol symbol, string context, ImmutableArray<ComponentDeclaration> components)
        {
            Namespace = !symbol.ContainingNamespace.IsGlobalNamespace
                ? symbol.ContainingNamespace.ToDisplayString()
                : null;

            Class = symbol.ContainingType.Name;
            Name = symbol.Name;
            Context = context;
            Components = components;

            // Computed
            FullContextPrefix = context.RemoveSuffix("Context");
        }

        public bool Equals(ContextInitializationMethodDeclaration other) =>
            Namespace == other.Namespace &&
            Class == other.Class &&
            Name == other.Name &&
            Context == other.Context &&
            Components.SequenceEqual(other.Components, _comparer);

        public override bool Equals(object? obj) => obj is ContextInitializationMethodDeclaration other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (Namespace != null ? Namespace.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ Class.GetHashCode();
                hashCode = (hashCode * 397) ^ Name.GetHashCode();
                hashCode = (hashCode * 397) ^ Context.GetHashCode();
                hashCode = (hashCode * 397) ^ Components.GetHashCode();
                return hashCode;
            }
        }
    }
}
