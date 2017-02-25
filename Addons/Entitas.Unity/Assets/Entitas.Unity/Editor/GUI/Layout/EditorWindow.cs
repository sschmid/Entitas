using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {

    public static partial class EntitasEditorLayout {

        public static void ShowWindow<T>(string title) where T : EditorWindow {
            var window = EditorWindow.GetWindow<T>(true, title);
            window.minSize = window.maxSize = new Vector2(415f, 520f);
            window.Show();
        }

        public static Texture2D LoadTexture(string label) {
            var assets = AssetDatabase.FindAssets(label);
            if(assets.Length > 0) {
                var guid = assets[0];
                if(guid != null) {
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    return AssetDatabase.LoadAssetAtPath<Texture2D>(path);
                }
            }

            return null;
        }

        public static void DrawTexture(Texture2D texture) {
            var ratio = (float)texture.width / (float)texture.height;
            var rect = GUILayoutUtility.GetAspectRect(ratio, GUILayout.ExpandWidth(true));
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleAndCrop);
        }
    }
}