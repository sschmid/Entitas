using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {
    public class EntitasPreferencesWindow : EditorWindow {

        [MenuItem("Entitas/Open Preferences...", false, 0)]
        public static void OpenPreferences() {
            EditorWindow.GetWindow<EntitasPreferencesWindow>().Show();
        }

        static Vector2 _scrollViewPosition;

        void OnGUI() {
            var config = EntitasPreferences.LoadConfig();
            var types = Assembly.GetAssembly(typeof(IEntitasPreferencesDrawer)).GetTypes();
            var preferencesDrawers = types
                .Where(type => type.GetInterfaces().Contains(typeof(IEntitasPreferencesDrawer)))
                .OrderBy(type => type.FullName)
                .Select(type => (IEntitasPreferencesDrawer)Activator.CreateInstance(type))
                .ToArray();

            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            foreach (var drawer in preferencesDrawers) {
                drawer.Draw(config);
                EditorGUILayout.Space();
            }
            EditorGUILayout.EndScrollView();

            if (GUI.changed) {
                EntitasPreferences.SaveConfig(config);
            }
        }
    }
}
