using System;
using System.Linq;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;
using Entitas.CodeGenerator.TypeReflection;

namespace Entitas.Unity.CodeGenerator {
    public static class UnityCodeGenerator {

        [MenuItem("Entitas/Generate", false, 100)]
        public static void Generate() {
            assertCanGenerate();

            var codeGenerators = GetCodeGenerators();
            var codeGeneratorNames = codeGenerators.Select(cg => cg.Name).ToArray();
            var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig(), codeGeneratorNames);

            var enabledCodeGeneratorNames = config.enabledCodeGenerators;
            var enabledCodeGenerators = codeGenerators
                .Where(type => enabledCodeGeneratorNames.Contains(type.Name))
                .Select(type => (ICodeGenerator)Activator.CreateInstance(type))
                .ToArray();

            var assembly = Assembly.GetAssembly(typeof(Entity));
            TypeReflectionCodeGenerator.Generate(assembly, config.generatedFolderPath, enabledCodeGenerators);

            AssetDatabase.Refresh();
        }

        public static Type[] GetCodeGenerators() {
            return Assembly.GetAssembly(typeof(ICodeGenerator)).GetTypes()
                .Where(type => type.GetInterfaces().Contains(typeof(ICodeGenerator))
                    && type != typeof(ICodeGenerator)
                    && type != typeof(IPoolCodeGenerator)
                    && type != typeof(IComponentCodeGenerator))
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
