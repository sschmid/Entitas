using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {
    public static class EntitasEditorLayout {

        public static void ShowWindow<T>(string title) where T : EditorWindow {
            var window = EditorWindow.GetWindow<T>(true, title);
            window.minSize = window.maxSize = new Vector2(415f, 520f);
            window.Show();
        }

        public static Texture2D LoadTexture(string label) {
            var assets = AssetDatabase.FindAssets(label);
            if (assets.Length > 0) {
                var guid = assets[0];
                if (guid != null) {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }

            return null;
        }

        public static float DrawHeaderTexture(EditorWindow window, Texture2D texture) {

            // For unknown reasons OnGUI is called twice and and so is this method.
            // var rect = GUILayoutUtility.GetRect(EditorGUILayout.GetControlRect().width, height);
            // will return wrong width and height (1, 1) every other call
            // workaround: hardcode scrollBarWidth

            const int scollBarWidth = 15;
            var ratio = texture.width / texture.height;
            var width = window.position.width - 8 - scollBarWidth;
            var height = width / ratio;
            GUI.DrawTexture(new Rect(4, 2, width, height), texture, ScaleMode.ScaleToFit);

            return height;
        }

        public static Rect BeginVertical() {
            return EditorGUILayout.BeginVertical();
        }

        public static Rect BeginVerticalBox(GUIStyle style = null) {
            return EditorGUILayout.BeginVertical(style ?? GUI.skin.box);
        }

        public static void EndVertical() {
            EditorGUILayout.EndVertical();
        }

        public static Rect BeginHorizontal() {
            return EditorGUILayout.BeginHorizontal();
        }

        public static void EndHorizontal() {
            EditorGUILayout.EndHorizontal();
        }
    }
}
