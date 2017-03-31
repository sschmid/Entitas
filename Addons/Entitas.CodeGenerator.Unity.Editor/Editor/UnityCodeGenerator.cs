using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.CodeGenerator.Unity.Editor {

    public static class UnityCodeGenerator {

        [MenuItem("Entitas/Generate #%g", false, 100)]
        public static void Generate() {
            checkCanGenerate();

            Debug.Log("Generating...");


            // TODO Try catch EditorUtility.ClearProgressBar()

            var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromConfig(Preferences.configPath);

            var progressOffset = 0f;

            codeGenerator.OnProgress += (title, info, progress) => {
                var cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progressOffset + progress / 2);
                if(cancel) {
                    codeGenerator.Cancel();
                }
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

        static void checkCanGenerate() {
            if(EditorApplication.isCompiling) {
                throw new Exception("Cannot generate because Unity is still compiling. Please wait...");
            }

            var assembly = Assembly.GetAssembly(typeof(UnityEditor.Editor));
            var logEntries = assembly.GetType("UnityEditorInternal.LogEntries");
            logEntries.GetMethod("Clear").Invoke(new object(), null);
            var canCompile = (int)logEntries.GetMethod("GetCount").Invoke(new object(), null) == 0;
            if(!canCompile) {
                Debug.Log("There are compile errors! Generated code will be based on last compiled executable.");
            }
        }
    }
}
