using System;
using System.Linq;
using System.Collections.Generic;

namespace Entitas.CodeGenerator {
    public static class IndicesLookupGenerator {

        public static Dictionary<string, string> GenerateIndicesLookup(Type[] components) {
            return getLookups(components).ToDictionary(
                lookup => lookup.Key,
                lookup => generateIndicesLookup(lookup.Key, lookup.Value.ToArray())
            );
        }

        static Dictionary<string, List<Type>> getLookups(Type[] components) {
            var lookups = new Dictionary<string, List<Type>>();
            foreach (var type in components.Where(shouldGenerate)) {
                var lookupTag = type.IndicesLookupTag();
                if (!lookups.ContainsKey(lookupTag)) {
                    lookups.Add(lookupTag, new List<Type>());
                }

                lookups[lookupTag].Add(type);
            }

            return lookups;
        }

        static bool shouldGenerate(Type type) {
            Attribute[] attrs = Attribute.GetCustomAttributes(type);
            foreach (Attribute attr in attrs) {
                var dontGenerate = attr as DontGenerateAttribute;
                if (dontGenerate != null && !dontGenerate.generateIndex) {
                    return false;
                }
            }

            return true;
        }

        static string generateIndicesLookup(string tag, Type[] components) {
            return addClassHeader(tag)
            + addIndices(components)
            + idToString(components)
            + addCloseClass()
            + addMatcher(tag);
        }

        static string addClassHeader(string lookupTag) {
            var code = string.Format("using System.Collections.Generic;\n\npublic static class {0} {{\n", lookupTag);
            if (stripDefaultTag(lookupTag) != string.Empty) {
                code = "using Entitas;\n\n" + code;
            }
            return code;
        }

        static string addIndices(Type[] components) {
            const string fieldFormat = "    public const int {0} = {1};\n";
            const string totalFormat = "    public const int TotalComponents = {0};";
            var code = string.Empty;
            for (int i = 0; i < components.Length; i++) {
                code += string.Format(fieldFormat, components[i].RemoveComponentSuffix(), i);
            }

            return code + "\n" + string.Format(totalFormat, components.Length);
        }

        static string idToString(Type[] components) {
            const string format = "        {{ {0}, \"{1}\" }},\n";
            const string formatLast = "        {{ {0}, \"{1}\" }}\n";
            var code = string.Empty;
            for (int i = 0; i < components.Length; i++) {
                if (i < components.Length - 1) {
                    code += string.Format(format, i, components[i].RemoveComponentSuffix());
                } else {
                    code += string.Format(formatLast, i, components[i].RemoveComponentSuffix());
                }
            }

            return string.Format(@"

    static readonly Dictionary<int, string> components = new Dictionary<int, string> {{
{0}    }};

    public static string IdToString(int componentId) {{
        return components[componentId];
    }}", code);
        }

        static string addCloseClass() {
            return "\n}";
        }

        static string addMatcher(string lookupTag) {
            var tag = stripDefaultTag(lookupTag);
            if (tag == string.Empty) {
                return @"

namespace Entitas {
    public partial class Matcher : AllOfMatcher {
        public Matcher(int index) : base(new [] { index }) {
        }

        public override string ToString() {
            return string.Format(""Matcher_"" + ComponentIds.IdToString(indices[0]));
        }
    }
}";
            }

            return string.Format(@"

public partial class {0}Matcher : AllOfMatcher {{
    public {0}Matcher(int index) : base(new [] {{ index }}) {{
    }}

    public override string ToString() {{
        return string.Format(""{0}_"" + {0}ComponentIds.IdToString(indices[0]));
    }}
}}", tag);
        }

        static string stripDefaultTag(string tag) {
            return tag.Replace(typeof(object).IndicesLookupTag(), string.Empty);
        }
    }
}