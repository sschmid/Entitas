using System.Linq;
using Jenny;
using Entitas.Blueprints.Unity.Editor;

namespace Entitas.Plugins.Blueprints
{
    public class BlueprintDataProvider : IDataProvider
    {
        public string Name => "Blueprint";
        public int Order => 0;
        public bool RunInDryMode => true;

        readonly string[] _blueprintNames;

        public BlueprintDataProvider()
        {
            _blueprintNames = BinaryBlueprintInspector
                .FindAllBlueprints()
                .Select(b => b.Deserialize().name)
                .ToArray();
        }

        public CodeGeneratorData[] GetData() => _blueprintNames
            .Select(blueprintName =>
            {
                var data = new BlueprintData();
                data.SetBlueprintName(blueprintName);
                return data;
            }).ToArray();
    }
}
