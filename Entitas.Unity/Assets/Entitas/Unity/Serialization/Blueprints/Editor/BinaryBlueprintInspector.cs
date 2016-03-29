using System;
using System.Linq;
using Entitas;
using Entitas.Serialization.Blueprints;
using Entitas.Unity.VisualDebugging;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Serialization.Blueprints {

    [CustomEditor(typeof(BinaryBlueprint))]
    public class BinaryBlueprintInspector : Editor {

        Blueprint _blueprint;

        string[] _allPoolNames;
        int _poolIndex;

        Pool _pool;
        Entity _entity;

        void Awake() {
            var binaryBlueprint = ((BinaryBlueprint)target);
            _blueprint = binaryBlueprint.Deserialize();

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), _blueprint.name);

            _allPoolNames = Pools.allPools.Select(pool => pool.metaData.poolName).ToArray();
            if (string.IsNullOrEmpty(_blueprint.poolIdentifier)) {
                _blueprint.poolIdentifier = _allPoolNames[0];
            }
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