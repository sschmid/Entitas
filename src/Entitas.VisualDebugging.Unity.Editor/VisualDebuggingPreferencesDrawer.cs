using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor
{
    public class VisualDebuggingPreferencesDrawer : AbstractPreferencesDrawer
    {
        public override string Title => "Visual Debugging";

        const string ENTITAS_DISABLE_VISUAL_DEBUGGING = "ENTITAS_DISABLE_VISUAL_DEBUGGING";
        const string ENTITAS_DISABLE_DEEP_PROFILING = "ENTITAS_DISABLE_DEEP_PROFILING";

        VisualDebuggingConfig _visualDebuggingConfig;
        ScriptingDefineSymbols _scriptingDefineSymbols;

        bool _enableVisualDebugging;
        bool _enableDeviceDeepProfiling;

        public override void Initialize(Preferences preferences)
        {
            _visualDebuggingConfig = preferences.CreateAndConfigure<VisualDebuggingConfig>();
            preferences.Properties.AddProperties(_visualDebuggingConfig.DefaultProperties, false);
            preferences.Save();
            preferences.Reload();

            _scriptingDefineSymbols = new ScriptingDefineSymbols();
            _enableVisualDebugging = !ScriptingDefineSymbols.BuildTargetGroups
                .All(buildTarget => PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Contains(ENTITAS_DISABLE_VISUAL_DEBUGGING));
            _enableDeviceDeepProfiling = !ScriptingDefineSymbols.BuildTargetGroups
                .All(buildTarget => PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Contains(ENTITAS_DISABLE_DEEP_PROFILING));
        }

        public override void DrawHeader(Preferences preferences) { }

        protected override void OnDrawContent(Preferences preferences)
        {
            EditorGUILayout.BeginHorizontal();
            {
                drawVisualDebugging();
                if (GUILayout.Button("Show Stats", EditorStyles.miniButton))
                    EntitasStats.ShowStats();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            _visualDebuggingConfig.systemWarningThreshold = EditorGUILayout.IntField("System Warning Threshold", _visualDebuggingConfig.systemWarningThreshold);

            EditorGUILayout.Space();

            drawDefaultInstanceCreator();
            drawTypeDrawerFolder();
        }

        void drawVisualDebugging()
        {
            EditorGUILayout.BeginVertical();
            {
                EditorGUI.BeginChangeCheck();
                {
                    _enableVisualDebugging = EditorGUILayout.Toggle("Enable Visual Debugging", _enableVisualDebugging);
                }
                var visualDebuggingChanged = EditorGUI.EndChangeCheck();

                if (visualDebuggingChanged)
                {
                    if (_enableVisualDebugging)
                        _scriptingDefineSymbols.RemoveForAll(ENTITAS_DISABLE_VISUAL_DEBUGGING);
                    else
                        _scriptingDefineSymbols.AddForAll(ENTITAS_DISABLE_VISUAL_DEBUGGING);
                }

                EditorGUI.BeginChangeCheck();
                {
                    _enableDeviceDeepProfiling = EditorGUILayout.Toggle("Enable Device Profiling", _enableDeviceDeepProfiling);
                }
                var deviceDeepProfilingChanged = EditorGUI.EndChangeCheck();

                if (deviceDeepProfilingChanged)
                {
                    if (_enableDeviceDeepProfiling)
                        _scriptingDefineSymbols.RemoveForAll(ENTITAS_DISABLE_DEEP_PROFILING);
                    else
                        _scriptingDefineSymbols.AddForAll(ENTITAS_DISABLE_DEEP_PROFILING);
                }
            }
            EditorGUILayout.EndVertical();
        }

        void drawDefaultInstanceCreator()
        {
            EditorGUILayout.BeginHorizontal();
            {
                var path = EditorLayout.ObjectFieldOpenFolderPanel(
                    "Default Instance Creators",
                    _visualDebuggingConfig.defaultInstanceCreatorFolderPath,
                    _visualDebuggingConfig.defaultInstanceCreatorFolderPath
                );
                if (!string.IsNullOrEmpty(path))
                {
                    _visualDebuggingConfig.defaultInstanceCreatorFolderPath = path;
                }

                if (EditorLayout.MiniButton("New"))
                {
                    EntityDrawer.GenerateIDefaultInstanceCreator("MyType");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void drawTypeDrawerFolder()
        {
            EditorGUILayout.BeginHorizontal();
            {
                var path = EditorLayout.ObjectFieldOpenFolderPanel(
                    "Type Drawers",
                    _visualDebuggingConfig.typeDrawerFolderPath,
                    _visualDebuggingConfig.typeDrawerFolderPath
                );
                if (!string.IsNullOrEmpty(path))
                    _visualDebuggingConfig.typeDrawerFolderPath = path;

                if (EditorLayout.MiniButton("New"))
                    EntityDrawer.GenerateITypeDrawer("MyType");
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
