using System;
using System.Linq;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using Entitas.CodeGenerator.TypeReflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.CodeGenerator {
    public static class UnityCodeGenerator {

        [MenuItem("Entitas/Generate #%g", false, 100)]
        public static void Generate() {
            assertCanGenerate();

            Debug.Log("Generating...");

            var codeGenerators = GetCodeGenerators();
            var codeGeneratorNames = codeGenerators.Select(cg => cg.Name).ToArray();
            var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig(), codeGeneratorNames);

            var enabledCodeGeneratorNames = config.enabledCodeGenerators;
            var enabledCodeGenerators = codeGenerators
                .Where(type => enabledCodeGeneratorNames.Contains(type.Name))
                .Select(type => (ICodeGenerator)Activator.CreateInstance(type))
                .ToArray();

            var assembly = Assembly.GetAssembly(typeof(Entity));
            var totalFilesGenerated = TypeReflectionCodeGenerator.Generate(assembly, config.pools, config.generatedFolderPath, enabledCodeGenerators);

            AssetDatabase.Refresh();

            Debug.Log("Generated " + totalFilesGenerated + " files.");
        }

        public static Type[] GetCodeGenerators() {
            return Assembly.GetAssembly(typeof(ICodeGenerator)).GetTypes()
                .Where(type => type.ImplementsInterface<ICodeGenerator>())
                .OrderBy(type => type.FullName)
                .ToArray();
        }

        static void assertCanGenerate() {
            if (EditorApplication.isCompiling) {
                throw new Exception("Cannot generate because Unity is still compiling. Please wait...");
            }

            var assembly = Assembly.GetAssembly(typeof(Editor));
            var logEntries = assembly.GetType("UnityEditorInternal.LogEntries");
            logEntries.GetMethod("Clear").Invoke(new object(), null);
            var canCompile = (int)logEntries.GetMethod("GetCount").Invoke(new object(), null) == 0;
            if (!canCompile) {
                throw new Exception("Cannot generate because there are compile errors!");
            }
        }
    }
}
