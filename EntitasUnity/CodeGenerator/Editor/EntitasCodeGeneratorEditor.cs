using System;
using System.Collections.Generic;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

public static class EntitasCodeGeneratorEditor {

    const string defaultGeneratedFolder = "Assets/Sources/Generated/";
    const string generatedFolderKey = "Entitas.CodeGenerator.GeneratedFolder";

    const string poolsKey = "Entitas.CodeGenerator.Pools";

    [MenuItem("Entitas/Generate")]
    public static void EntitasGenerate() {
        var types = Assembly.GetAssembly(typeof(Entity)).GetTypes();
        var poolNames = EditorPrefs.GetString(poolsKey, string.Empty).Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries);
        var dir = EditorPrefs.GetString(generatedFolderKey, defaultGeneratedFolder);
        EntitasCodeGenerator.Generate(types, poolNames, dir);
        AssetDatabase.Refresh();
    }

    [PreferenceItem("Entitas")]
    public static void PreferenceItem() {
        drawCodeGenerator();
    }

    static void drawCodeGenerator() {
        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("CodeGenerator", EditorStyles.boldLabel);

        // Generated Folder
        var dir = EditorPrefs.GetString(generatedFolderKey, defaultGeneratedFolder);
        dir = EditorGUILayout.TextField("Generated Folder", dir);

        // Pools
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Pools");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("DefaultPool");
        EditorGUI.EndDisabledGroup();

        var pools = new List<string>(EditorPrefs.GetString(poolsKey, string.Empty).Split(new [] { ',' }, StringSplitOptions.RemoveEmptyEntries));
        for (int i = 0; i < pools.Count; i++) {
            EditorGUILayout.BeginHorizontal();
            pools[i] = EditorGUILayout.TextField(pools[i]);
            if (GUILayout.Button("-", GUILayout.Width(19), GUILayout.Height(14))) {
                pools[i] = string.Empty;
            }
            EditorGUILayout.EndHorizontal();
        }

        if (GUILayout.Button("Add pool")) {
            pools.Add("PoolName");
        }

        var poolNames = string.Join(",", pools.ToArray()).Replace(" ", string.Empty);
        if (poolNames == string.Empty) {
            EditorGUILayout.HelpBox("You can optimize the memory footprint of entities by creating multiple pools. " +
                "The code generator generates subclasses of PoolAttribute for each pool name. " +
                "You can assign components to a specific pool with the generated attribute, e.g. [UI] or [MetaGame], " +
                "otherwise they are assigned to the default pool.", MessageType.Info);
        }

        EditorGUILayout.EndVertical();

        if (GUI.changed) {
            EditorPrefs.SetString(generatedFolderKey, dir);
            EditorPrefs.SetString(poolsKey, poolNames);
        }
    }
}
