using System;
using System.Linq;
using Entitas;
using Entitas.Serialization;
using Entitas.Serialization.Blueprints;
using Entitas.Unity.VisualDebugging;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Serialization.Blueprints {

    [CustomEditor(typeof(BinaryBlueprint))]
    public class BinaryBlueprintInspector : Editor {

        [MenuItem("Entitas/Blueprints/Update all Blueprints", false, 300)]
        public static void UpdateAllBinaryBlueprints() {
            var binaryBlueprints = Resources.FindObjectsOfTypeAll<BinaryBlueprint>();
            var allPoolNames = Pools.allPools.Select(pool => pool.metaData.poolName).ToArray();
            var updated = 0;
            foreach (var binaryBlueprint in binaryBlueprints) {
                var didUpdate = UpdateBinaryBlueprint(binaryBlueprint, allPoolNames);
                if (didUpdate) {
                    updated += 1;
                }
            }

            Debug.Log("Validated " + binaryBlueprints.Length + " Blueprints. " + updated + " have been updated.");
        }

        public static bool UpdateBinaryBlueprint(BinaryBlueprint binaryBlueprint, string[] allPoolNames) {
            var blueprint = binaryBlueprint.Deserialize();
            var needsUpdate = false;

            var poolIndex = Array.IndexOf(allPoolNames, blueprint.poolIdentifier);
            if (poolIndex < 0) {
                poolIndex = 0;
                needsUpdate = true;
            }

            var pool = Pools.allPools[poolIndex];
            blueprint.poolIdentifier = pool.metaData.poolName;

            foreach (var component in blueprint.components) {
                var type = component.fullTypeName.ToType();
                var index = Array.IndexOf(pool.metaData.componentTypes, type);

                if (index != component.index) {
                    Debug.Log(string.Format(
                        "Blueprint '{0}' has invalid or outdated component index for '{1}'. Index was {2} but should be {3}. This has been fixed now! Happy coding!",
                        blueprint.name, component.fullTypeName, component.index, index));

                    component.index = index;
                    needsUpdate = true;
                }
            }

            if (needsUpdate) {
                Debug.Log("Updating Blueprint '" + blueprint.name + "'");
                binaryBlueprint.Serialize(blueprint);
            }

            return needsUpdate;
        }

        Blueprint _blueprint;

        string[] _allPoolNames;
        int _poolIndex;

        Pool _pool;
        Entity _entity;

        void Awake() {
            var binaryBlueprint = ((BinaryBlueprint)target);

            _allPoolNames = Pools.allPools.Select(pool => pool.metaData.poolName).ToArray();

            BinaryBlueprintInspector.UpdateBinaryBlueprint(binaryBlueprint, _allPoolNames);

            _blueprint = binaryBlueprint.Deserialize();

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), _blueprint.name);

            _poolIndex = Array.IndexOf(_allPoolNames, _blueprint.poolIdentifier);
            switchToPool();

            _entity.ApplyBlueprint(_blueprint);

            EntityDrawer.Initialize();
        }

        public override void OnInspectorGUI() {
            var binaryBlueprint = ((BinaryBlueprint)target);

            EditorGUI.BeginChangeCheck();
            {
                EntitasEditorLayout.BeginHorizontal();
                {
                    _poolIndex = EditorGUILayout.Popup(_poolIndex, _allPoolNames);

                    if (GUILayout.Button("Switch")) {
                        switchToPool();
                    }
                }
                EntitasEditorLayout.EndHorizontal();

                EditorGUILayout.LabelField("Blueprint", EditorStyles.boldLabel);
                binaryBlueprint.name = EditorGUILayout.TextField("Name", binaryBlueprint.name);
                EntityDrawer.DrawComponents(_pool, _entity);
            }
            var changed = EditorGUI.EndChangeCheck();
            if (changed) {
                binaryBlueprint.Serialize(_entity);
                AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), binaryBlueprint.name);
                EditorUtility.SetDirty(target);
            }
        }

        void switchToPool() {
            if (_pool != null) {
                _pool.Reset();
            }
            _pool = Pools.allPools[_poolIndex];
            _entity = _pool.CreateEntity();
        }
    }
}