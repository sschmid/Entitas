using System;
using Entitas;
using Entitas.Unity.VisualDebugging;
using UnityEditor;

namespace Entitas.Unity.Serialization.Blueprints {

    [CustomEditor(typeof(BinaryBlueprint))]
    public class BinaryBlueprintInspector : Editor {

        Pool _pool;
        Entity _entity;

        void Awake() {
            var binaryBlueprint = ((BinaryBlueprint)target);
            var blueprint = binaryBlueprint.Deserialize();

            AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(target), blueprint.name);

            EntityDrawer.Initialize();
            _pool = new Pool(VisualDebuggingComponentIds.TotalComponents, 0, new PoolMetaData("Pool", VisualDebuggingComponentIds.componentNames, VisualDebuggingComponentIds.componentTypes));
            _entity = _pool.CreateEntity();
            _entity.ApplyBlueprint(blueprint);
        }

        public override void OnInspectorGUI() {
            var binaryBlueprint = ((BinaryBlueprint)target);

            EditorGUI.BeginChangeCheck();
            {
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
    }
}