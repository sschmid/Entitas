using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {

    public class EntitasPreferencesWindow : EditorWindow {

        [MenuItem(EntitasMenuItems.preferences, false, EntitasMenuItemPriorities.preferences)]
        public static void OpenPreferences() {
            EntitasEditorLayout.ShowWindow<EntitasPreferencesWindow>("Entitas " + EntitasCheckForUpdates.GetLocalVersion());
        }

        Texture2D _headerTexture;
        EntitasPreferencesConfig _config;
        IEntitasPreferencesDrawer[] _preferencesDrawers;
        Vector2 _scrollViewPosition;

        void OnEnable() {
            _headerTexture = EntitasEditorLayout.LoadTexture("l:EntitasHeader");
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
            EntitasEditorLayout.DrawTexture(_headerTexture);
        }

        void drawPreferencesDrawers() {
            for(int i = 0; i < _preferencesDrawers.Length; i++) {
                _preferencesDrawers[i].Draw(_config);
                if(i < _preferencesDrawers.Length -1) {
                    EditorGUILayout.Space();
                }
            }
        }
    }
}
