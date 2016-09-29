using UnityEditor;
using UnityEngine;
using Entitas;

[CustomEditor(typeof(EditorModeController))]
public class EditorModeInspector : Editor {

    public override void OnInspectorGUI() {

        var controller = (EditorModeController)target;

        if(controller.pool == null) {
            controller.pool = Pools.CreateVisualDebuggingPool();
            new Entitas.Unity.VisualDebugging.PoolObserver(controller.pool);
        }

        if(GUILayout.Button("Reset pool")) {
            controller.pool.Reset();
        }

        if(GUILayout.Button("Create entitiy")) {
            var e = controller.pool.CreateEntity();
            e.AddMyString("Editor Mode");

            Debug.Log("Entities count: " + controller.pool.count);
        }
    }
}
