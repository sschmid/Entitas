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

            var codeGeneratorConfig = new CodeGeneratorConfig(config);
            drawGeneratedFolderPath(codeGeneratorConfig);
            drawPools(codeGeneratorConfig);
            drawCodeGenerators(codeGeneratorConfig);
            drawGenerateButton();

            EditorGUILayout.EndVertical();
        }

        void drawGeneratedFolderPath(CodeGeneratorConfig codeGeneratorConfig) {
            codeGeneratorConfig.generatedFolderPath = EditorGUILayout.TextField("Generated Folder", codeGeneratorConfig.generatedFolderPath);
        }

        void drawPools(CodeGeneratorConfig codeGeneratorConfig) {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Pools");
            
            var pools = new List<string>(codeGeneratorConfig.pools);
            if (pools.Count == 0) {
                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.TextField("DefaultPool");
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

        void drawCodeGenerators(CodeGeneratorConfig codeGeneratorConfig) {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Code Generators", EditorStyles.boldLabel);

            var codeGenerators = CodeGeneratorEditor.GetCodeGenerators();
            var disabledCodeGenerators = new HashSet<string>(codeGeneratorConfig.disabledCodeGenerators);

            foreach (var codeGenerator in codeGenerators) {
                var isEnabled = !disabledCodeGenerators.Contains(codeGenerator.Name);
                isEnabled = EditorGUILayout.Toggle(codeGenerator.Name, isEnabled);
                if (isEnabled) {
                    disabledCodeGenerators.Remove(codeGenerator.Name);
                } else {
                    disabledCodeGenerators.Add(codeGenerator.Name);
                }
            }

            codeGeneratorConfig.disabledCodeGenerators = disabledCodeGenerators.ToArray();
        }

        void drawGenerateButton() {
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate")) {
                CodeGeneratorEditor.Generate();
            }
        }
    }
}
