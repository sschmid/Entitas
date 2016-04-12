using System;
using System.Linq;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using Entitas.Unity.Serialization.Blueprints;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.CodeGenerator {
    public static class UnityCodeGenerator {

        [MenuItem("Entitas/Generate #%g", false, 100)]
        public static void Generate() {
            checkCanGenerate();

            Debug.Log("Generating...");

            var codeGenerators = GetCodeGenerators();
            var codeGeneratorNames = codeGenerators.Select(cg => cg.Name).ToArray();
            var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig(), codeGeneratorNames);

            var enabledCodeGeneratorNames = config.enabledCodeGenerators;
            var enabledCodeGenerators = codeGenerators
                .Where(type => enabledCodeGeneratorNames.Contains(type.Name))
                .Select(type => (ICodeGenerator)Activator.CreateInstance(type))
                .ToArray();

            var blueprintNames = Resources.FindObjectsOfTypeAll<BinaryBlueprint>()
                .Select(b => b.Deserialize().name)
                .ToArray();

            var assembly = Assembly.GetAssembly(typeof(Entity));
            var generatedFiles = TypeReflectionCodeGenerator.Generate(assembly, config.pools,
                blueprintNames, config.generatedFolderPath, enabledCodeGenerators);

            foreach (var file in generatedFiles) {
                Debug.Log(file.generatorName + ": " + file.fileName);
            }

            var totalGeneratedFiles = generatedFiles.Select(file => file.fileName).Distinct().Count();
            Debug.Log("Generated " + totalGeneratedFiles + " files.");

            AssetDatabase.Refresh();
        }

        public static Type[] GetCodeGenerators() {
            return Assembly.GetAssembly(typeof(ICodeGenerator)).GetTypes()
                .Where(type => type.ImplementsInterface<ICodeGenerator>())
                .Where(type => type.GetCustomAttributes(typeof(ObsoleteAttribute), false).Length == 0)
                .OrderBy(type => type.FullName)
                .ToArray();
        }

        static void checkCanGenerate() {
            if (EditorApplication.isCompiling) {
                throw new Exception("Cannot generate because Unity is still compiling. Please wait...");
            }

            var assembly = Assembly.GetAssembly(typeof(Editor));
            var logEntries = assembly.GetType("UnityEditorInternal.LogEntries");
            logEntries.GetMethod("Clear").Invoke(new object(), null);
            var canCompile = (int)logEntries.GetMethod("GetCount").Invoke(new object(), null) == 0;
            if (!canCompile) {
                Debug.Log("There are compile errors! Generated code will be based on last compiled executable.");
            }
        }
    }
}
