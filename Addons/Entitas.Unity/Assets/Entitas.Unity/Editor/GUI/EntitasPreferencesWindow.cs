using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {

    public class EntitasPreferencesWindow : EditorWindow {

        [MenuItem(EntitasMenuItems.preferences, false, EntitasMenuItemPriorities.preferences)]
        public static void OpenPreferences() {
            EntitasEditorLayout.ShowWindow<EntitasPreferencesWindow>("Entitas Preferences");
        }

        Texture2D _headerTexture;
        string _localVersion;
        EntitasPreferencesConfig _config;
        IEntitasPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        void OnEnable() {
            _headerTexture = EntitasEditorLayout.LoadTexture("l:EntitasHeader");
            _localVersion = EntitasCheckForUpdates.GetLocalVersion();
            _config = EntitasPreferences.LoadConfig();
            _preferencesDrawers = Assembly.GetAssembly(typeof(IEntitasPreferencesDrawer)).GetTypes()
                .Where(type => type.ImplementsInterface<IEntitasPreferencesDrawer>())
                .Where(type => !type.IsAbstract)
                .Select(type => (IEntitasPreferencesDrawer)Activator.CreateInstance(type))
                .OrderBy(drawer => drawer.priority)
                .ToArray();

            foreach(var drawer in _preferencesDrawers) {
                drawer.Initialize(_config);
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
                EntitasPreferences.SaveConfig(_config);
            }
        }

        void drawToolbar() {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar, GUILayout.ExpandWidth(true));
            {
                if(GUILayout.Button("Check for Updates", EditorStyles.toolbarButton)) {
                    EntitasCheckForUpdates.CheckForUpdates();
                }
                if(GUILayout.Button("Chat", EditorStyles.toolbarButton)) {
                    EntitasFeedback.EntitasChat();
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
            var offsetY = EntitasEditorLayout.DrawHeaderTexture(this, _headerTexture);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Version: " + _localVersion);
            GUILayout.Space(offsetY - 34);
        }

        void drawPreferencesDrawers() {
            foreach(var drawer in _preferencesDrawers) {
                EditorGUILayout.Space();
                drawer.Draw(_config);
            }
        }
    }
}
