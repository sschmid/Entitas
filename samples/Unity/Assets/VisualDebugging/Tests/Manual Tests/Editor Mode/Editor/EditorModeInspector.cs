using Entitas.VisualDebugging.Unity;
using Entitas.VisualDebugging.Unity.Editor;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EditorModeController))]
public class EditorModeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var controller = (EditorModeController)target;

        if (controller.Context == null)
        {
            controller.Context = new GameContext();
            new ContextObserver(controller.Context);
        }

        if (GUILayout.Button("Reset context"))
        {
            controller.Context.Reset();
            controller.Context.FindContextObserver().gameObject.DestroyGameObject();
            controller.Context = null;
        }

        if (GUILayout.Button("Create entity"))
        {
            var e = (GameEntity)controller.Context.CreateEntity();
            e.AddMyString("Editor Mode");
            Debug.Log($"Entities count: {controller.Context.count}");
        }
    }
}
