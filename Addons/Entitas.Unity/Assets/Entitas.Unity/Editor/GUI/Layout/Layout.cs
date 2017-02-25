using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {

    public static partial class EntitasEditorLayout {

        public static bool DrawSectionHeaderToggle(string header, bool value) {
            return GUILayout.Toggle(value, header, EntitasStyles.sectionHeader);
        }

        public static void BeginSectionContent() {
            EditorGUILayout.BeginVertical(EntitasStyles.sectionContent);
        }

        public static void EndSectionContent() {
            EditorGUILayout.EndVertical();
        }

        public static Rect BeginVerticalBox() {
            return EditorGUILayout.BeginVertical(GUI.skin.box);
        }

        public static void EndVerticalBox() {
            EditorGUILayout.EndVertical();
        }
    }
}
