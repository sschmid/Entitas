using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

public static class EntitasCodeGeneratorEditor {

    const string defaultGeneratedFolder = "Assets/Sources/Generated/";
    const string generatedFolderKey = "Entitas.CodeGenerator.GeneratedFolder";

    [MenuItem("Entitas/Generate")]
    public static void EntitasGenerate() {
        var assembly = Assembly.GetAssembly(typeof(Entity));
        var dir = EditorPrefs.GetString(generatedFolderKey, defaultGeneratedFolder);
        EntitasCodeGenerator.Generate(assembly, dir);
        AssetDatabase.Refresh();
    }

    [PreferenceItem("Entitas")]
    public static void PreferenceItem() {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("CodeGenerator", EditorStyles.boldLabel);
        var dir = EditorPrefs.GetString(generatedFolderKey, defaultGeneratedFolder);
        dir = EditorGUILayout.TextField("Generated Folder", dir);
        EditorGUILayout.EndVertical();

        if (GUI.changed) {
            EditorPrefs.SetString(generatedFolderKey, dir);
        }
    }
}
