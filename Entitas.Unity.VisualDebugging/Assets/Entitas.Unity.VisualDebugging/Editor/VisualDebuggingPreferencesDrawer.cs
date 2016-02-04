using Entitas.Unity;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class VisualDebuggingPreferencesDrawer : IEntitasPreferencesDrawer {

        public int priority { get { return 20; } }

        VisualDebuggingConfig _visualDebuggingConfig;

        public void Initialize(EntitasPreferencesConfig config) {
            _visualDebuggingConfig = new VisualDebuggingConfig(config);
        }

        public void Draw(EntitasPreferencesConfig config) {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("VisualDebugging", EditorStyles.boldLabel);

                _visualDebuggingConfig.defaultInstanceCreatorFolderPath =
                    EditorGUILayout.TextField("DefaultInstanceCreator Folder", _visualDebuggingConfig.defaultInstanceCreatorFolderPath);

                _visualDebuggingConfig.typeDrawerFolderPath =
                    EditorGUILayout.TextField("TypeDrawer Folder", _visualDebuggingConfig.typeDrawerFolderPath);
            }
            EditorGUILayout.EndVertical();
        }
    }
}
