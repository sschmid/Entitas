using Entitas.VisualDebugging.Unity;
using Entitas.VisualDebugging.Unity.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorModeController))]
public class EditorModeInspector : Editor {

    public override void OnInspectorGUI() {

        var controller = (EditorModeController)target;

        if (controller.context == null) {
            controller.context = new GameContext();
            new Entitas.VisualDebugging.Unity.ContextObserver(controller.context);
        }

        if (GUILayout.Button("Reset context")) {
            controller.context.Reset();
            controller.context.FindContextObserver().gameObject.DestroyGameObject();
            controller.context = null;
        }

        if (GUILayout.Button("Create entitiy")) {
            var e = (GameEntity)controller.context.CreateEntity();
            e.AddMyString("Editor Mode");

            Debug.Log("Entities count: " + controller.context.count);
        }
    }
}
