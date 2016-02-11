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
            var guid = AssetDatabase.FindAssets(label)[0];
            if (guid != null) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
            return null;
        }

        public static float DrawHeaderTexture(EditorWindow window, Texture2D texture) {
            const int scollBarWidth = 15;
            var ratio = texture.width / texture.height;
            var width = window.position.width - 8 - scollBarWidth;
            var height = width / ratio;
            GUI.DrawTexture(new Rect(4, 2, width, height), texture, ScaleMode.ScaleToFit);

            return height;
        }

        public static Texture2D CreateTexture(int width, int height, Color color) {
            var pixels = new Color[width * height];
            for (int i = 0; i < pixels.Length; ++i) {
                pixels[i] = color;
            }
            var result = new Texture2D(width, height);
            result.SetPixels(pixels);
            result.Apply();
            return result;
        }

        public static void BeginVertical() {
            EditorGUILayout.BeginVertical();
        }

        public static void BeginVerticalBox() {
            EditorGUILayout.BeginVertical(GUI.skin.box);
        }

        public static void BeginVerticalBox(Color color) {
            color.a = 0.15f;
            var style = new GUIStyle(GUI.skin.box);
            style.normal.background = CreateTexture(2, 2, color);
            EditorGUILayout.BeginVertical(style);
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