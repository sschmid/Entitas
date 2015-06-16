using System;
using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {
    public class IndicesLookupGenerator : IComponentCodeGenerator {

        public CodeGenFile[] Generate(Type[] components) {
            var sortedComponents = components.OrderBy(type => type.ToString()).ToArray();
            return getLookups(sortedComponents)
                .Aggregate(new List<CodeGenFile>(), (files, lookup) => {
                    files.Add(new CodeGenFile {
                        fileName = lookup.Key,
                        fileContent = generateIndicesLookup(lookup.Key, lookup.Value.ToArray())
                    });
                    return files;
                }).ToArray();
        }

        static Dictionary<string, List<Type>> getLookups(Type[] components) {
            return components
                .Where(shouldGenerate)
                .Aggregate(new Dictionary<string, List<Type>>(), (lookups, type) => {
                    var lookupTag = type.IndicesLookupTag();
                    if (!lookups.ContainsKey(lookupTag)) {
                        lookups.Add(lookupTag, new List<Type>());
                    }
                        
                    lookups[lookupTag].Add(type);
                    return lookups;
                });
        }

        static bool shouldGenerate(Type type) {
            return Attribute.GetCustomAttributes(type)
                .OfType<DontGenerateAttribute>()
                .All(attr => attr.generateIndex);
        }

        static string generateIndicesLookup(string tag, Type[] components) {
            return addClassHeader(tag)
                    + addIndices(components)
                    + addIdToString(components)
                    + addCloseClass()
                    + addMatcher(tag);
        }

        static string addClassHeader(string lookupTag) {
            var code = string.Format("public static class {0} {{\n", lookupTag);
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

        static string addIdToString(Type[] components) {
            const string format = "        \"{1}\",\n";
            const string formatLast = "        \"{1}\"\n";
            var code = string.Empty;
            for (int i = 0; i < components.Length; i++) {
                if (i < components.Length - 1) {
                    code += string.Format(format, i, components[i].RemoveComponentSuffix());
                } else {
                    code += string.Format(formatLast, i, components[i].RemoveComponentSuffix());
                }
            }

            return string.Format(@"

    static readonly string[] components = {{
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
            return ComponentIds.IdToString(indices[0]);
        }
    }
}";
            }

            return string.Format(@"

public partial class {0}Matcher : AllOfMatcher {{
    public {0}Matcher(int index) : base(new [] {{ index }}) {{
    }}

    public override string ToString() {{
        return {0}ComponentIds.IdToString(indices[0]);
    }}
}}", tag);
        }

        static string stripDefaultTag(string tag) {
            return tag.Replace(typeof(object).IndicesLookupTag(), string.Empty);
        }
    }
}