using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.CodeGenerator {

    public class CodeGeneratorPreferencesDrawer : IEntitasPreferencesDrawer {

        public int priority { get { return 10; } }

        string[] _availableDataProviderNames;
        string[] _availableGeneratorNames;
        string[] _availablePostProcessorNames;

        CodeGeneratorConfig _codeGeneratorConfig;
        List<string> _contexts;
        UnityEditorInternal.ReorderableList _contextList;

        public void Initialize(EntitasPreferencesConfig config) {
            _availableDataProviderNames = UnityCodeGenerator
                .GetTypes<ICodeGeneratorDataProvider>()
				.Where(type => !type.IsAbstract)
				.Select(type => type.Name)
                .OrderBy(name => name)
                .ToArray();

            _availableGeneratorNames = UnityCodeGenerator
                .GetTypes<ICodeGenerator>()
				.Where(type => !type.IsAbstract)
                .Select(type => type.Name)
                .OrderBy(name => name)
                .ToArray();

            _availablePostProcessorNames = UnityCodeGenerator
                .GetTypes<ICodeGenFilePostProcessor>()
				.Where(type => !type.IsAbstract)
                .Select(type => type.Name)
                .OrderBy(name => name)
                .ToArray();

            var enabledDataProviderNames = UnityCodeGenerator
                .GetTypes<ICodeGeneratorDataProvider>()
				.Where(type => !type.IsAbstract)
				.Where(type => ((ICodeGeneratorDataProvider)Activator.CreateInstance(type)).IsEnabledByDefault)
				.Select(type => type.Name)
                .OrderBy(name => name)
                .ToArray();

			var enabledGeneratorNames = UnityCodeGenerator
                .GetTypes<ICodeGenerator>()
				.Where(type => !type.IsAbstract)
				.Where(type => ((ICodeGenerator)Activator.CreateInstance(type)).IsEnabledByDefault)
                .Select(type => type.Name)
                .OrderBy(name => name)
                .ToArray();

			var enabledPostProcessorNames = UnityCodeGenerator
                .GetTypes<ICodeGenFilePostProcessor>()
				.Where(type => !type.IsAbstract)
				.Where(type => ((ICodeGenFilePostProcessor)Activator.CreateInstance(type)).IsEnabledByDefault)
                .Select(type => type.Name)
                .OrderBy(name => name)
                .ToArray();

			_codeGeneratorConfig = new CodeGeneratorConfig(config, enabledDataProviderNames, enabledGeneratorNames, enabledPostProcessorNames);

            _contexts = new List<string>(_codeGeneratorConfig.contexts);

            _contextList = new UnityEditorInternal.ReorderableList(_contexts, typeof(string), true, true, true, true);
            _contextList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "Contexts");
            _contextList.drawElementCallback = (rect, index, isActive, isFocused) => {
                rect.width -= 20;
                _contexts[index] = EditorGUI.TextField(rect, _contexts[index]);
            };
            _contextList.onAddCallback = list => list.list.Add("New Context");
            _contextList.onCanRemoveCallback = list => list.count > 1;
            _contextList.onChangedCallback = list => GUI.changed = true;
        }

        public void Draw(EntitasPreferencesConfig config) {
            EntitasEditorLayout.BeginVerticalBox();
            {
                EditorGUILayout.LabelField("Code Generator", EditorStyles.boldLabel);

                drawGeneratedFolderPath();
                drawContexts();

                _codeGeneratorConfig.dataProviders = drawMaskField("Data Providers", _availableDataProviderNames, _codeGeneratorConfig.dataProviders);
                _codeGeneratorConfig.codeGenerators = drawMaskField("Code Generators", _availableGeneratorNames, _codeGeneratorConfig.codeGenerators);
                _codeGeneratorConfig.postProcessors = drawMaskField("Post Processors", _availablePostProcessorNames, _codeGeneratorConfig.postProcessors);

                var bgColor = GUI.backgroundColor;
                GUI.backgroundColor = Color.green;
                if(GUILayout.Button("Generate", GUILayout.Height(32))) {
                    UnityCodeGenerator.Generate();
                }
                GUI.backgroundColor = bgColor;
            }
            EntitasEditorLayout.EndVertical();
        }

        void drawGeneratedFolderPath() {
            _codeGeneratorConfig.targetDirectory = EditorGUILayout.TextField("Target Directory", _codeGeneratorConfig.targetDirectory);
        }

        void drawContexts() {
            EditorGUILayout.Space();

            _contextList.DoLayoutList();

            if(_contexts.Count <= 1) {
                EditorGUILayout.HelpBox("You can optimize the memory footprint of entities by creating multiple contexts. " +
                "The code generator generates subclasses of ContextAttribute for each context name. " +
                "You have to assign components to one or more contexts with the generated attribute, e.g. [Game] or [Input], " +
                "otherwise they will be ignored by the code generator.", MessageType.Info);
            }

            _codeGeneratorConfig.contexts = _contexts.ToArray();
        }

        static string[] drawMaskField(string title, string[] names, string[] input) {
            var mask = 0;

            for(int i = 0; i < names.Length; i++) {
                if(input.Contains(names[i])) {
                    mask += (1 << i);
                }
            }

            mask = EditorGUILayout.MaskField(title, mask, names);

            var selected = new List<string>();
            for(int i = 0; i < names.Length; i++) {
                var index = 1 << i;
                if((index & mask) == index) {
                    selected.Add(names[i]);
                }
            }

            return selected.ToArray();
        }
    }
}
