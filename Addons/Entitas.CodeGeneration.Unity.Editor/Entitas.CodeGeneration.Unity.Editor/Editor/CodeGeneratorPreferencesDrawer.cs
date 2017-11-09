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

        Preferences _preferences;
        Type[] _types;

        CodeGeneratorConfig _codeGeneratorConfig;

        public override void Initialize(Preferences preferences) {
            _preferences = preferences;
            _codeGeneratorConfig = new CodeGeneratorConfig();
            preferences.AddProperties(_codeGeneratorConfig.defaultProperties, false);
            _codeGeneratorConfig.Configure(preferences);

            _types = CodeGeneratorUtil.LoadTypesFromPlugins(preferences);

            setTypesAndNames<ICodeGeneratorDataProvider>(_types, out _availableDataProviderTypes, out _availableDataProviderNames);
            setTypesAndNames<ICodeGenerator>(_types, out _availableGeneratorTypes, out _availableGeneratorNames);
            setTypesAndNames<ICodeGenFilePostProcessor>(_types, out _availablePostProcessorTypes, out _availablePostProcessorNames);

            _preferences.AddProperties(CodeGeneratorUtil.GetDefaultProperties(_types, _codeGeneratorConfig), false);
        }

        protected override void drawContent(Preferences preferences) {
            _codeGeneratorConfig.dataProviders = drawMaskField("Data Providers", _availableDataProviderTypes, _availableDataProviderNames, _codeGeneratorConfig.dataProviders);
            _codeGeneratorConfig.codeGenerators = drawMaskField("Code Generators", _availableGeneratorTypes, _availableGeneratorNames, _codeGeneratorConfig.codeGenerators);
            _codeGeneratorConfig.postProcessors = drawMaskField("Post Processors", _availablePostProcessorTypes, _availablePostProcessorNames, _codeGeneratorConfig.postProcessors);

            EditorGUILayout.Space();
            drawConfigurables();

            drawGenerateButton();
        }

        void drawConfigurables() {
            var defaultProperties = CodeGeneratorUtil.GetDefaultProperties(_types, _codeGeneratorConfig);
            _preferences.AddProperties(defaultProperties, false);

            foreach (var kv in defaultProperties.OrderBy(kv => kv.Key)) {
                _preferences[kv.Key] = EditorGUILayout.TextField(kv.Key.ShortTypeName().ToSpacedCamelCase(), _preferences[kv.Key]);
            }
        }

        static void setTypesAndNames<T>(Type[] types, out string[] availableTypes, out string[] availableNames) where T : ICodeGeneratorInterface {
            IEnumerable<T> instances = CodeGeneratorUtil.GetOrderedInstancesOf<T>(types);

            availableTypes = instances
                .Select(instance => instance.GetType().ToCompilableString())
                .ToArray();

            availableNames = instances
                .Select(instance => instance.name)
                .ToArray();
        }

        static string[] drawMaskField(string title, string[] types, string[] names, string[] input) {
            var mask = 0;

            for (int i = 0; i < types.Length; i++) {
                if (input.Contains(types[i])) {
                    mask += (1 << i);
                }
            }

            mask = EditorGUILayout.MaskField(title, mask, names);

            var selected = new List<string>();
            for (int i = 0; i < types.Length; i++) {
                var index = 1 << i;
                if ((index & mask) == index) {
                    selected.Add(types[i]);
                }
            }

            return selected.ToArray();
        }

        void drawGenerateButton() {
            var bgColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.green;
            if (GUILayout.Button("Generate", GUILayout.Height(32))) {
                UnityCodeGenerator.Generate();
            }
            GUI.backgroundColor = bgColor;
        }
    }
}
