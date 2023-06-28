using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Entitas.Generators
{
    readonly struct MemberDeclaration
    {
        public readonly string Type;
        public readonly string Name;

        public readonly string ValidLowerFirstName;

        public MemberDeclaration(IFieldSymbol field) : this(field.Type.ToDisplayString(), field) { }

        public MemberDeclaration(IPropertySymbol property) : this(property.Type.ToDisplayString(), property) { }

        public MemberDeclaration(string type, ISymbol symbol)
        {
            Type = type;
            Name = symbol.Name;
            ValidLowerFirstName = ToValidLowerFirst(Name);
        }

        static string ToValidLowerFirst(string value)
        {
            var lowerFirst = char.ToLower(value[0]) + value.Substring(1);
            return SyntaxFacts.GetKeywordKind(lowerFirst) != SyntaxKind.None
                ? $"@{lowerFirst}"
                : lowerFirst;
        }
    }
}
