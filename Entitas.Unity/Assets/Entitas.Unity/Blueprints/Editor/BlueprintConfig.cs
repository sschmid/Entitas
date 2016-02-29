using System;
using Entitas;
using UnityEditor;
using UnityEngine;
using Entitas.Unity.VisualDebugging;

namespace Entitas.Unity.Blueprints {
    
    [Serializable]
    public class BlueprintConfig : ScriptableObject {

        [MenuItem("Assets/Create/Blueprint")]
        public static void CreateBlueprintsAsset() {
            var asset = ScriptableObject.CreateInstance<BlueprintConfig>();
            var assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/New Blueprint.asset");
            AssetDatabase.CreateAsset(asset, assetPathAndName);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = asset;
        }
    }

    [CustomEditor(typeof(BlueprintConfig))]
    public class BlueprintConfigInspector : Editor {

        Pool _pool;
        Entity _entity;

        void Awake() {
            EntityDrawer.Initialize();
            _pool = new Pool(ComponentIds.TotalComponents, 0, new PoolMetaData("Pool", ComponentIds.componentNames, ComponentIds.componentTypes));
            _entity = _pool.CreateEntity();
            _entity.AddMyString("Test");
        }

        public override void OnInspectorGUI() {
            try {
                EntityDrawer.DrawEntity(_pool, _entity);
            } catch (Exception) {
            }
        }
    }
}