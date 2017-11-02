using System.Collections.Generic;
using System.Linq;
using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class KitDataProvider : ICodeGeneratorDataProvider, IConfigurable, ICodeGeneratorCachable {

        public string name { get { return "Entitas Kits"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return true; } }

        public Dictionary<string, string> defaultProperties {
            get { return _contextNamesConfig.defaultProperties; }
        }

        public Dictionary<string, object> objectCache { get; set; }

        readonly CodeGeneratorConfig _codeGeneratorConfig = new CodeGeneratorConfig();
        readonly ContextNamesConfig _contextNamesConfig = new ContextNamesConfig();

        public void Configure(Preferences preferences) {
            _codeGeneratorConfig.Configure(preferences);
            _contextNamesConfig.Configure(preferences);
        }

        public CodeGeneratorData[] GetData() {
            var kitToContextNames = _contextNamesConfig.contextsWithKits
                .Where(kv => kv.Value.Length != 0)
                .Aggregate(new Dictionary<string, List<string>>(), (acc, kv) => {
                    foreach (var kit in kv.Value) {
                        List<string> contextNames;
                        if (!acc.TryGetValue(kit, out contextNames)) {
                            contextNames = new List<string>();
                            acc.Add(kit, contextNames);
                        }
                        contextNames.Add(kv.Key);
                    }

                    return acc;
            });

            return createDataFromKits(kitToContextNames);
        }

        ComponentData[] createDataFromKits(Dictionary<string, List<string>> kitToContextNames) {
            return kitToContextNames.Aggregate(new List<ComponentData>(), (acc, kv) => {
                var kitTypes = PluginUtil
                    .GetCachedAssemblyResolver(objectCache, new[] { kv.Key }, _codeGeneratorConfig.searchPaths)
                    .GetTypes();

                var data = (ComponentData[])new ComponentDataProvider(kitTypes).GetData();
                foreach (var d in data) {
                    d.SetContextNames(kv.Value.ToArray());
                }

                acc.AddRange(data);
                return acc;
            }).ToArray();
        }
    }
}
