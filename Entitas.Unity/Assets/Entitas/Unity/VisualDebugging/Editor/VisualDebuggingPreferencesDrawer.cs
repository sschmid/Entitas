using System.Linq;
using Entitas.Unity;
using UnityEditor;

namespace Entitas.Unity.VisualDebugging {
    public class VisualDebuggingPreferencesDrawer : IEntitasPreferencesDrawer {

        public int priority { get { return 20; } }

        const string ENTITAS_DISABLE_VISUAL_DEBUGGING = "ENTITAS_DISABLE_VISUAL_DEBUGGING";

        VisualDebuggingConfig _visualDebuggingConfig;
        ScriptingDefineSymbols _scriptingDefineSymbols;

        bool _enableVisualDebugging;

        public void Initialize(EntitasPreferencesConfig config) {
            _visualDebuggingConfig = new VisualDebuggingConfig(config);
            _scriptingDefineSymbols = new ScriptingDefineSymbols();
            _enableVisualDebugging = !_scriptingDefineSymbols.buildTargetToDefSymbol.Values
                .All<string>(defs => defs.Contains(ENTITAS_DISABLE_VISUAL_DEBUGGING));
        }

        public void Draw(EntitasPreferencesConfig config) {
            EntitasEditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.LabelField("Visual Debugging", EditorStyles.boldLabel);

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

                EditorGUILayout.Space();

                _visualDebuggingConfig.defaultInstanceCreatorFolderPath =
                    EditorGUILayout.TextField("DefaultInstanceCreator Folder", _visualDebuggingConfig.defaultInstanceCreatorFolderPath);

                _visualDebuggingConfig.typeDrawerFolderPath =
                    EditorGUILayout.TextField("TypeDrawer Folder", _visualDebuggingConfig.typeDrawerFolderPath);
            }
            EntitasEditorLayout.EndVertical();
        }
    }
}
