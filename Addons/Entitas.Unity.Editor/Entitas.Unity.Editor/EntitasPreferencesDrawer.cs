using System;
using System.IO;
using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor {

    public class EntitasPreferencesDrawer : AbstractPreferencesDrawer {

        public override int priority { get { return 0; } }
        public override string title { get { return "Entitas"; } }

        const string ENTITAS_FAST_AND_UNSAFE = "ENTITAS_FAST_AND_UNSAFE";

        enum AERCMode {
            Safe,
            FastAndUnsafe
        }

        Texture2D _headerTexture;
        ScriptingDefineSymbols _scriptingDefineSymbols;
        AERCMode _scriptCallOptimization;

        public override void Initialize(Preferences preferences) {
            _headerTexture = EditorLayout.LoadTexture("l:EntitasHeader");

            _scriptingDefineSymbols = new ScriptingDefineSymbols();
            _scriptCallOptimization = _scriptingDefineSymbols.buildTargetToDefSymbol.Values
                .All<string>(defs => defs.Contains(ENTITAS_FAST_AND_UNSAFE))
                ? AERCMode.FastAndUnsafe
                : AERCMode.Safe;
        }

        public override void DrawHeader(Preferences preferences) {
            drawToolbar();
            drawHeader(preferences);
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

        void drawHeader(Preferences preferences) {
            var rect = EditorLayout.DrawTexture(_headerTexture);
//            if (rect.Contains(Event.current.mousePosition) && Event.current.clickCount > 0) {
//                Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/blob/develop/README.md");
//            }

            var propertiesPath = Path.GetFileName(preferences.propertiesPath);

            var buttonWidth = 60 + propertiesPath.Length * 5;
            const int buttonHeight = 15;
            const int padding = 4;
            var buttonRect = new Rect(
                rect.width - buttonWidth - padding,
                rect.y + rect.height - buttonHeight - padding,
                buttonWidth,
                buttonHeight
            );

            var allPreferences = Preferences.FindAll("*.properties")
                .Select(Path.GetFileName)
                .ToArray();

            if (allPreferences.Length > 1) {
                var r = new Rect(
                    rect.width - 50 - padding,
                    buttonRect.y,
                    50,
                    buttonHeight
                );

                if (GUI.Button(r, "Switch", EditorStyles.miniButton)) {
                    var path = EditorPrefs.GetString(PreferencesWindow.PREFERENCES_KEY);
                    var index = Array.IndexOf(allPreferences, path);
                    if (index >= 0) {
                        index += 1;
                        if (index >= allPreferences.Length) {
                            index = 0;
                        }
                        EditorPrefs.SetString(PreferencesWindow.PREFERENCES_KEY, allPreferences[index]);
                        var window = EditorWindow.focusedWindow;
                        window.Close();
                        EntitasPreferencesWindow.OpenPreferences();
                    }
                }

                buttonRect.x -= r.width + padding;
            }

            if (GUI.Button(buttonRect, "Edit " + propertiesPath, EditorStyles.miniButton)) {
                EditorWindow.focusedWindow.Close();
                System.Diagnostics.Process.Start(preferences.propertiesPath);
            }
        }

        protected override void drawContent(Preferences preferences) {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Automatic Entity Reference Counting");
                var buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
                if (_scriptCallOptimization == AERCMode.Safe) {
                    buttonStyle.normal = buttonStyle.active;
                }
                if (GUILayout.Button("Safe", buttonStyle)) {
                    _scriptCallOptimization = AERCMode.Safe;
                    _scriptingDefineSymbols.RemoveDefineSymbol(ENTITAS_FAST_AND_UNSAFE);
                }

                buttonStyle = new GUIStyle(EditorStyles.miniButtonRight);
                if (_scriptCallOptimization == AERCMode.FastAndUnsafe) {
                    buttonStyle.normal = buttonStyle.active;
                }
                if (GUILayout.Button("Fast And Unsafe", buttonStyle)) {
                    _scriptCallOptimization = AERCMode.FastAndUnsafe;
                    _scriptingDefineSymbols.AddDefineSymbol(ENTITAS_FAST_AND_UNSAFE);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
