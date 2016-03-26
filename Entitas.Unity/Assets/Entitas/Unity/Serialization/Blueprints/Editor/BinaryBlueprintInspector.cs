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
            EntityDrawer.Initialize();
            _pool = new Pool(ComponentIds.TotalComponents, 0, new PoolMetaData("Pool", ComponentIds.componentNames, ComponentIds.componentTypes));
            _entity = _pool.CreateEntity();
            _entity.ApplyBlueprint(((BinaryBlueprint)target).Deserialize());
        }

        public override void OnInspectorGUI() {
            var binaryBlueprint = ((BinaryBlueprint)target);

            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.LabelField("Blueprint", EditorStyles.boldLabel);
                binaryBlueprint.blueprintName = EditorGUILayout.TextField("Name", binaryBlueprint.blueprintName);
                EntityDrawer.DrawComponents(_pool, _entity);
            }
            var changed = EditorGUI.EndChangeCheck();
            if (changed) {
                binaryBlueprint.Serialize(_entity);
                EditorUtility.SetDirty(target);
            }
        }
    }
}