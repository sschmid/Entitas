using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Unity;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.CodeGenerator {
    public class EntitasPreferencesDrawer : IEntitasPreferencesDrawer {

        const string ENTITAS_FAST_AND_UNSAFE = "ENTITAS_FAST_AND_UNSAFE";

        enum ScriptCallOptimization {
            Disabled,
            FastAndUnsafe
        }

        Dictionary<BuildTargetGroup, string> _buildTargetToDefSymbol;
        ScriptCallOptimization _scriptCallOptimization;

        public void Initialize(EntitasPreferencesConfig config) {
            _buildTargetToDefSymbol = Enum.GetValues(typeof(BuildTargetGroup))
                .Cast<BuildTargetGroup>()
                .Where(buildTargetGroup => buildTargetGroup != BuildTargetGroup.Unknown)
                .Distinct()
                .ToDictionary(
                    buildTargetGroup => buildTargetGroup,
                    buildTargetGroup => PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup)
                );

            _scriptCallOptimization = _buildTargetToDefSymbol.Values.All<string>(defs => defs.Contains(ENTITAS_FAST_AND_UNSAFE))
                                        ? ScriptCallOptimization.FastAndUnsafe
                                        : ScriptCallOptimization.Disabled;
        }

        public void Draw(EntitasPreferencesConfig config) {
            EditorGUI.BeginChangeCheck();
            {
                EditorGUILayout.BeginVertical(GUI.skin.box);
                {
                    EditorGUILayout.LabelField("Entitas", EditorStyles.boldLabel);

                    _scriptCallOptimization = (ScriptCallOptimization)EditorGUILayout
                        .EnumPopup("Script Call Optimization", _scriptCallOptimization);
                }
                EditorGUILayout.EndVertical();
            }
            var changed = EditorGUI.EndChangeCheck();

            if (changed) {
                if (_scriptCallOptimization == ScriptCallOptimization.Disabled) {
                    foreach (var kv in _buildTargetToDefSymbol) {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(
                            kv.Key, kv.Value.Replace(ENTITAS_FAST_AND_UNSAFE, string.Empty)
                        );
                    }
                } else {
                    foreach (var kv in _buildTargetToDefSymbol) {
                        PlayerSettings.SetScriptingDefineSymbolsForGroup(
                            kv.Key, kv.Value.Replace(ENTITAS_FAST_AND_UNSAFE, string.Empty) + "," + ENTITAS_FAST_AND_UNSAFE
                        );
                    }
                }
            }
        }
    }
}
