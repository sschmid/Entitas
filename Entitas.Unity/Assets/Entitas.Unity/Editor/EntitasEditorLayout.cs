using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {
    public static class EntitasEditorLayout {

        public static void ShowWindow<T>(string title) where T : EditorWindow {
            var window = EditorWindow.GetWindow<T>(true, title);
            window.minSize = window.maxSize = new Vector2(415f, 520f);
            window.Show();
        }

        public static float DrawHeaderTexture(EditorWindow window, Texture2D texture) {
            const int scollBarWidth = 15;
            var ratio = texture.width / texture.height;
            var width = window.position.width - 8 - scollBarWidth;
            var height = width / ratio;
            GUI.DrawTexture(new Rect(4, 2, width, height), texture, ScaleMode.ScaleToFit);

            return height;
        }

        public static void BeginVertical() {
            EditorGUILayout.BeginVertical();
        }

        public static void BeginVerticalBox() {
            EditorGUILayout.BeginVertical(GUI.skin.box);
        }

        public static void EndVertical() {
            EditorGUILayout.EndVertical();
        }

        public static void BeginHorizontal() {
            EditorGUILayout.BeginHorizontal();
        }

        public static void EndHorizontal() {
            EditorGUILayout.EndHorizontal();
        }
    }
}