using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public static class TypeGenerator {

        static readonly Dictionary<string, string> _builtInTypes = new Dictionary<string, string>() {
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
            { "System.String", "string" }
        };

        public static string Generate(Type type) {
            if (_builtInTypes.ContainsKey(type.FullName)) {
                return _builtInTypes[type.FullName];
            }
            if (type.IsGenericType) {
                return generateGenericTypeString(type);
            }
            if (type.IsArray) {
                return generateArrayString(type);
            }
            if (type.IsNested) {
                return type.FullName.Replace('+', '.');
            }

            return type.FullName;
        }

        static string generateGenericTypeString(Type type) {
            var genericMainType = type.FullName.Split('`')[0];
            var genericArguments = type.GetGenericArguments()
                .Select(arg => Generate(arg)).ToArray();

            return genericMainType + "<" + string.Join(", ", genericArguments) + ">";
        }

        static string generateArrayString(Type type) {
            var rankString = string.Empty;
            for (int i = 0; i < type.GetArrayRank() - 1; i++) {
                rankString += ",";
            }

            return Generate(type.GetElementType()) + "[" + rankString + "]";
        }
    }
}