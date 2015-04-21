using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

public static class EntitasCodeGeneratorEditor {

    const string configPath = "Entitas.properties";

    [MenuItem("Entitas/Generate")]
    public static void Generate() {
        var types = Assembly.GetAssembly(typeof(Entity)).GetTypes();
        var config = loadConfig();
        EntitasCodeGenerator.Generate(types, config.pools, config.generatedFolderPath);
        AssetDatabase.Refresh();
    }

    [PreferenceItem("Entitas")]
    public static void PreferenceItem() {
        drawCodeGenerator();
    }

    static void drawCodeGenerator() {
        var config = loadConfig();

        EditorGUILayout.BeginVertical(GUI.skin.box);
        EditorGUILayout.LabelField("CodeGenerator", EditorStyles.boldLabel);

        // Generated Folder
        config.generatedFolderPath = EditorGUILayout.TextField("Generated Folder", config.generatedFolderPath);

        // Pools
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Pools");
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("DefaultPool");
        EditorGUI.EndDisabledGroup();

        var pools = new List<string>(config.pools);
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

        if (pools.Count == 0) {
            EditorGUILayout.HelpBox("You can optimize the memory footprint of entities by creating multiple pools. " +
                "The code generator generates subclasses of PoolAttribute for each pool name. " +
                "You can assign components to a specific pool with the generated attribute, e.g. [UI] or [MetaGame], " +
                "otherwise they are assigned to the default pool.", MessageType.Info);
        }

        if (GUI.changed) {
            config.pools = pools.ToArray();
            saveConfig(config);
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("Generate")) {
            Generate();
        }

        EditorGUILayout.EndVertical();
    }

    static CodeGeneratorConfig loadConfig() {
        return new CodeGeneratorConfig(File.Exists(configPath) ? File.ReadAllText(configPath) : string.Empty);
    }

    static void saveConfig(CodeGeneratorConfig config) {
        File.WriteAllText(configPath, config.ToString());
    }
}
