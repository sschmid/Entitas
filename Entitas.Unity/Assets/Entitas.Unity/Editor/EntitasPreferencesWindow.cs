using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {

    public interface IEntitasPreferencesDrawer {
        int priority { get; }
        void Initialize(EntitasPreferencesConfig config);
        void Draw(EntitasPreferencesConfig config);
    }

    public class EntitasPreferencesWindow : EditorWindow {

        [MenuItem("Entitas/Preferences... #%e", false, 1)]
        public static void OpenPreferences() {
            EntitasEditorLayout.ShowWindow<EntitasPreferencesWindow>("Entitas Preferences");
        }

        Texture2D _headerTexture;
        string _localVersion;
        EntitasPreferencesConfig _config;
        IEntitasPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        void OnEnable() {
            _headerTexture = EntitasEditorLayout.LoadTexture("l:Entitas-Header");
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
                var offsetY = EntitasEditorLayout.DrawHeaderTexture(this, _headerTexture);
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
    }
}
