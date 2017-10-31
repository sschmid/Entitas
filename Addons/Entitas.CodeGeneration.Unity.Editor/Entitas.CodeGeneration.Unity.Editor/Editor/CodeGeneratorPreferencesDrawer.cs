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

            initPhase<ICodeGeneratorDataProvider>(_types, out _availableDataProviderTypes, out _availableDataProviderNames);
            initPhase<ICodeGenerator>(_types, out _availableGeneratorTypes, out _availableGeneratorNames);
            initPhase<ICodeGenFilePostProcessor>(_types, out _availablePostProcessorTypes, out _availablePostProcessorNames);

            _preferences.AddProperties(getConfigurables(), false);
        }

        protected override void drawContent(Preferences preferences) {
            _codeGeneratorConfig.dataProviders = drawMaskField("Data Providers", _availableDataProviderTypes, _availableDataProviderNames, _codeGeneratorConfig.dataProviders);
            _codeGeneratorConfig.codeGenerators = drawMaskField("Code Generators", _availableGeneratorTypes, _availableGeneratorNames, _codeGeneratorConfig.codeGenerators);
            _codeGeneratorConfig.postProcessors = drawMaskField("Post Processors", _availablePostProcessorTypes, _availablePostProcessorNames, _codeGeneratorConfig.postProcessors);

            EditorGUILayout.Space();
            drawConfigurables();

            drawGenerateButton();
        }

        Dictionary<string, string> getConfigurables() {
            return CodeGeneratorUtil.GetConfigurables(
                CodeGeneratorUtil.GetUsed<ICodeGeneratorDataProvider>(_types, _codeGeneratorConfig.dataProviders),
                CodeGeneratorUtil.GetUsed<ICodeGenerator>(_types, _codeGeneratorConfig.codeGenerators),
                CodeGeneratorUtil.GetUsed<ICodeGenFilePostProcessor>(_types, _codeGeneratorConfig.postProcessors)
            );
        }

        void drawConfigurables() {
            var configurables = getConfigurables();
            _preferences.AddProperties(configurables, false);

            foreach (var kv in configurables.OrderBy(kv => kv.Key)) {
                _preferences[kv.Key] = EditorGUILayout.TextField(kv.Key.ShortTypeName().ToSpacedCamelCase(), _preferences[kv.Key]);
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
