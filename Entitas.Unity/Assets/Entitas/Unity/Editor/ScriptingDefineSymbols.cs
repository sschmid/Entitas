using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Entitas.Unity {

    public class ScriptingDefineSymbols {

        public Dictionary<BuildTargetGroup, string> buildTargetToDefSymbol { get { return _buildTargetToDefSymbol; } }

        readonly Dictionary<BuildTargetGroup, string> _buildTargetToDefSymbol;

        public ScriptingDefineSymbols() {
            _buildTargetToDefSymbol = Enum.GetValues(typeof(BuildTargetGroup))
                .Cast<BuildTargetGroup>()
                .Where(buildTargetGroup => buildTargetGroup != BuildTargetGroup.Unknown)
                .Where(buildTargetGroup => !isBuildTargetObsolete(buildTargetGroup))
                .Distinct()
                    .ToDictionary(
                    buildTargetGroup => buildTargetGroup,
                    buildTargetGroup => PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup)
                );
        }

        public void AddDefineSymbol(string defineSymbol) {
            foreach(var kv in _buildTargetToDefSymbol) {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    kv.Key, kv.Value.Replace(defineSymbol, string.Empty) + "," + defineSymbol
                );
            }
        }

        public void RemoveDefineSymbol(string defineSymbol) {
            foreach(var kv in _buildTargetToDefSymbol) {
                PlayerSettings.SetScriptingDefineSymbolsForGroup(
                    kv.Key, kv.Value.Replace(defineSymbol, string.Empty)
                );
            }
        }

        bool isBuildTargetObsolete(BuildTargetGroup value) {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = (ObsoleteAttribute[])fieldInfo.GetCustomAttributes(typeof(ObsoleteAttribute), false);
            return (attributes != null && attributes.Length > 0);
        }
    }
}
