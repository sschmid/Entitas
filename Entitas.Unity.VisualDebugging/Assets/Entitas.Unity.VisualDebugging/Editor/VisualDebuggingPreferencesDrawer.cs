using Entitas.Unity;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class VisualDebuggingPreferencesDrawer : IEntitasPreferencesDrawer {
        public void Draw(EntitasPreferencesConfig config) {
            var visualDebuggingConfig = new VisualDebuggingConfig(config);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("VisualDebugging", EditorStyles.boldLabel);

            // Generated Folder
            visualDebuggingConfig.defaultInstanceCreatorFolderPath = EditorGUILayout.TextField("DefaultInstanceCreator Folder", visualDebuggingConfig.defaultInstanceCreatorFolderPath);
            visualDebuggingConfig.typeDrawerFolderPath = EditorGUILayout.TextField("TypeDrawer Folder", visualDebuggingConfig.typeDrawerFolderPath);

            EditorGUILayout.EndVertical();
        }
    }
}
