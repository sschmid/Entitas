using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Blueprints.Unity {

    [CreateAssetMenu(menuName = "Entitas/Blueprints", fileName = "Blueprints.asset")]
    public class Blueprints : ScriptableObject {

        public BinaryBlueprint[] blueprints;

        Dictionary<string, BinaryBlueprint> _binaryBlueprintsMap;

#if (!UNITY_EDITOR)
        Dictionary<string, Blueprint> _blueprintsMap;
#endif

        void OnEnable() {
            if (blueprints == null) {
                blueprints = new BinaryBlueprint[0];
            }

            _binaryBlueprintsMap = new Dictionary<string, BinaryBlueprint>(blueprints.Length);
#if (!UNITY_EDITOR)
            _blueprintsMap = new Dictionary<string, Blueprint>(blueprints.Length);
#endif

            for (int i = 0; i < blueprints.Length; i++) {
                var blueprint = blueprints[i];
                if (blueprint != null) {
                    _binaryBlueprintsMap.Add(blueprint.name, blueprint);
                }
            }
        }

#if (UNITY_EDITOR)
        public Blueprint GetBlueprint(string name) {
            BinaryBlueprint binaryBlueprint;
            if (!_binaryBlueprintsMap.TryGetValue(name, out binaryBlueprint)) {
                throw new BlueprintsNotFoundException(name);
            }

            return binaryBlueprint.Deserialize();
        }

        #else

        public Blueprint GetBlueprint(string name) {
            Blueprint blueprint;
            if (!_blueprintsMap.TryGetValue(name, out blueprint)) {
                BinaryBlueprint binaryBlueprint;
                if (_binaryBlueprintsMap.TryGetValue(name, out binaryBlueprint)) {
                    blueprint = binaryBlueprint.Deserialize();
                    _blueprintsMap.Add(name, blueprint);
                } else {
                    throw new BlueprintsNotFoundException(name);
                }
            }

            return blueprint;
        }

#endif
    }
}
