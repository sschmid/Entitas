using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor {

    public class EntitasPreferencesDrawer : AbstractEntitasPreferencesDrawer {

        public override int priority { get { return 0; } }
        public override string title { get { return "Entitas"; } }

        const string ENTITAS_FAST_AND_UNSAFE = "ENTITAS_FAST_AND_UNSAFE";

        enum ScriptCallOptimization {
            Disabled,
            FastAndUnsafe
        }

        ScriptingDefineSymbols _scriptingDefineSymbols;
        ScriptCallOptimization _scriptCallOptimization;

        public override void Initialize(EntitasPreferencesConfig config) {
            _scriptingDefineSymbols = new ScriptingDefineSymbols();
            _scriptCallOptimization = _scriptingDefineSymbols.buildTargetToDefSymbol.Values
                                            .All<string>(defs => defs.Contains(ENTITAS_FAST_AND_UNSAFE))
                                                ? ScriptCallOptimization.FastAndUnsafe
                                                : ScriptCallOptimization.Disabled;
        }

        protected override void drawContent(EntitasPreferencesConfig config) {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.LabelField("Optimizations");
                var buttonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
                if(_scriptCallOptimization == ScriptCallOptimization.Disabled) {
                    buttonStyle.normal = buttonStyle.active;
                }
                if(GUILayout.Button("Disabled", buttonStyle)) {
                    _scriptCallOptimization = ScriptCallOptimization.Disabled;
                    _scriptingDefineSymbols.RemoveDefineSymbol(ENTITAS_FAST_AND_UNSAFE);
                }

                buttonStyle = new GUIStyle(EditorStyles.miniButtonRight);
                if(_scriptCallOptimization == ScriptCallOptimization.FastAndUnsafe) {
                    buttonStyle.normal = buttonStyle.active;
                }
                if(GUILayout.Button("Fast And Unsafe", buttonStyle)) {
                    _scriptCallOptimization = ScriptCallOptimization.FastAndUnsafe;
                    _scriptingDefineSymbols.AddDefineSymbol(ENTITAS_FAST_AND_UNSAFE);
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
