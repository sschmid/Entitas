using System;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public class EntitasPreferencesWindow : PreferencesWindow
    {
        [MenuItem(EntitasMenuItems.preferences, false, EntitasMenuItemPriorities.preferences)]
        public static void OpenPreferences()
        {
            var window = EditorLayout.GetWindow<EntitasPreferencesWindow>(
                "Entitas " + CheckForUpdates.GetLocalVersion(),
                new Vector2(415f, 350)
            );

            window.Initialize(
                "Entitas.properties",
                Environment.UserName + ".userproperties",
                "Entitas.Unity.Editor.EntitasPreferencesDrawer",
                "Entitas.VisualDebugging.Unity.Editor.VisualDebuggingPreferencesDrawer"
            );

            window.Show();
        }
    }
}
