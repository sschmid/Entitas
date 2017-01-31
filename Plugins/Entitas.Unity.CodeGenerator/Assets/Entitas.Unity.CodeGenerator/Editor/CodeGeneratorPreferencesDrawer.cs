using System.Collections.Generic;
using System.Linq;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.CodeGenerator {

    public class CodeGeneratorPreferencesDrawer : IEntitasPreferencesDrawer {

        public int priority { get { return EntitasPreferencesDrawerPriorities.codeGenerator; } }

        string[] _availableGeneratorNames;
        CodeGeneratorConfig _codeGeneratorConfig;
        List<string> _contexts;
        UnityEditorInternal.ReorderableList _contextList;

        public void Initialize(EntitasPreferencesConfig config) {
            _availableGeneratorNames = UnityCodeGenerator.GetCodeGenerators()
                                                         .Select(cg => cg.Name)
                                                         .OrderBy(generatorName => generatorName)
                                                         .ToArray();

            _codeGeneratorConfig = new CodeGeneratorConfig(config, _availableGeneratorNames);

            _contexts = new List<string>(_codeGeneratorConfig.contexts);

            _contextList = new UnityEditorInternal.ReorderableList(_contexts, typeof(string), true, true, true, true);
            _contextList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Contexts");
            _contextList.drawElementCallback = (rect, index, isActive, isFocused) => {
                rect.width -= 20;
                _contexts[index] = EditorGUI.TextField(rect, _contexts[index]);
            };
            _contextList.onAddCallback = list => list.list.Add("New Context");
            _contextList.onCanRemoveCallback = list => true;
            _contextList.onChangedCallback = list => GUI.changed = true;
        }

        public void Draw(EntitasPreferencesConfig config) {
            EntitasEditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.LabelField("Code Generator", EditorStyles.boldLabel);

                drawGeneratedFolderPath();
                drawContexts();
                drawCodeGenerators();
            }
            EntitasEditorLayout.EndVertical();
        }

        void drawGeneratedFolderPath() {
            _codeGeneratorConfig.generatedFolderPath = EditorGUILayout.TextField("Generated Folder", _codeGeneratorConfig.generatedFolderPath);
        }

        void drawContexts() {
            EditorGUILayout.Space();

            _contextList.DoLayoutList();

            if(_contexts.Count <= 1) {
                EditorGUILayout.HelpBox("You can optimize the memory footprint of entities by creating multiple contexts. " +
                "The code generator generates subclasses of ContextAttribute for each context name. " +
                "You can assign components to a specific context with the generated attribute, e.g. [UI] or [MetaGame], " +
                "otherwise they are assigned to the default context.", MessageType.Info);
            }

            _codeGeneratorConfig.contexts = _contexts.ToArray();
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
