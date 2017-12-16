using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor {

    public class EntitasPreferencesWindow {

        [MenuItem(EntitasMenuItems.preferences, false, EntitasMenuItemPriorities.preferences)]
        public static void OpenPreferences() {
            Preferences.sharedInstance = null;
            var window = EditorLayout.GetWindow<PreferencesWindow>(
                "Entitas " + CheckForUpdates.GetLocalVersion(),
                new Vector2(415f, 600));

            window.preferencesName = "Entitas";
            window.Show();
        }
    }
}
