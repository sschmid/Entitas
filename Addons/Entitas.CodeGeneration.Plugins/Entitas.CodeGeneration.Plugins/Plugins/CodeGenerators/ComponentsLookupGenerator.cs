using System.Collections.Generic;
using System.Linq;
using System.IO;

namespace Entitas.CodeGeneration.Plugins {

    public class ComponentsLookupGenerator : ICodeGenerator, IConfigurable {

        public string name { get { return "Components Lookup"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        const string IGNORE_NAMESPACES_KEY = "Entitas.CodeGeneration.Plugins.IgnoreNamespaces";

        public Dictionary<string, string> defaultProperties {
            get { return new Dictionary<string, string> { { IGNORE_NAMESPACES_KEY, "false" } }; }
        }

        bool ignoreNamespaces { get { return properties[IGNORE_NAMESPACES_KEY] == "true"; } }

        Dictionary<string, string> properties {
            get {
                if(_properties == null) {
                    _properties = defaultProperties;
                }

                return _properties;
            }
        }

        Dictionary<string, string> _properties;

        public const string COMPONENTS_LOOKUP = "ComponentsLookup";

        const string COMPONENTS_LOOKUP_TEMPLATE =
@"public static class ${Lookup} {

${componentConstants}

${totalComponentsConstant}

    public static readonly string[] componentNames = {
${componentNames}
    };

    public static readonly System.Type[] componentTypes = {
${componentTypes}
    };
}
";

        const string COMPONENT_CONSTANTS_TEMPLATE =
@"    public const int ${ComponentName} = ${Index};";

        const string TOTAL_COMPONENTS_CONSTANT_TEMPLATE =
@"    public const int TotalComponents = ${totalComponents};";

        const string COMPONENT_NAMES_TEMPLATE =
@"        ""${ComponentName}""";

        const string COMPONENT_TYPES_TEMPLATE =
@"        typeof(${ComponentType})";

        public void Configure(Dictionary<string, string> properties) {
            _properties = properties;
        }

        public CodeGenFile[] Generate(CodeGeneratorData[] data) {
            var files = new List<CodeGenFile>();

            var contextData = data
                .OfType<ContextData>()
                .ToArray();

            files.AddRange(generateEmptyLookup(contextData));

            var componentData = data
                .OfType<ComponentData>()
                .Where(d => d.ShouldGenerateIndex())
                .ToArray();

            var lookup = generateLookup(componentData);
            var fileNames = new HashSet<string>(lookup.Select(file => file.fileName));

            files = files
                .Where(file => !fileNames.Contains(file.fileName))
                .ToList();

            files.AddRange(lookup);

            return files.ToArray();
        }

        CodeGenFile[] generateEmptyLookup(ContextData[] data) {
            var emptyData = new ComponentData[0];
            return data
                .Select(d => generateComponentsLookupClass(d.GetContextName(), emptyData))
                .ToArray();
        }

        CodeGenFile[] generateLookup(ComponentData[] data) {
            var contextNameToComponentData = data
                .Aggregate(new Dictionary<string, List<ComponentData>>(), (dict, d) => {
                    var contextNames = d.GetContextNames();
                    foreach(var contextName in contextNames) {
                        if(!dict.ContainsKey(contextName)) {
                            dict.Add(contextName, new List<ComponentData>());
                        }

                        dict[contextName].Add(d);
                    }

                    return dict;
                });

            foreach(var key in contextNameToComponentData.Keys.ToArray()) {
                contextNameToComponentData[key] = contextNameToComponentData[key]
                    .OrderBy(d => d.GetFullTypeName())
                    .ToList();
            }

            return contextNameToComponentData
                .Select(kv => generateComponentsLookupClass(kv.Key, kv.Value.ToArray()))
                .ToArray();
        }

        CodeGenFile generateComponentsLookupClass(string contextName, ComponentData[] data) {
            var componentConstants = string.Join("\n", data
                .Select((d, index) => {
                    return COMPONENT_CONSTANTS_TEMPLATE
                    .Replace("${ComponentName}", d.GetFullTypeName().ToComponentName(ignoreNamespaces))
                        .Replace("${Index}", index.ToString());
                }).ToArray());

            var totalComponentsConstant = TOTAL_COMPONENTS_CONSTANT_TEMPLATE
                .Replace("${totalComponents}", data.Length.ToString());

            var componentNames = string.Join(",\n", data
                .Select(d => COMPONENT_NAMES_TEMPLATE
                        .Replace("${ComponentName}", d.GetFullTypeName().ToComponentName(ignoreNamespaces))
                ).ToArray());

            var componentTypes = string.Join(",\n", data
                .Select(d => COMPONENT_TYPES_TEMPLATE
                    .Replace("${ComponentType}", d.GetFullTypeName())
                ).ToArray());

            var fileContent = COMPONENTS_LOOKUP_TEMPLATE
                .Replace("${Lookup}", contextName + COMPONENTS_LOOKUP)
                .Replace("${componentConstants}", componentConstants)
                .Replace("${totalComponentsConstant}", totalComponentsConstant)
                .Replace("${componentNames}", componentNames)
                .Replace("${componentTypes}", componentTypes);

            return new CodeGenFile(
                contextName + Path.DirectorySeparatorChar + contextName + COMPONENTS_LOOKUP + ".cs",
                fileContent,
                GetType().FullName
            );
        }
    }
}
