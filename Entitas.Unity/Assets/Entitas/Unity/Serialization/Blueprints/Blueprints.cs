﻿using System.Collections.Generic;
using Entitas.Serialization.Blueprints;
using UnityEngine;

namespace Entitas.Unity.Serialization.Blueprints {

    [CreateAssetMenu(menuName = "Entitas/Blueprints", fileName = "Assets/Blueprints.asset")]
    public partial class Blueprints : ScriptableObject {

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

            for (int i = 0, blueprintsLength = blueprints.Length; i < blueprintsLength; i++) {
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

    public class BlueprintsNotFoundException : EntitasException {
        public BlueprintsNotFoundException(string blueprintName) :
            base("'" + blueprintName + "' does not exist!", "Did you update the Blueprints ScriptableObject?") {
        }
    }
}