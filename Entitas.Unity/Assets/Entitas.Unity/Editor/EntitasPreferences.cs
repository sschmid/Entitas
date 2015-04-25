using System;
using System.Linq;
using System.Reflection;
using UnityEditor;

namespace Entitas.Unity {
    public interface IEntitasPreferencesDrawer {
        void Draw(string configPath);
    }

    public static class EntitasPreferences {

        const string configPath = "Entitas.properties";

        [PreferenceItem("Entitas")]
        public static void PreferenceItem() {
            var types = Assembly.GetAssembly(typeof(IEntitasPreferencesDrawer)).GetTypes();

            var preferencesDrawers = types
            .Where(type => type.GetInterfaces().Contains(typeof(IEntitasPreferencesDrawer)))
            .OrderBy(type => type.FullName)
            .Select(type => (IEntitasPreferencesDrawer)Activator.CreateInstance(type))
            .ToArray();

            foreach (var drawer in preferencesDrawers) {
                drawer.Draw(configPath);
                EditorGUILayout.Space();
            }
        }
    }
}