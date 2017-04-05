using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor {

    public class PreferencesWindow : EditorWindow {

        [MenuItem(EntitasMenuItems.preferences, false, EntitasMenuItemPriorities.preferences)]
        public static void OpenPreferences() {
            EntitasEditorLayout.ShowWindow<PreferencesWindow>("Entitas " + CheckForUpdates.GetLocalVersion());
        }

        Texture2D _headerTexture;
        Config _config;
        IEntitasPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        Exception _configException;

        void OnEnable() {
            _headerTexture = EntitasEditorLayout.LoadTexture("l:EntitasHeader");

            try {
                _config = Preferences.LoadConfig();
            } catch(Exception ex) {
                _configException = ex;
            }

            if(_configException == null) {
                _preferencesDrawers = AppDomain.CurrentDomain
                                               .GetInstancesOf<IEntitasPreferencesDrawer>()
                                               .OrderBy(drawer => drawer.priority)
                                               .ToArray();

                foreach(var drawer in _preferencesDrawers) {
                    drawer.Initialize(_config);
                }
            }
        }

        void OnGUI() {
            drawToolbar();
            _scrollViewPosition = EditorGUILayout.BeginScrollView(_scrollViewPosition);
            {
                drawHeader();
                drawPreferencesDrawers();
            }
            EditorGUILayout.EndScrollView();

            if(GUI.changed) {
                Preferences.SaveConfig(_config);
            }
        }

        void drawToolbar() {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            {
                if(GUILayout.Button("Check for Updates", EditorStyles.toolbarButton)) {
                    CheckForUpdates.DisplayUpdates();
                }
                if(GUILayout.Button("Chat", EditorStyles.toolbarButton)) {
                    EntitasFeedback.EntitasChat();
                }
                if(GUILayout.Button("Docs", EditorStyles.toolbarButton)) {
                    EntitasFeedback.EntitasDocs();
                }
                if(GUILayout.Button("Wiki", EditorStyles.toolbarButton)) {
                    EntitasFeedback.EntitasWiki();
                }
                if(GUILayout.Button("Donate", EditorStyles.toolbarButton)) {
                    EntitasFeedback.Donate();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void drawHeader() {
            var rect = EntitasEditorLayout.DrawTexture(_headerTexture);
            if(rect.Contains(Event.current.mousePosition) && Event.current.clickCount > 0) {
                Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/blob/develop/README.md");
            }
        }

        void drawPreferencesDrawers() {
            if(_configException == null) {
                for(int i = 0; i < _preferencesDrawers.Length; i++) {
                    _preferencesDrawers[i].Draw(_config);
                    if(i < _preferencesDrawers.Length -1) {
                        EditorGUILayout.Space();
                    }
                }
            } else {
                EditorGUILayout.LabelField("Entitas.properties is not in a correct format.");
                EditorGUILayout.Space();
                EditorGUILayout.LabelField(_configException.Message);
            }
        }
    }
}
