using System;
using Microsoft.CodeAnalysis;

namespace Entitas.Generators
{
    public static class GeneratorUtils
    {
        public static readonly SymbolDisplayFormat NameOnlyFormat = new SymbolDisplayFormat(
            SymbolDisplayGlobalNamespaceStyle.Omitted,
            SymbolDisplayTypeQualificationStyle.NameOnly
        );

        public static string AddSuffix(this string str, string suffix) =>
            str.EndsWith(suffix, StringComparison.Ordinal) ? str : str + suffix;

        public static string RemoveSuffix(this string str, string suffix) =>
            str.EndsWith(suffix, StringComparison.Ordinal) ? str[..^suffix.Length] : str;
    }
}
