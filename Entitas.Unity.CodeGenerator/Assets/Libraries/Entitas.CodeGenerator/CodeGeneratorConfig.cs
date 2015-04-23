using System;
using System.Linq;

namespace Entitas.CodeGenerator {
    public class CodeGeneratorConfig {
        public string generatedFolderPath { 
            get { return getValueOrDefault(generatedFolderPathKey, defaultgeneratedFolderPath); }
            set { _properties[generatedFolderPathKey] = value; }
        }

        public string[] pools {
            get { 
                return getValueOrDefault(poolsKey, string.Empty)
                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(poolName => poolName.Trim())
                    .ToArray();
            }
            set { _properties[poolsKey] = string.Join(",", value.Where(pool => !string.IsNullOrEmpty(pool)).ToArray()).Replace(" ", string.Empty); }
        }

        const string generatedFolderPathKey = "Entitas.CodeGenerator.GeneratedFolderPath";
        const string poolsKey               = "Entitas.CodeGenerator.Pools";

        const string defaultgeneratedFolderPath = "Assets/Generated/";

        readonly Properties _properties;

        public CodeGeneratorConfig(string config) {
            _properties = new Properties(config);
            generatedFolderPath = generatedFolderPath;
            pools = pools;
        }

        public override string ToString() {
            return _properties.ToString();
        }

        string getValueOrDefault(string key, string defaultValue) {
            if (_properties.ContainsKey(key)) {
                return _properties[key];
            }

            _properties[key] = defaultValue;
            return defaultValue;
        }
    }
}

