using System;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public class EntitasPreferencesWindow : PreferencesWindow
    {
        [MenuItem(EntitasMenuItems.Preferences, false, EntitasMenuItemPriorities.Preferences)]
        public static void OpenPreferences()
        {
            var window = GetWindow<EntitasPreferencesWindow>(true, $"Entitas {CheckForUpdates.GetLocalVersion()}");
            window.minSize = new Vector2(415f, 348f);
            window.Initialize(
                "Entitas.properties",
                $"{Environment.UserName}.userproperties",
                false,
                false,
                "Entitas.Unity.Editor.EntitasPreferencesDrawer",
                "Entitas.VisualDebugging.Unity.Editor.VisualDebuggingPreferencesDrawer"
            );

            window.Show();
        }
    }
}
