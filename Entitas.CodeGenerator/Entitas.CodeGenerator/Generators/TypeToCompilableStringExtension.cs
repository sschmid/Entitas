using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public static class TypeToCompilableStringExtension {

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
            { "System.String", "string" },
            { "System.Void", "void" }
        };

        public static string ToCompilableString(this Type type) {
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
                .Select(argType => argType.ToCompilableString()).ToArray();

            return genericMainType + "<" + string.Join(", ", genericArguments) + ">";
        }

        static string generateArrayString(Type type) {
            return type.GetElementType().ToCompilableString() + "[" + new string(',', type.GetArrayRank() - 1) + "]";
        }
    }
}