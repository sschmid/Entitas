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

            var config = new CodeGeneratorConfig(EntitasPreferences.LoadConfig());

            var codeGenerator = new Entitas.CodeGenerator.CodeGenerator(
                getEnabled<ICodeGeneratorDataProvider>(config.dataProviders),
                getEnabled<ICodeGenerator>(config.codeGenerators),
                getEnabled<ICodeGenFilePostProcessor>(config.postProcessors)
            );

            var progressOffset = 0f;

            codeGenerator.OnProgress += (title, info, progress) => {
                EditorUtility.DisplayProgressBar(title, info, progressOffset + progress / 2);
            };

            var dryFiles = codeGenerator.DryRun();
            progressOffset = 0.5f;
            var files = codeGenerator.Generate();
            EditorUtility.ClearProgressBar();

            var totalGeneratedFiles = files.Select(file => file.fileName).Distinct().Count();

            var sloc = dryFiles
                .Select(file => file.fileContent.ToUnixLineEndings())
                .Sum(content => content.Split(new [] { '\n' }, StringSplitOptions.RemoveEmptyEntries).Length);

            var loc = files
                .Select(file => file.fileContent.ToUnixLineEndings())
                .Sum(content => content.Split(new [] { '\n' }).Length);

            Debug.Log("Generated " + totalGeneratedFiles + " files (" + sloc + " sloc, " + loc + " loc)");

            AssetDatabase.Refresh();
        }

        static T[] getEnabled<T>(string[] types) {
            return GetTypes<T>()
                    .Where(type => types.Contains(type.FullName))
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
