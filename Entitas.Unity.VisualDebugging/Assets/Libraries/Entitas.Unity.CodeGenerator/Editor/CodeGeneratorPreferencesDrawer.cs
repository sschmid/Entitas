using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Unity;
using Entitas.Unity.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.CodeGenerator {
    public class CodeGeneratorPreferencesDrawer : IEntitasPreferencesDrawer {
        public void Draw(EntitasPreferencesConfig config) {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            EditorGUILayout.LabelField("CodeGenerator", EditorStyles.boldLabel);

            var codeGenerators = CodeGenerator.GetCodeGenerators();
            var codeGeneratorNames = codeGenerators.Select(cg => cg.Name).ToArray();
            var codeGeneratorConfig = new CodeGeneratorConfig(config, codeGeneratorNames);
            drawGeneratedFolderPath(codeGeneratorConfig);
            drawPools(codeGeneratorConfig);
            drawCodeGenerators(codeGeneratorConfig, codeGenerators);
            drawGenerateButton();

            EditorGUILayout.EndVertical();
        }

        static void drawGeneratedFolderPath(CodeGeneratorConfig codeGeneratorConfig) {
            codeGeneratorConfig.generatedFolderPath = EditorGUILayout.TextField("Generated Folder", codeGeneratorConfig.generatedFolderPath);
        }

        static void drawPools(CodeGeneratorConfig codeGeneratorConfig) {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Pools");
            
            var pools = new List<string>(codeGeneratorConfig.pools);
            if (pools.Count == 0) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("Pool");
                EditorGUI.EndDisabledGroup();
            }

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

            codeGeneratorConfig.pools = pools.ToArray();
        }

        static void drawCodeGenerators(CodeGeneratorConfig codeGeneratorConfig, Type[] codeGenerators) {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Code Generators", EditorStyles.boldLabel);

            var enabledCodeGenerators = new HashSet<string>(codeGeneratorConfig.enabledCodeGenerators);

            var availableGeneratorNames = new HashSet<string>();
            foreach (var codeGenerator in codeGenerators) {
                availableGeneratorNames.Add(codeGenerator.Name);
                var isEnabled = enabledCodeGenerators.Contains(codeGenerator.Name);
                isEnabled = EditorGUILayout.Toggle(codeGenerator.Name, isEnabled);
                if (isEnabled) {
                    enabledCodeGenerators.Add(codeGenerator.Name);
                } else {
                    enabledCodeGenerators.Remove(codeGenerator.Name);
                }
            }

            foreach (var generatorName in codeGeneratorConfig.enabledCodeGenerators.ToArray()) {
                if (!availableGeneratorNames.Contains(generatorName)) {
                    enabledCodeGenerators.Remove(generatorName);
                }
            }

            var sortedCodeGenerators = enabledCodeGenerators.ToArray();
            Array.Sort(sortedCodeGenerators);
            codeGeneratorConfig.enabledCodeGenerators = sortedCodeGenerators;
        }

        static void drawGenerateButton() {
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate")) {
                CodeGenerator.Generate();
            }
        }
    }
}
