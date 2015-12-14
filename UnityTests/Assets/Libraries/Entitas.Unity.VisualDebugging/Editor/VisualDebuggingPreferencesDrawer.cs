using Entitas.Unity;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class VisualDebuggingPreferencesDrawer : IEntitasPreferencesDrawer {
        public void Draw(EntitasPreferencesConfig config) {
            var visualDebuggingConfig = new VisualDebuggingConfig(config);

            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("VisualDebugging", EditorStyles.boldLabel);

                visualDebuggingConfig.defaultInstanceCreatorFolderPath = EditorGUILayout.TextField("DefaultInstanceCreator Folder", visualDebuggingConfig.defaultInstanceCreatorFolderPath);
                visualDebuggingConfig.typeDrawerFolderPath = EditorGUILayout.TextField("TypeDrawer Folder", visualDebuggingConfig.typeDrawerFolderPath);

                EditorGUILayout.HelpBox("Specify the folder where to save generated templates.", MessageType.Info);

            }
            EditorGUILayout.EndVertical();
        }
    }
}
