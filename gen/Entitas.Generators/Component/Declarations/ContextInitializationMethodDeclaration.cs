using System;
using Microsoft.CodeAnalysis;

namespace Entitas.Generators
{
    readonly struct ContextInitializationMethodDeclaration : IEquatable<ContextInitializationMethodDeclaration>
    {
        public readonly string? Namespace;
        public readonly string Class;
        public readonly string Name;
        public readonly string Context;

        public readonly string FullContextPrefix;

        public ContextInitializationMethodDeclaration(IMethodSymbol symbol, AttributeData attribute)
        {
            Namespace = !symbol.ContainingNamespace.IsGlobalNamespace ? symbol.ContainingNamespace.ToDisplayString() : null;
            Class = symbol.ContainingType.Name;
            Name = symbol.Name;

            FullContextPrefix = attribute.AttributeClass!.ToDisplayString().RemoveAttributeSuffix(".ContextInitialization");

            Context = FullContextPrefix + "Context";
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
