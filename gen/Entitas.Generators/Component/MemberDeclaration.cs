using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Entitas.Generators
{
    public readonly struct MemberDeclaration : IEquatable<MemberDeclaration>
    {
        public readonly string Type;
        public readonly string Name;
        public readonly string ValidLowerFirstName;

        public MemberDeclaration(IFieldSymbol field) : this(field.Type.ToDisplayString(), field.Name) { }

        public MemberDeclaration(IPropertySymbol property) : this(property.Type.ToDisplayString(), property.Name) { }

        public MemberDeclaration(string type, string name)
        {
            Type = type;
            Name = name;
            ValidLowerFirstName = ToValidLowerFirst(Name);
        }

        static string ToValidLowerFirst(string value)
        {
            var lowerFirst = char.ToLower(value[0]) + value.Substring(1);
            return SyntaxFacts.GetKeywordKind(lowerFirst) != SyntaxKind.None
                ? $"@{lowerFirst}"
                : lowerFirst;
        }

        public bool Equals(MemberDeclaration other) =>
            Type == other.Type &&
            Name == other.Name;

        public override bool Equals(object? obj) => obj is MemberDeclaration other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return (Type.GetHashCode() * 397) ^ Name.GetHashCode();
            }
        }
    }
}
