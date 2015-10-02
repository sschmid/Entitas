using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {
    public interface IEntitasPreferencesDrawer {
        void Draw(EntitasPreferencesConfig config);
    }

    public static class EntitasPreferencesEditor {

        const string CONFIG_PATH = "Entitas.properties";

        static Vector2 _scrollViewPosition;

        [PreferenceItem("Entitas")]
        public static void PreferenceItem() {
            var config = LoadConfig();
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
                SaveConfig(config);
            }
        }

        public static EntitasPreferencesConfig LoadConfig() {
            return new EntitasPreferencesConfig(File.Exists(CONFIG_PATH) ? File.ReadAllText(CONFIG_PATH) : string.Empty);
        }

        public static void SaveConfig(EntitasPreferencesConfig config) {
            File.WriteAllText(CONFIG_PATH, config.ToString());
        }
    }
}