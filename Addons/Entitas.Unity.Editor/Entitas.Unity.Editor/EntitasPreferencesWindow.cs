using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor {

    public class EntitasPreferencesWindow {

        [MenuItem(EntitasMenuItems.preferences, false, EntitasMenuItemPriorities.preferences)]
        public static void OpenPreferences() {
            EditorLayout.ShowWindow<PreferencesWindow>(
                "Entitas " + CheckForUpdates.GetLocalVersion(),
                new Vector2(415f, 564));
        }
    }
}
