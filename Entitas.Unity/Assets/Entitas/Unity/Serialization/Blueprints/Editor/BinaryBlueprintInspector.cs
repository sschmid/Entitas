using System;
using System.Linq;
using System.Reflection;
using Entitas.Serialization;
using Entitas.Serialization.Blueprints;
using Entitas.Unity.VisualDebugging;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace Entitas.Unity.Serialization.Blueprints {

    [CustomEditor(typeof(BinaryBlueprint))]
    public class BinaryBlueprintInspector : Editor {

        public static BinaryBlueprint[] FindAllBlueprints() {
            return AssetDatabase.FindAssets("l:" + BinaryBlueprintPostprocessor.ASSET_LABEL)
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath<BinaryBlueprint>(path))
                .ToArray();
        }

        [DidReloadScripts, MenuItem("Entitas/Blueprints/Update all Blueprints", false, EntitasMenuItemPriorities.blueprints_update_all_blueprints)]
        public static void UpdateAllBinaryBlueprints() {
            if (!EditorApplication.isPlayingOrWillChangePlaymode) {
                var allPools = findAllPools();
                if (allPools == null) {
                    return;
                }

                var binaryBlueprints = FindAllBlueprints();
                var allPoolNames = allPools.Select(pool => pool.metaData.poolName).ToArray();
                var updated = 0;
                foreach (var binaryBlueprint in binaryBlueprints) {
                    var didUpdate = UpdateBinaryBlueprint(binaryBlueprint, allPools, allPoolNames);
                    if (didUpdate) {
                        updated += 1;
                    }
                }

                if (updated > 0) {
                    Debug.Log("Validated " + binaryBlueprints.Length + " Blueprints, " + updated + " have been updated.");
                }
            }
        }

        public static bool UpdateBinaryBlueprint(BinaryBlueprint binaryBlueprint, Pool[] allPools, string[] allPoolNames) {
            var blueprint = binaryBlueprint.Deserialize();
            var needsUpdate = false;

            var poolIndex = Array.IndexOf(allPoolNames, blueprint.poolIdentifier);
            if (poolIndex < 0) {
                poolIndex = 0;
                needsUpdate = true;
            }

            var pool = allPools[poolIndex];
            blueprint.poolIdentifier = pool.metaData.poolName;

            foreach (var component in blueprint.components) {
                var type = component.fullTypeName.ToType();
                var index = Array.IndexOf(pool.metaData.componentTypes, type);

                if (index != component.index) {
                    Debug.Log(string.Format(
                        "Blueprint '{0}' has invalid or outdated component index for '{1}'. Index was {2} but should be {3}. This will be fixed now!",
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

        static Pool[] findAllPools() {

            // Use reflection because there is no generated Pools.cs when you create a new emtpy project.

            var poolsType = Assembly.GetAssembly(typeof(Entity)).GetTypes().SingleOrDefault(type =>
                type.FullName == "Entitas.Pools" ||
                type.FullName == "Pools" // Obsolete, last gen PoolsGenerator
            );

            if (poolsType != null) {
                var allPools = poolsType.GetProperties(BindingFlags.Public)
                    .Single(info => info.Name == "allPools");

                return (Pool[])allPools.GetValue(poolsType, null);
            }

            return new Pool[0];
        }

        Blueprint _blueprint;

        Pool[] _allPools;
        string[] _allPoolNames;
        int _poolIndex;

        Pool _pool;
        Entity _entity;

        void Awake() {
            _allPools = findAllPools();
            if (_allPools == null) {
                return;
            }

            var binaryBlueprint = ((BinaryBlueprint)target);

            _allPoolNames = _allPools.Select(pool => pool.metaData.poolName).ToArray();

            BinaryBlueprintInspector.UpdateBinaryBlueprint(binaryBlueprint, _allPools, _allPoolNames);

            _blueprint = binaryBlueprint.Deserialize();

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), _blueprint.name);

            _poolIndex = Array.IndexOf(_allPoolNames, _blueprint.poolIdentifier);
            switchToPool();

            _entity.ApplyBlueprint(_blueprint);

            EntityDrawer.Initialize();
        }

        void OnDisable() {
            if (_pool != null) {
                _pool.Reset();
            }
        }

        public override void OnInspectorGUI() {
            var binaryBlueprint = ((BinaryBlueprint)target);

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.LabelField("Blueprint", EditorStyles.boldLabel);
                binaryBlueprint.name = EditorGUILayout.TextField("Name", binaryBlueprint.name);

                EntitasEditorLayout.BeginHorizontal();
                {
                    _poolIndex = EditorGUILayout.Popup(_poolIndex, _allPoolNames);

                    if (GUILayout.Button("Switch")) {
                        switchToPool();
                    }
                }
                EntitasEditorLayout.EndHorizontal();

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
            var targetPool = _allPools[_poolIndex];
            _pool = new Pool(targetPool.totalComponents, 0, targetPool.metaData);
            _entity = _pool.CreateEntity();
        }
    }
}
