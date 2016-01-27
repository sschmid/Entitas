using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {
    public class EntitasPreferencesWindow : EditorWindow {

        [MenuItem("Entitas/Preferences...", false, 1)]
        public static void OpenPreferences() {
            EditorWindow.GetWindow<EntitasPreferencesWindow>("Entitas Prefs").Show();
        }

        EntitasPreferencesConfig _config;
        IEntitasPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        void OnEnable() {
            _config = EntitasPreferences.LoadConfig();
            _preferencesDrawers = Assembly.GetAssembly(typeof(IEntitasPreferencesDrawer)).GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(IEntitasPreferencesDrawer)))
                .OrderBy(type => type.FullName)
                .Select(type => (IEntitasPreferencesDrawer)Activator.CreateInstance(type))
                .ToArray();

            foreach (var drawer in _preferencesDrawers) {
                drawer.Initialize(_config);
            }
        }


        void OnGUI() {
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            {
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
