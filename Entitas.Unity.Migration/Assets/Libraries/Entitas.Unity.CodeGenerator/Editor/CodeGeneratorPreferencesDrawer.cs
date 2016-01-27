using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Unity;
using Entitas.Unity.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.CodeGenerator {
    public class CodeGeneratorPreferencesDrawer : IEntitasPreferencesDrawer {

        Type[] _codeGenerators;
        CodeGeneratorConfig _codeGeneratorConfig;
        List<string> _pools;
        UnityEditorInternal.ReorderableList _poolList;

        public void Initialize(EntitasPreferencesConfig config) {
            _codeGenerators = CodeGenerator.GetCodeGenerators();
            var codeGeneratorNames = _codeGenerators.Select(cg => cg.Name).ToArray();
            _codeGeneratorConfig = new CodeGeneratorConfig(config, codeGeneratorNames);

            _pools = new List<string>(_codeGeneratorConfig.pools);

            _poolList = new UnityEditorInternal.ReorderableList(_pools, typeof(string), true, true, true, true);
            _poolList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Custom Pools");;
            _poolList.drawElementCallback = (rect, index, isActive, isFocused) => {
                rect.width -= 20;
                _pools[index] = EditorGUI.TextField(rect, _pools[index]);
            };
            _poolList.onAddCallback = list => list.list.Add("New Pool");
            _poolList.onCanRemoveCallback = list => true;
            _poolList.onChangedCallback = list => GUI.changed = true;
        }

        public void Draw(EntitasPreferencesConfig config) {
            EditorGUILayout.BeginVertical(GUI.skin.box);
            {
                EditorGUILayout.LabelField("CodeGenerator", EditorStyles.boldLabel);

                drawGeneratedFolderPath();
                drawPools();
                drawCodeGenerators();
                drawGenerateButton();
            }
            EditorGUILayout.EndVertical();
        }

        void drawGeneratedFolderPath() {
            _codeGeneratorConfig.generatedFolderPath = EditorGUILayout.TextField("Generated Folder", _codeGeneratorConfig.generatedFolderPath);
        }

        void drawPools() {
            EditorGUILayout.Space();

            _poolList.DoLayoutList();

            if (_pools.Count == 0) {
                EditorGUILayout.HelpBox("You can optimize the memory footprint of entities by creating multiple pools. " +
                "The code generator generates subclasses of PoolAttribute for each pool name. " +
                "You can assign components to a specific pool with the generated attribute, e.g. [UI] or [MetaGame], " +
                "otherwise they are assigned to the default pool.", MessageType.Info);
            }

            _codeGeneratorConfig.pools = _pools.ToArray();
        }

        void drawCodeGenerators() {
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Code Generators", EditorStyles.boldLabel);

            var enabledCodeGenerators = new HashSet<string>(_codeGeneratorConfig.enabledCodeGenerators);

            var availableGeneratorNames = new HashSet<string>();
            foreach (var codeGenerator in _codeGenerators) {
                availableGeneratorNames.Add(codeGenerator.Name);
                var isEnabled = enabledCodeGenerators.Contains(codeGenerator.Name);
                isEnabled = EditorGUILayout.Toggle(codeGenerator.Name, isEnabled);
                if (isEnabled) {
                    enabledCodeGenerators.Add(codeGenerator.Name);
                } else {
                    enabledCodeGenerators.Remove(codeGenerator.Name);
                }
            }

            foreach (var generatorName in _codeGeneratorConfig.enabledCodeGenerators.ToArray()) {
                if (!availableGeneratorNames.Contains(generatorName)) {
                    enabledCodeGenerators.Remove(generatorName);
                }
            }

            var sortedCodeGenerators = enabledCodeGenerators.ToArray();
            Array.Sort(sortedCodeGenerators);
            _codeGeneratorConfig.enabledCodeGenerators = sortedCodeGenerators;
        }

        static void drawGenerateButton() {
            EditorGUILayout.Space();
            if (GUILayout.Button("Generate")) {
                CodeGenerator.Generate();
            }
        }
    }
}
