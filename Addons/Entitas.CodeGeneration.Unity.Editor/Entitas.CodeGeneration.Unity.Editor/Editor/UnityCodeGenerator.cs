using System;
using System.IO;
using System.Linq;
using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;
using UnityEditor;
using UnityEngine;

namespace Entitas.CodeGeneration.Unity.Editor {

    public static class UnityCodeGenerator {

        [MenuItem("Tools/Entitas/Generate #%g", false, 100)]
        public static void Generate() {
            checkCanGenerate();

            Debug.Log("Generating...");

            Preferences.sharedInstance.Refresh();
            var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromPreferences(Preferences.sharedInstance);

            var progressOffset = 0f;

            codeGenerator.OnProgress += (title, info, progress) => {
                var cancel = EditorUtility.DisplayCancelableProgressBar(title, info, progressOffset + progress / 2);
                if (cancel) {
                    codeGenerator.Cancel();
                }
            };

            CodeGenFile[] dryFiles = null;
            CodeGenFile[] files = null;

            try {
                dryFiles = codeGenerator.DryRun();
                progressOffset = 0.5f;
                files = codeGenerator.Generate();
            } catch(Exception ex) {
                dryFiles = new CodeGenFile[0];
                files = new CodeGenFile[0];

                EditorUtility.DisplayDialog("Error", ex.Message, "Ok");
            }

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
            if (EditorApplication.isCompiling) {
                throw new Exception("Cannot generate because Unity is still compiling. Please wait...");
            }

            var assembly = typeof(UnityEditor.Editor).Assembly;

            var logEntries = assembly.GetType("UnityEditorInternal.LogEntries")
                          ?? assembly.GetType("UnityEditor.LogEntries");

            logEntries.GetMethod("Clear").Invoke(new object(), null);
            var canCompile = (int)logEntries.GetMethod("GetCount").Invoke(new object(), null) == 0;
            if (!canCompile) {
                Debug.Log("There are compile errors! Generated code will be based on last compiled executable.");
            }
        }

        [MenuItem("Tools/Entitas/Generate with CLI %&g", false, 101)]
        public static void GenerateWithCLI() {
            Debug.Log("Generating...");

            Preferences.sharedInstance.Refresh();
            var config = new CodeGeneratorConfig();
            config.Configure(Preferences.sharedInstance);

            var projectRoot = Application.dataPath.Substring(0, Application.dataPath.Length - "/Assets".Length);
            var cli = Path.Combine(projectRoot, config.cli);
            if (!File.Exists(cli)) {
                Debug.Log(cli + " does not exist!");
                return;
            }

            if (Application.platform == RuntimePlatform.WindowsEditor) {
                runCommand(cli, "gen", projectRoot);
            } else {
                runCommand(config.mono, cli + " gen", projectRoot);
            }

            Debug.Log("Generating done.");

            AssetDatabase.Refresh();
        }

        static void runCommand(string fileName, string arguments, string workingDirectory) {
            var startInfo = createStartInfo(workingDirectory);
            startInfo.FileName = fileName;
            startInfo.Arguments = arguments;
            startProcess(startInfo);
        }

        static System.Diagnostics.ProcessStartInfo createStartInfo(string workingDirectory) {
            var startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WorkingDirectory = workingDirectory;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            return startInfo;
        }

        static void startProcess(System.Diagnostics.ProcessStartInfo startInfo) {
            var process = new System.Diagnostics.Process();
            process.StartInfo = startInfo;
            process.OutputDataReceived += OnDataReceivedEventHandler;
            process.ErrorDataReceived += OnErrorReceivedEventHandler;
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            process.WaitForExit();
            process.Close();
        }

        static void OnDataReceivedEventHandler(object sender, System.Diagnostics.DataReceivedEventArgs e) {
            if (e.Data != null) {
                UnityEngine.Debug.Log(e.Data);
            }
        }

        static void OnErrorReceivedEventHandler(object sender, System.Diagnostics.DataReceivedEventArgs e) {
            if (e.Data != null) {
                UnityEngine.Debug.Log(e.Data);
            }
        }
    }
}
