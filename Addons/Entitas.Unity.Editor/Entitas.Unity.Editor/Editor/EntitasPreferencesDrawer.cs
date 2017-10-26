using System.Linq;
using Entitas.Utils;
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

        ScriptingDefineSymbols _scriptingDefineSymbols;
        AERCMode _scriptCallOptimization;

        public override void Initialize(Preferences preferences) {
            _scriptingDefineSymbols = new ScriptingDefineSymbols();
            _scriptCallOptimization = _scriptingDefineSymbols.buildTargetToDefSymbol.Values
                                            .All<string>(defs => defs.Contains(ENTITAS_FAST_AND_UNSAFE))
                                                ? AERCMode.FastAndUnsafe
                                                : AERCMode.Safe;
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
