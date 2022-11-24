using System.Linq;
using DesperateDevs.Serialization;
using DesperateDevs.Unity.Editor;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public class VisualDebuggingPreferencesDrawer : AbstractPreferencesDrawer
    {
        public override string Title => "Visual Debugging";

        const string EntitasDisableVisualDebugging = "ENTITAS_DISABLE_VISUAL_DEBUGGING";
        const string EntitasDisableDeepProfiling = "ENTITAS_DISABLE_DEEP_PROFILING";

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
                .All(buildTarget => PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Contains(EntitasDisableVisualDebugging));
            _enableDeviceDeepProfiling = !ScriptingDefineSymbols.BuildTargetGroups
                .All(buildTarget => PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Contains(EntitasDisableDeepProfiling));
        }

        public override void DrawHeader(Preferences preferences) { }

        protected override void OnDrawContent(Preferences preferences)
        {
            EditorGUILayout.BeginHorizontal();
            {
                DrawVisualDebugging();
                if (GUILayout.Button("Show Stats", EditorStyles.miniButton))
                    EntitasStats.ShowStats();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            _visualDebuggingConfig.SystemWarningThreshold = EditorGUILayout.IntField("System Warning Threshold", _visualDebuggingConfig.SystemWarningThreshold);

            EditorGUILayout.Space();

            DrawDefaultInstanceCreator();
            DrawTypeDrawerFolder();
        }

        void DrawVisualDebugging()
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
                        _scriptingDefineSymbols.RemoveForAll(EntitasDisableVisualDebugging);
                    else
                        _scriptingDefineSymbols.AddForAll(EntitasDisableVisualDebugging);
                }

                EditorGUI.BeginChangeCheck();
                {
                    _enableDeviceDeepProfiling = EditorGUILayout.Toggle("Enable Device Profiling", _enableDeviceDeepProfiling);
                }
                var deviceDeepProfilingChanged = EditorGUI.EndChangeCheck();

                if (deviceDeepProfilingChanged)
                {
                    if (_enableDeviceDeepProfiling)
                        _scriptingDefineSymbols.RemoveForAll(EntitasDisableDeepProfiling);
                    else
                        _scriptingDefineSymbols.AddForAll(EntitasDisableDeepProfiling);
                }
            }
            EditorGUILayout.EndVertical();
        }

        void DrawDefaultInstanceCreator()
        {
            EditorGUILayout.BeginHorizontal();
            {
                var path = EditorLayout.ObjectFieldOpenFolderPanel(
                    "Default Instance Creators",
                    _visualDebuggingConfig.DefaultInstanceCreatorFolderPath,
                    _visualDebuggingConfig.DefaultInstanceCreatorFolderPath
                );
                if (!string.IsNullOrEmpty(path))
                {
                    _visualDebuggingConfig.DefaultInstanceCreatorFolderPath = path;
                }

                if (EditorLayout.MiniButton("New"))
                {
                    EntityDrawer.GenerateIDefaultInstanceCreator("MyType");
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        void DrawTypeDrawerFolder()
        {
            EditorGUILayout.BeginHorizontal();
            {
                var path = EditorLayout.ObjectFieldOpenFolderPanel(
                    "Type Drawers",
                    _visualDebuggingConfig.TypeDrawerFolderPath,
                    _visualDebuggingConfig.TypeDrawerFolderPath
                );
                if (!string.IsNullOrEmpty(path))
                    _visualDebuggingConfig.TypeDrawerFolderPath = path;

                if (EditorLayout.MiniButton("New"))
                    EntityDrawer.GenerateITypeDrawer("MyType");
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
