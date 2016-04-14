using System.Collections.Generic;
using System.Linq;
using Entitas.Serialization.Blueprints;
using Entitas.Unity.Serialization.Blueprints;
using UnityEngine;

namespace Entitas.Unity.Serialization.Blueprints {
    
    [CreateAssetMenu(menuName = "Entitas/Blueprints", fileName = "Assets/Blueprints.asset")]
    public partial class Blueprints : ScriptableObject {

        public BinaryBlueprint[] blueprints;

        Dictionary<string, BinaryBlueprint> _binaryBlueprintsMap;
        Dictionary<string, Blueprint> _blueprintsMap;

        void OnEnable() {
            if (blueprints == null) {
                blueprints = FindAllBlueprints();
            }

            _binaryBlueprintsMap = new Dictionary<string, BinaryBlueprint>(blueprints.Length);
            _blueprintsMap = new Dictionary<string, Blueprint>(blueprints.Length);

            for (int i = 0, blueprintsLength = blueprints.Length; i < blueprintsLength; i++) {
                var blueprint = blueprints[i];
                if (blueprint != null) {
                    _binaryBlueprintsMap.Add(blueprint.name, blueprint);
                }
            }
        }

        public BinaryBlueprint[] FindAllBlueprints() {
            return Resources.FindObjectsOfTypeAll<BinaryBlueprint>()
                .OrderBy(blueprint => blueprint.name)
                .ToArray();
        }

        public Blueprint GetBlueprint(string name) {
            Blueprint blueprint;
            if (!_blueprintsMap.TryGetValue(name, out blueprint)) {
                blueprint = _binaryBlueprintsMap[name].Deserialize();
                _blueprintsMap.Add(name, blueprint);
            }

            return blueprint;
        }
    }
}