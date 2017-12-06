using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor {

    public class VisualDebuggingPreferencesDrawer : AbstractPreferencesDrawer {

        public override int priority { get { return 20; } }
        public override string title { get { return "Visual Debugging"; } }

        const string ENTITAS_DISABLE_VISUAL_DEBUGGING = "ENTITAS_DISABLE_VISUAL_DEBUGGING";

        VisualDebuggingConfig _visualDebuggingConfig;
        ScriptingDefineSymbols _scriptingDefineSymbols;

        bool _enableVisualDebugging;

        public override void Initialize(Preferences preferences) {
            _visualDebuggingConfig = preferences.CreateAndConfigure<VisualDebuggingConfig>();
            preferences.properties.AddProperties(_visualDebuggingConfig.defaultProperties, false);

            _scriptingDefineSymbols = new ScriptingDefineSymbols();
            _enableVisualDebugging = !_scriptingDefineSymbols.buildTargetToDefSymbol.Values
                .All<string>(defs => defs.Contains(ENTITAS_DISABLE_VISUAL_DEBUGGING));
        }

        public override void DrawHeader(Preferences preferences) {
        }

        protected override void drawContent(Preferences preferences) {
            EditorGUILayout.BeginHorizontal();
            {
                drawVisualDebugging();
                if (GUILayout.Button("Show Stats", EditorStyles.miniButton)) {
                    EntitasStats.ShowStats();
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            _visualDebuggingConfig.systemWarningThreshold = EditorGUILayout.IntField("System Warning Threshold", _visualDebuggingConfig.systemWarningThreshold);

            EditorGUILayout.Space();

            drawDefaultInstanceCreator();
            drawTypeDrawerFolder();
        }

        void drawVisualDebugging() {
            EditorGUI.BeginChangeCheck();
            {
                _enableVisualDebugging = EditorGUILayout.Toggle("Enable Visual Debugging", _enableVisualDebugging);
            }
            var changed = EditorGUI.EndChangeCheck();

            if (changed) {
                if (_enableVisualDebugging) {
                    _scriptingDefineSymbols.RemoveDefineSymbol(ENTITAS_DISABLE_VISUAL_DEBUGGING);
                } else {
                    _scriptingDefineSymbols.AddDefineSymbol(ENTITAS_DISABLE_VISUAL_DEBUGGING);
                }
            }
        }

        void drawDefaultInstanceCreator() {
            var path = EditorLayout.ObjectFieldOpenFolderPanel(
                "Default Instance Creators",
                _visualDebuggingConfig.defaultInstanceCreatorFolderPath,
                _visualDebuggingConfig.defaultInstanceCreatorFolderPath
            );
            if (!string.IsNullOrEmpty(path)) {
                _visualDebuggingConfig.defaultInstanceCreatorFolderPath = path;
            }
        }

        void drawTypeDrawerFolder() {
            var path = EditorLayout.ObjectFieldOpenFolderPanel(
                "Type Drawers",
                _visualDebuggingConfig.typeDrawerFolderPath,
                _visualDebuggingConfig.typeDrawerFolderPath
            );
            if (!string.IsNullOrEmpty(path)) {
                _visualDebuggingConfig.typeDrawerFolderPath = path;
            }
        }
    }
}
