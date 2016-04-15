using System.Collections.Generic;
using Entitas.Unity.Serialization.Blueprints;
using UnityEditor;

public class BinaryBlueprintPostprocessor : AssetPostprocessor {

    public const string ASSET_LABEL = "EntitasBinaryBlueprint";

    static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath) {
        foreach (string assetPath in importedAssets) {
            var asset = AssetDatabase.LoadAssetAtPath<BinaryBlueprint>(assetPath);
            var labels = new List<string>(AssetDatabase.GetLabels(asset));
            if (!labels.Contains(ASSET_LABEL)) {
                labels.Add(ASSET_LABEL);
                AssetDatabase.SetLabels(asset, labels.ToArray());
            }
        }
    }
}
