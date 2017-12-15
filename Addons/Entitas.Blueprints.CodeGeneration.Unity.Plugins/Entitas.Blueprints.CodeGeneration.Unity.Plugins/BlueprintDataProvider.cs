using System.Linq;
using DesperateDevs.CodeGeneration;
using Entitas.Blueprints.CodeGeneration.Plugins;
using Entitas.Blueprints.Unity.Editor;

namespace Entitas.Blueprints.CodeGeneration.Unity.Plugins {

    public class BlueprintDataProvider : IDataProvider {

        public string name { get { return "Blueprint"; } }
        public int priority { get { return 0; } }
        public bool runInDryMode { get { return true; } }

        readonly string[] _blueprintNames;

        public BlueprintDataProvider() {
            _blueprintNames = BinaryBlueprintInspector
                .FindAllBlueprints()
                .Select(b => b.Deserialize().name)
                .ToArray();
        }

        public CodeGeneratorData[] GetData() {
            return _blueprintNames
                .Select(blueprintName => {
                    var data = new BlueprintData();
                    data.SetBlueprintName(blueprintName);
                    return data;
                }).ToArray();
        }
    }
}
