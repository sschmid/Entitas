using System;
using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public class IndicesLookupGenerator : CodeGenerator {
        public void GenerateIndicesLookup(Type[] components, string folderPath) {
            var lookups = getLookups(components);
            foreach (var lookup in lookups)
                generateIndicesLookup(lookup.Key, lookup.Value.ToArray(), folderPath);
        }

        Dictionary<string, List<Type>> getLookups(Type[] components) {
            var lookups = new Dictionary<string, List<Type>>();
            foreach (var type in components) {
                var lookupTag = lookupTagForType(type);
                if (!lookups.ContainsKey(lookupTag))
                    lookups.Add(lookupTag, new List<Type>());

                lookups[lookupTag].Add(type);
            }

            return lookups;
        }

        string lookupTagForType(Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs) {
                var era = attr as EntityRepositoryAttribute;
                if (era != null)
                    return era.tag;
            }

            return "ComponentIds";
        }

        void generateIndicesLookup(string tag, Type[] components, string folderPath) {
            var code = string.Empty;
            code = addClassHeader(code, tag);
            code = addIndices(code, components);
            code = addCloseClass(code);
            write(folderPath + tag + ".cs", code);
        }

        string addClassHeader(string str, string className) {
            str += string.Format("public static class {0} {{\n", className);
            return str;
        }

        string addIndices(string str, Type[] components) {
            const string fieldFormat = "    public const int {0} = {1};\n";
            const string totalFormat = "    public const int TotalComponents = {0};";
            var code = string.Empty;
            for (int i = 0; i < components.Length; i++)
                code += string.Format(fieldFormat, components[i].RemoveComponentSuffix(), i);

            code += "\n";
            code += string.Format(totalFormat, components.Length);
            return str + code;        
        }

        string addCloseClass(string code) {
            return code + "\n}";
        }
    }
}