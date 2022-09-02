using System.Collections.Generic;
using UnityEditor;

namespace Entitas.Blueprints.Unity.Editor {

    public class BinaryBlueprintPostprocessor : AssetPostprocessor {

        public const string ASSET_LABEL = "EntitasBinaryBlueprint";

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath) {
            foreach (var assetPath in importedAssets) {
                var asset = AssetDatabase.LoadAssetAtPath<BinaryBlueprint>(assetPath);
                if (asset != null) {
                    var labels = new List<string>(AssetDatabase.GetLabels(asset));
                    if (!labels.Contains(ASSET_LABEL)) {
                        labels.Add(ASSET_LABEL);
                        AssetDatabase.SetLabels(asset, labels.ToArray());
                    }
                }
            }
        }
    }
}
