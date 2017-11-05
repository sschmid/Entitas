using System;
using System.Linq;
using Entitas.Utils;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor {

    public class PreferencesWindow : EditorWindow {

        [MenuItem(EntitasMenuItems.preferences, false, EntitasMenuItemPriorities.preferences)]
        public static void OpenPreferences() {
            EntitasEditorLayout.ShowWindow<PreferencesWindow>("Entitas " + CheckForUpdates.GetLocalVersion());
        }

        Texture2D _headerTexture;
        Preferences _preferences;
        IEntitasPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        Exception _configException;

        void OnEnable() {
            _headerTexture = EntitasEditorLayout.LoadTexture("l:EntitasHeader");
            _preferencesDrawers = AppDomain.CurrentDomain
                                           .GetInstancesOf<IEntitasPreferencesDrawer>()
                                           .OrderBy(drawer => drawer.priority)
                                           .ToArray();

            try {
                _preferences = Preferences.sharedInstance;
                _preferences.Refresh();

                foreach (var drawer in _preferencesDrawers) {
                    drawer.Initialize(_preferences);
                }

                _preferences.Save();
            } catch(Exception ex) {
                _configException = ex;
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

            if (GUI.changed) {
                _preferences.Save();
            }
        }

        void drawToolbar() {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            {
                if (GUILayout.Button("Check for Updates", EditorStyles.toolbarButton)) {
                    CheckForUpdates.DisplayUpdates();
                }
                if (GUILayout.Button("Chat", EditorStyles.toolbarButton)) {
                    EntitasFeedback.EntitasChat();
                }
                if (GUILayout.Button("Docs", EditorStyles.toolbarButton)) {
                    EntitasFeedback.EntitasDocs();
                }
                if (GUILayout.Button("Wiki", EditorStyles.toolbarButton)) {
                    EntitasFeedback.EntitasWiki();
                }
                if (GUILayout.Button("Donate", EditorStyles.toolbarButton)) {
                    EntitasFeedback.Donate();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void drawHeader() {
            var rect = EntitasEditorLayout.DrawTexture(_headerTexture);
            if (rect.Contains(Event.current.mousePosition) && Event.current.clickCount > 0) {
                //Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/blob/develop/README.md");
            }

            const int buttonWidth = 60;
            const int buttonHeight = 15;
            const int padding = 4;
            var buttonRect = new Rect(
                rect.width - buttonWidth - padding,
                rect.height - buttonHeight - padding,
                buttonWidth,
                buttonHeight
            );
            if (GUI.Button(buttonRect, "Edit", EditorStyles.miniButton)) {
                System.Diagnostics.Process.Start(_preferences.propertiesPath);
                Close();
            }
        }

        void drawPreferencesDrawers() {
            if (_configException == null) {
                for (int i = 0; i < _preferencesDrawers.Length; i++) {
                    try {
                        _preferencesDrawers[i].Draw(_preferences);
                    } catch(Exception ex) {
                        drawException(ex);
                    }

                    if (i < _preferencesDrawers.Length -1) {
                        EditorGUILayout.Space();
                    }
                }
            } else {
                drawException(_configException);
            }
        }

        void drawException(Exception exception) {
            var style = new GUIStyle(GUI.skin.label);
            style.wordWrap = true;
            style.normal.textColor = Color.red;

            EditorGUILayout.LabelField(exception.Message, style);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Please make sure Entitas.properties is set up correctly.");
        }
    }
}
