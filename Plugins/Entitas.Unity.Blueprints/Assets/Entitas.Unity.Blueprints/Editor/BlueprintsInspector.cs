using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Blueprints {

    [CustomEditor(typeof(Blueprints))]
    public class BlueprintsInspector : Editor {

        public override void OnInspectorGUI() {
            DrawDefaultInspector();

            if(GUILayout.Button("Find all Blueprints")) {
                var blueprints = ((Blueprints)target);
                blueprints.blueprints = BinaryBlueprintInspector.FindAllBlueprints();
                EditorUtility.SetDirty(blueprints);
            }
        }
    }
}
