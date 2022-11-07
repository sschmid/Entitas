using System;
using System.Collections.Generic;
using UnityEngine;

namespace Entitas.Blueprints.Unity
{
    [CreateAssetMenu(menuName = "Entitas/Blueprints", fileName = "Blueprints.asset")]
    public class Blueprints : ScriptableObject
    {
        public BinaryBlueprint[] blueprints;

        Dictionary<string, BinaryBlueprint> _binaryBlueprintsMap;

#if (!UNITY_EDITOR)
        Dictionary<string, Blueprint> _blueprintsMap;
#endif

        void OnEnable()
        {
            blueprints ??= Array.Empty<BinaryBlueprint>();
            _binaryBlueprintsMap = new Dictionary<string, BinaryBlueprint>(blueprints.Length);
#if (!UNITY_EDITOR)
            _blueprintsMap = new Dictionary<string, Blueprint>(blueprints.Length);
#endif

            for (var i = 0; i < blueprints.Length; i++)
            {
                var blueprint = blueprints[i];
                if (blueprint != null)
                    _binaryBlueprintsMap.Add(blueprint.name, blueprint);
            }
        }

#if (UNITY_EDITOR)
        public Blueprint GetBlueprint(string name)
        {
            if (!_binaryBlueprintsMap.TryGetValue(name, out var binaryBlueprint))
                throw new BlueprintsNotFoundException(name);

            return binaryBlueprint.Deserialize();
        }

#else
        public Blueprint GetBlueprint(string name)
        {
            if (!_blueprintsMap.TryGetValue(name, out var blueprint))
            {
                if (_binaryBlueprintsMap.TryGetValue(name, out var binaryBlueprint))
                {
                    blueprint = binaryBlueprint.Deserialize();
                    _blueprintsMap.Add(name, blueprint);
                }
                else
                {
                    throw new BlueprintsNotFoundException(name);
                }
            }

            return blueprint;
        }

#endif
    }
}
