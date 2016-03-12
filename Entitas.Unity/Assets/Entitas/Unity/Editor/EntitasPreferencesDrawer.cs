using System;
using System.Linq;
using Entitas.Unity;
using UnityEditor;

namespace Entitas.Unity {
    public class EntitasPreferencesDrawer : IEntitasPreferencesDrawer {
        public int priority { get { return 0; } }

        const string ENTITAS_FAST_AND_UNSAFE = "ENTITAS_FAST_AND_UNSAFE";

        enum ScriptCallOptimization {
            Disabled,
            FastAndUnsafe
        }

        ScriptingDefineSymbols _scriptingDefineSymbols;
        ScriptCallOptimization _scriptCallOptimization;

        public void Initialize(EntitasPreferencesConfig config) {
            _scriptingDefineSymbols = new ScriptingDefineSymbols();
            _scriptCallOptimization = _scriptingDefineSymbols.buildTargetToDefSymbol.Values
                                            .All<string>(defs => defs.Contains(ENTITAS_FAST_AND_UNSAFE))
                                                ? ScriptCallOptimization.FastAndUnsafe
                                                : ScriptCallOptimization.Disabled;
        }

        public void Draw(EntitasPreferencesConfig config) {
            EditorGUI.BeginChangeCheck();
            {
                EntitasEditorLayout.BeginVerticalBox();
                {
                    EditorGUILayout.LabelField("Entitas", EditorStyles.boldLabel);

                    _scriptCallOptimization = (ScriptCallOptimization)EditorGUILayout
                        .EnumPopup("Optimizations", _scriptCallOptimization);
                }
                EntitasEditorLayout.EndVertical();
            }
            var changed = EditorGUI.EndChangeCheck();

            if (changed) {
                if (_scriptCallOptimization == ScriptCallOptimization.Disabled) {
                    _scriptingDefineSymbols.RemoveDefineSymbol(ENTITAS_FAST_AND_UNSAFE);
                } else {
                    _scriptingDefineSymbols.AddDefineSymbol(ENTITAS_FAST_AND_UNSAFE);
                }
            }
        }
    }
}
