using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Entitas.Generators
{
    public readonly struct MemberDeclaration
    {
        public readonly string Type;
        public readonly string Name;
        public readonly int EntityIndexType;
        public readonly string ValidLowerFirstName;

        public MemberDeclaration(IFieldSymbol field) : this(field, field.Type) { }

        public MemberDeclaration(IPropertySymbol property) : this(property, property.Type) { }

        public MemberDeclaration(ISymbol symbol, ITypeSymbol type) : this(GetTypeName(type), symbol.Name, GetEntityIndexType(symbol)) { }

        public MemberDeclaration(string type, string name, int entityIndexType)
        {
            Type = type;
            Name = name;
            EntityIndexType = entityIndexType;
            ValidLowerFirstName = ToValidLowerFirst(Name);
        }

        static string GetTypeName(ITypeSymbol type)
        {
            return type is IArrayTypeSymbol arrayType
                ? arrayType.ToDisplayString().Replace("*", string.Empty)
                : type.ToDisplayString();
        }

        static int GetEntityIndexType(ISymbol symbol)
        {
            var attribute = symbol.GetAttributes().FirstOrDefault(static attribute => attribute.AttributeClass?.ToDisplayString() == "Entitas.Generators.Attributes.EntityIndexAttribute");
            if (attribute is null)
                return -1;

            var arg = attribute.ConstructorArguments.FirstOrDefault();
            return arg.Type?.ToDisplayString() == "bool" && arg.Value is bool isPrimary ? isPrimary ? 1 : 0 : -1;
        }

        static string ToValidLowerFirst(string value)
        {
            var lowerFirst = char.ToLower(value[0]) + value.Substring(1);
            return SyntaxFacts.GetKeywordKind(lowerFirst) != SyntaxKind.None
                ? $"@{lowerFirst}"
                : lowerFirst;
        }
    }

    public class TypeAndNameComparer : IEqualityComparer<MemberDeclaration>
    {
        public static readonly TypeAndNameComparer Instance = new TypeAndNameComparer();

        public bool Equals(MemberDeclaration x, MemberDeclaration y) =>
            x.Type == y.Type &&
            x.Name == y.Name;

        public int GetHashCode(MemberDeclaration obj)
        {
            unchecked
            {
                return (obj.Type.GetHashCode() * 397) ^ obj.Name.GetHashCode();
            }
        }
    }

    public class TypeAndNameAndEntityIndexTypeComparer : IEqualityComparer<MemberDeclaration>
    {
        public static readonly TypeAndNameAndEntityIndexTypeComparer Instance = new TypeAndNameAndEntityIndexTypeComparer();

        public bool Equals(MemberDeclaration x, MemberDeclaration y) =>
            x.Type == y.Type &&
            x.Name == y.Name &&
            x.EntityIndexType == y.EntityIndexType;

        public int GetHashCode(MemberDeclaration obj)
        {
            unchecked
            {
                var hashCode = obj.Type.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Name.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.EntityIndexType;
                return hashCode;
            }
        }
    }
}
