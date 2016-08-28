using System.Collections.Generic;
using System.Linq;
using Entitas.Serialization.Configuration;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.CodeGenerator {

    public class CodeGeneratorPreferencesDrawer : IEntitasPreferencesDrawer {

        public int priority { get { return EntitasPreferencesDrawerPriorities.codeGenerator; } }

        string[] _availableGeneratorNames;
        CodeGeneratorConfig _codeGeneratorConfig;
        List<string> _pools;
        UnityEditorInternal.ReorderableList _poolList;

        public void Initialize(EntitasPreferencesConfig config) {
            _availableGeneratorNames = UnityCodeGenerator.GetCodeGenerators()
                                                         .Select(cg => cg.Name)
                                                         .OrderBy(generatorName => generatorName)
                                                         .ToArray();

            _codeGeneratorConfig = new CodeGeneratorConfig(config, _availableGeneratorNames);

            _pools = new List<string>(_codeGeneratorConfig.pools);

            _poolList = new UnityEditorInternal.ReorderableList(_pools, typeof(string), true, true, true, true);
            _poolList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Custom Pools");
            _poolList.drawElementCallback = (rect, index, isActive, isFocused) => {
                rect.width -= 20;
                _pools[index] = EditorGUI.TextField(rect, _pools[index]);
            };
            _poolList.onAddCallback = list => list.list.Add("New Pool");
            _poolList.onCanRemoveCallback = list => list.count > 1;
            _poolList.onChangedCallback = list => GUI.changed = true;
        }

        public void Draw(EntitasPreferencesConfig config) {
            EntitasEditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.LabelField("Code Generator", EditorStyles.boldLabel);

                drawGeneratedFolderPath();
                drawPools();
                drawCodeGenerators();
            }
            EntitasEditorLayout.EndVertical();
        }

        void drawGeneratedFolderPath() {
            _codeGeneratorConfig.generatedFolderPath = EditorGUILayout.TextField("Generated Folder", _codeGeneratorConfig.generatedFolderPath);
        }

        void drawPools() {
            EditorGUILayout.Space();

            _poolList.DoLayoutList();

            if(_pools.Count <= 1) {
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

            var enabledCodeGeneratorsMask = 0;

            for(int i = 0; i < _availableGeneratorNames.Length; i++) {
                if(_codeGeneratorConfig.enabledCodeGenerators.Contains(_availableGeneratorNames[i])) {
                    enabledCodeGeneratorsMask += (1 << i);
                }
            }

            enabledCodeGeneratorsMask = EditorGUILayout.MaskField("Code Generators", enabledCodeGeneratorsMask, _availableGeneratorNames);

            var enabledCodeGenerators = new List<string>();
            for(int i = 0; i < _availableGeneratorNames.Length; i++) {
                var index = 1 << i;
                if((index & enabledCodeGeneratorsMask) == index) {
                    enabledCodeGenerators.Add(_availableGeneratorNames[i]);
                }
            }

            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if(GUILayout.Button("Generate", GUILayout.Height(32))) {
                UnityCodeGenerator.Generate();
            }
            GUI.backgroundColor = bgColor;

            _codeGeneratorConfig.enabledCodeGenerators = enabledCodeGenerators.ToArray();
        }
    }
}
