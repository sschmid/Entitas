using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {

    public static class EntitasEditorLayout {

        /*
         * 
         * Editor Window
         * 
         */

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

        static float lastTextureHeight;

        public static void DrawTexture(Texture2D texture) {
            var ratio = (float)texture.width / (float)texture.height;
            var rect = GUILayoutUtility.GetAspectRect(ratio, GUILayout.ExpandWidth(true));
            GUI.DrawTexture(rect, texture, ScaleMode.ScaleAndCrop);
        }

        /*
         * 
         * Section
         * 
         */

        public static bool DrawSectionHeaderToggle(string header, bool value) {
            return GUILayout.Toggle(value, header, EntitasStyles.sectionHeader);
        }

        public static void BeginSectionContent() {
            EditorGUILayout.BeginVertical(EntitasStyles.sectionContent);
        }

        public static void EndSectionContent() {
            EditorGUILayout.EndVertical();
        }

        /*
         * 
         * Layout
         * 
         */

        public static Rect BeginVerticalBox() {
            return EditorGUILayout.BeginVertical(GUI.skin.box);
        }

        public static void EndVerticalBox() {
            EditorGUILayout.EndVertical();
        }

        /*
         * 
         * GUI
         * 
         */

        public static bool ObjectFieldButton(string label, string buttonText) {
            var clicked = false;
            EditorGUILayout.BeginHorizontal();
            {

                EditorGUILayout.LabelField(label, GUILayout.Width(146));

                if(buttonText.Length > 24) {
                    buttonText = "..." + buttonText.Substring(buttonText.Length - 24);
                }

                clicked = (GUILayout.Button(buttonText, EditorStyles.objectField));
            }
            EditorGUILayout.EndHorizontal();

            return clicked;
        }

        public static string ObjectFieldOpenFolderPanel(string label, string buttonText) {
            if(ObjectFieldButton(label, buttonText)) {
                var path = "Assets/";
                path = EditorUtility.OpenFolderPanel(label, path, string.Empty);
                return path.Replace(Directory.GetCurrentDirectory() + "/", string.Empty);
            }

            return null;
        }

        public static bool MiniButton(string c) {
            if(c.Length == 1) {
                return GUILayout.Button(c, EditorStyles.miniButton, GUILayout.Width(19));
            }

            return GUILayout.Button(c, EditorStyles.miniButton);
        }

        public static bool MiniButtonLeft(string c) {
            if(c.Length == 1) {
                return GUILayout.Button(c, EditorStyles.miniButtonLeft, GUILayout.Width(19));
            }

            return GUILayout.Button(c, EditorStyles.miniButtonLeft);
        }

        public static bool MiniButtonRight(string c) {
            if(c.Length == 1) {
                return GUILayout.Button(c, EditorStyles.miniButtonRight, GUILayout.Width(19));
            }

            return GUILayout.Button(c, EditorStyles.miniButtonRight);
        }

        const int DEFAULT_FOLDOUT_MARGIN = 11;

        public static bool Foldout(bool foldout, string content, int leftMargin = DEFAULT_FOLDOUT_MARGIN) {
            return Foldout(foldout, content, EditorStyles.foldout, leftMargin);
        }

        public static bool Foldout(bool foldout, string content, GUIStyle style, int leftMargin = DEFAULT_FOLDOUT_MARGIN) {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(leftMargin);
            foldout = EditorGUILayout.Foldout(foldout, content, style);
            EditorGUILayout.EndHorizontal();
            return foldout;
        }

        public static string SearchTextField(string searchString) {
            GUILayout.BeginHorizontal();
            searchString = GUILayout.TextField(searchString, GUI.skin.FindStyle("ToolbarSeachTextField"));
            if(GUILayout.Button(string.Empty, GUI.skin.FindStyle("ToolbarSeachCancelButton"))) {
                searchString = string.Empty;
            }
            GUILayout.EndHorizontal();

            return searchString;
        }

        public static bool MatchesSearchString(string str, string search) {
            var searches = search.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if(searches.Length == 0) {
                return true;
            }

            for(int i = 0; i < searches.Length; i++) {
                if(str.Contains(searches[i])) {
                    return true;
                }
            }

            return false;
        }
    }
}
