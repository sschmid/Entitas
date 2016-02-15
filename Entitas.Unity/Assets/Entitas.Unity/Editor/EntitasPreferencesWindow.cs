using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {
    public class EntitasPreferencesWindow : EditorWindow {

        [MenuItem("Entitas/Preferences...", false, 1)]
        public static void OpenPreferences() {
            var window = EditorWindow.GetWindow<EntitasPreferencesWindow>(true, "Entitas Preferences");
            window.minSize = window.maxSize = new Vector2(415f, 520f);
            window.Show();
        }

        Texture2D _headerTexture;
        string _localVersion;
        EntitasPreferencesConfig _config;
        IEntitasPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;


        void OnEnable() {
            var guid = AssetDatabase.FindAssets("l:Entitas-Header")[0];
            if (guid != null) {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                _headerTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }

            _localVersion = EntitasCheckForUpdates.GetLocalVersion();

            _config = EntitasPreferences.LoadConfig();
            _preferencesDrawers = Assembly.GetAssembly(typeof(IEntitasPreferencesDrawer)).GetTypes()
                .Where(type => type.ImplementsInterface<IEntitasPreferencesDrawer>())
                .Select(type => (IEntitasPreferencesDrawer)Activator.CreateInstance(type))
                .OrderBy(drawer => drawer.priority)
                .ToArray();

            foreach (var drawer in _preferencesDrawers) {
                drawer.Initialize(_config);
            }
        }

        void OnGUI() {
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            {
                var offsetY = drawHeaderTexture();
                EditorGUILayout.Space();
                EditorGUILayout.LabelField("Version: " + _localVersion);
                GUILayout.Space(offsetY - 24);

                foreach (var drawer in _preferencesDrawers) {
                    drawer.Draw(_config);
                    EditorGUILayout.Space();
                }
            }
            EditorGUILayout.EndScrollView();

            if (GUI.changed) {
                EntitasPreferences.SaveConfig(_config);
            }
        }

        float drawHeaderTexture() {
            const int scollBarWidth = 15;
            var ratio = _headerTexture.width / _headerTexture.height;
            var width = position.width - 8 - scollBarWidth;
            var height = width / ratio;
            GUI.DrawTexture(new Rect(4, 2, width, height), _headerTexture, ScaleMode.ScaleToFit);
            
            return height;
        }
    }
}
