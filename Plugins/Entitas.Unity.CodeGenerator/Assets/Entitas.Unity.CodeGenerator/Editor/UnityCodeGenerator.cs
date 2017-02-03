using System;
using System.Linq;
using System.Reflection;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.CodeGenerator {

    public static class UnityCodeGenerator {

        [MenuItem("Entitas/Generate #%g", false, 100)]
        public static void Generate() {
            checkCanGenerate();

            Debug.Log("Generating...");

			var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig(), new string[0], new string[0], new string[0]);

            var files = new Entitas.CodeGenerator.CodeGenerator(
                getEnabled<ICodeGeneratorDataProvider>(config.dataProviders),
                getEnabled<ICodeGenerator>(config.codeGenerators),
                getEnabled<ICodeGenFilePostProcessor>(config.postProcessors)
            ).Generate();

            foreach(var file in files) {
                Debug.Log(file.generatorName + ": " + file.fileName);
            }

            var totalGeneratedFiles = files.Select(file => file.fileName).Distinct().Count();
            Debug.Log("Generated " + totalGeneratedFiles + " files.");

            AssetDatabase.Refresh();
        }

        static T[] getEnabled<T>(string[] names) {
            return GetTypes<T>()
                    .Where(type => names.Contains(type.Name))
                    .Select(type => (T)Activator.CreateInstance(type))
                    .ToArray();
        }

        public static Type[] GetTypes<T>() {
            return Assembly.GetAssembly(typeof(T)).GetTypes()
                .Where(type => type.ImplementsInterface<T>())
                .OrderBy(type => type.FullName)
                .ToArray();
        }

        static void checkCanGenerate() {
            if(EditorApplication.isCompiling) {
                throw new Exception("Cannot generate because Unity is still compiling. Please wait...");
            }

            var assembly = Assembly.GetAssembly(typeof(Editor));
            var logEntries = assembly.GetType("UnityEditorInternal.LogEntries");
            logEntries.GetMethod("Clear").Invoke(new object(), null);
            var canCompile = (int)logEntries.GetMethod("GetCount").Invoke(new object(), null) == 0;
            if(!canCompile) {
                Debug.Log("There are compile errors! Generated code will be based on last compiled executable.");
            }
        }
    }
}
