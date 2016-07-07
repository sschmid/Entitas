using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Serialization {

    public static class TypeSerializationExtension {

        /// Generates a simplified type string for the specified type that can be compiled.
        /// This is useful for code generation that will produce compilable source code.
        /// e.g. int instead of System.Int32
        /// e.g. System.Collections.Generic.Dictionary<int, string> instead of System.Collections.Generic.Dictionary`2[System.Int32,System.String]
        public static string ToCompilableString(this Type type) {
            if (_builtInTypesToString.ContainsKey(type.FullName)) {
                return _builtInTypesToString[type.FullName];
            }
            if (type.IsGenericType) {
                var genericMainType = type.FullName.Split('`')[0];
                var genericArguments = type.GetGenericArguments().Select(argType => argType.ToCompilableString()).ToArray();
                return genericMainType + "<" + string.Join(", ", genericArguments) + ">";
            }
            if (type.IsArray) {
                return type.GetElementType().ToCompilableString() + "[" + new string(',', type.GetArrayRank() - 1) + "]";
            }
            if (type.IsNested) {
                return type.FullName.Replace('+', '.');
            }

            return type.FullName;
        }

        /// Generates a simplified type string for the specified type that is easy to read and can be parsed and converted into System.Type.
        /// This is useful for code generation that serializes objects which is also used to create runtime objects based on the type string.
        /// e.g. int instead of System.Int32
        /// e.g. System.Collections.Generic.Dictionary<int, string> instead of System.Collections.Generic.Dictionary`2[System.Int32,System.String]
        public static string ToReadableString(this Type type) {
            if (_builtInTypesToString.ContainsKey(type.FullName)) {
                return _builtInTypesToString[type.FullName];
            }
            if (type.IsGenericType) {
                var genericMainType = type.FullName.Split('`')[0];
                var genericArguments = type.GetGenericArguments().Select(argType => argType.ToReadableString()).ToArray();
                return genericMainType + "<" + string.Join(", ", genericArguments) + ">";
            }
            if (type.IsArray) {
                return type.GetElementType().ToReadableString() + "[" + new string(',', type.GetArrayRank() - 1) + "]";
            }

            return type.FullName;
        }

        /// Tries to find and create a type based on the specified type string.
        public static Type ToType(this string typeString) {
            var fullTypeName = generateTypeString(typeString);
            var type = Type.GetType(fullTypeName);
            if (type != null) {
                return type;
            }

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                type = assembly.GetType(fullTypeName);
                if (type != null) {
                    return type;
                }
            }

            return null;
        }

        static string generateTypeString(string typeString) {
            if (_builtInTypeStrings.ContainsKey(typeString)) {
                typeString = _builtInTypeStrings[typeString];
            } else {
                typeString = generateGenericArguments(typeString);
                typeString = generateArray(typeString);
            }

            return typeString;
        }

        static string generateGenericArguments(string typeString) {
            const string genericArgsPattern = @"<(?<arg>.*)>";
            var separator = new [] { ", " };
            typeString = Regex.Replace(typeString, genericArgsPattern,
                m => {
                    var ts = generateTypeString(m.Groups["arg"].Value);
                    var argsCount = ts.Split(separator, StringSplitOptions.None).Length;

                    return "`" + argsCount + "[" + ts + "]";
                });

            return typeString;
        }

        static string generateArray(string typeString) {
            const string arrayPattern = @"(?<type>[^\[]*)(?<rank>\[,*\])";
            typeString = Regex.Replace(typeString, arrayPattern,
                m => {
                    var type = generateTypeString(m.Groups["type"].Value);
                    var rank = m.Groups["rank"].Value;
                    return type + rank;
                });

            return typeString;
        }

        static readonly Dictionary<string, string> _builtInTypesToString = new Dictionary<string, string>() {
            { "System.Boolean", "bool" },
            { "System.Byte", "byte" },
            { "System.SByte", "sbyte" },
            { "System.Char", "char" },
            { "System.Decimal", "decimal" },
            { "System.Double", "double" },
            { "System.Single", "float" },
            { "System.Int32", "int" },
            { "System.UInt32", "uint" },
            { "System.Int64", "long" },
            { "System.UInt64", "ulong" },
            { "System.Object", "object" },
            { "System.Int16", "short" },
            { "System.UInt16", "ushort" },
            { "System.String", "string" },
            { "System.Void", "void" }
        };

        static readonly Dictionary<string, string> _builtInTypeStrings = new Dictionary<string, string>() {
            { "bool", "System.Boolean" },
            { "byte", "System.Byte" },
            { "sbyte", "System.SByte" },
            { "char", "System.Char" },
            { "decimal", "System.Decimal" },
            { "double", "System.Double" },
            { "float", "System.Single" },
            { "int", "System.Int32" },
            { "uint", "System.UInt32" },
            { "long", "System.Int64" },
            { "ulong", "System.UInt64" },
            { "object", "System.Object" },
            { "short", "System.Int16" },
            { "ushort", "System.UInt16" },
            { "string", "System.String" },
            { "void", "System.Void" }
        };
    }
}