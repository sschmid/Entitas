using Entitas.Unity;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public class VisualDebuggingPreferencesDrawer : IEntitasPreferencesDrawer {
        public void Draw(EntitasPreferencesConfig config) {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("Visual Debugging", EditorStyles.boldLabel);


            config["Entitas.Unity.VisualDebugging"] = EditorGUILayout.TextField("Test", config.GetValueOrDefault("Entitas.Unity.VisualDebugging", "Hello"));

//        // Generated Folder
//        codeGeneratorConfig.generatedFolderPath = EditorGUILayout.TextField("Generated Folder", codeGeneratorConfig.generatedFolderPath);
//
//        // Pools
//        EditorGUILayout.Space();
//        EditorGUILayout.LabelField("Pools");
//        EditorGUI.BeginDisabledGroup(true);
//        EditorGUILayout.TextField("DefaultPool");
//        EditorGUI.EndDisabledGroup();
//
//        var pools = new List<string>(codeGeneratorConfig.pools);
//        for (int i = 0; i < pools.Count; i++) {
//            EditorGUILayout.BeginHorizontal();
//            pools[i] = EditorGUILayout.TextField(pools[i]);
//            if (GUILayout.Button("-", GUILayout.Width(19), GUILayout.Height(14))) {
//                pools[i] = string.Empty;
//            }
//            EditorGUILayout.EndHorizontal();
//        }
//
//        if (GUILayout.Button("Add pool")) {
//            pools.Add("PoolName");
//        }
//
//        if (pools.Count == 0) {
//            EditorGUILayout.HelpBox("You can optimize the memory footprint of entities by creating multiple pools. " +
//                "The code generator generates subclasses of PoolAttribute for each pool name. " +
//                "You can assign components to a specific pool with the generated attribute, e.g. [UI] or [MetaGame], " +
//                "otherwise they are assigned to the default pool.", MessageType.Info);
//        }
//
//        codeGeneratorConfig.pools = pools.ToArray();
//
//        EditorGUILayout.Space();
//        if (GUILayout.Button("Generate")) {
//            CodeGeneratorEditor.Generate();
//        }

            EditorGUILayout.EndVertical();
        }
    }
}
