using UnityEditor;
using UnityEngine;

namespace Entitas.Blueprints.Unity.Editor {

    [CustomEditor(typeof(Blueprints))]
    public class BlueprintsInspector : UnityEditor.Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if (GUILayout.Button("Find all Blueprints")) {
                var blueprints = ((Blueprints)target);
                blueprints.blueprints = BinaryBlueprintInspector.FindAllBlueprints();
                EditorUtility.SetDirty(blueprints);
            }
        }
    }
}
