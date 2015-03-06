using Entitas;
using UnityEditor;

namespace Entitas.Debug {
    [CustomEditor(typeof(PoolDebugBehaviour))]
    public class PoolDebugEditor : Editor {

        public override void OnInspectorGUI() {
            var debugBehaviour = (PoolDebugBehaviour)target;
            var pool = debugBehaviour.pool;

            EditorGUILayout.LabelField("Entities", pool.Count.ToString());
            EditorGUILayout.LabelField("Reusable entities", pool.pooledEntitiesCount.ToString());
            EditorUtility.SetDirty(target);
        }
    }
}