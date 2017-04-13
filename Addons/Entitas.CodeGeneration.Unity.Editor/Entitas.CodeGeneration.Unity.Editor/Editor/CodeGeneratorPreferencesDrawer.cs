using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Unity.Editor;
using Entitas.Utils;
using UnityEditor;
using UnityEngine;

namespace Entitas.CodeGeneration.Unity.Editor {

    public class CodeGeneratorPreferencesDrawer : AbstractPreferencesDrawer {

        public override int priority { get { return 10; } }
        public override string title { get { return "Code Generator"; } }

        string[] _availableDataProviderTypes;
        string[] _availableGeneratorTypes;
        string[] _availablePostProcessorTypes;

        string[] _availableDataProviderNames;
        string[] _availableGeneratorNames;
        string[] _availablePostProcessorNames;

        Properties _properties;
        Type[] _types;

        CodeGeneratorConfig _config;
        Exception _configException;

        public override void Initialize(Properties properties) {
            _properties = properties;
            try {
                _types = CodeGeneratorUtil.LoadTypesFromPlugins();
            } catch(Exception ex) {
                _configException = ex;
            }

            if(_configException == null) {
                initPhase<ICodeGeneratorDataProvider>(_types, out _availableDataProviderTypes, out _availableDataProviderNames);
                initPhase<ICodeGenerator>(_types, out _availableGeneratorTypes, out _availableGeneratorNames);
                initPhase<ICodeGenFilePostProcessor>(_types, out _availablePostProcessorTypes, out _availablePostProcessorNames);

                _config = new CodeGeneratorConfig();
                _config.Configure(properties);
            }
        }

        protected override void drawContent(Properties properties) {
            if(_configException == null) {
                _config.dataProviders = drawMaskField("Data Providers", _availableDataProviderTypes, _availableDataProviderNames, _config.dataProviders);
                _config.codeGenerators = drawMaskField("Code Generators", _availableGeneratorTypes, _availableGeneratorNames, _config.codeGenerators);
                _config.postProcessors = drawMaskField("Post Processors", _availablePostProcessorTypes, _availablePostProcessorNames, _config.postProcessors);

                EditorGUILayout.Space();
                drawConfigurables();

                drawGenerateButton();
            } else {
                var style = new GUIStyle(GUI.skin.label);
                style.wordWrap = true;
                EditorGUILayout.LabelField(_configException.Message, style);
            }
        }

        void drawConfigurables() {
            var configurableKeyValuePairs = CodeGeneratorUtil.GetConfigurableKeyValuePairs(
                CodeGeneratorUtil.GetUsed<ICodeGeneratorDataProvider>(_types, _config.dataProviders),
                CodeGeneratorUtil.GetUsed<ICodeGenerator>(_types, _config.codeGenerators),
                CodeGeneratorUtil.GetUsed<ICodeGenFilePostProcessor>(_types, _config.postProcessors)
            );

            foreach(var kv in configurableKeyValuePairs) {
                if(!_properties.HasKey(kv.Key)) {
                    _properties[kv.Key] = kv.Value;
                }
                _properties[kv.Key] = EditorGUILayout.TextField(kv.Key.ShortTypeName(), _properties[kv.Key]);
            }
        }

        static string[] initPhase<T>(Type[] types, out string[] availableTypes, out string[] availableNames) where T : ICodeGeneratorInterface {
            IEnumerable<T> instances = CodeGeneratorUtil.GetOrderedInstances<T>(types);

            availableTypes = instances
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();

            availableNames = instances
                .Select(instance => instance.name)
                .ToArray();

            return instances
                .Where(instance => instance.isEnabledByDefault)
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();
        }

        static string[] drawMaskField(string title, string[] types, string[] names, string[] input) {
            var mask = 0;

            for(int i = 0; i < types.Length; i++) {
                if(input.Contains(types[i])) {
                    mask += (1 << i);
                }
            }

            mask = EditorGUILayout.MaskField(title, mask, names);

            var selected = new List<string>();
            for(int i = 0; i < types.Length; i++) {
                var index = 1 << i;
                if((index & mask) == index) {
                    selected.Add(types[i]);
                }
            }

            return selected.ToArray();
        }

        void drawGenerateButton() {
            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if(GUILayout.Button("Generate", GUILayout.Height(32))) {
                UnityCodeGenerator.Generate();
            }
            GUI.backgroundColor = bgColor;
        }
    }
}
