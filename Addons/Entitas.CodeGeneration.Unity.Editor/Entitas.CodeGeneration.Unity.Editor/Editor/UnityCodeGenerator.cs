using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Entitas.CodeGeneration.CodeGenerator;
using Entitas.Utils;
using Fabl.Appenders;
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

        [MenuItem("Tools/Entitas/Generate with external Code Generator %&g", false, 101)]
        public static void GenerateExternal() {
            Debug.Log("Connecting...");

            var client = new TcpClientSocket();
            client.OnConnect += onConnected;
            client.OnReceive += onReceive;
            client.OnDisconnect += onDisconnect;
            client.Connect(IPAddress.Parse("127.0.0.1"), 3333);

            AssetDatabase.Refresh();
        }

        static void onConnected(TcpClientSocket client) {
            Debug.Log("Connected.");
            Debug.Log("Generating...");
            client.Send(Encoding.UTF8.GetBytes("gen"));
        }

        static void onReceive(AbstractTcpSocket socket, Socket client, byte[] bytes) {
            Debug.Log("Generated.");
            socket.Disconnect();
        }

        static void onDisconnect(AbstractTcpSocket socket) {
            Debug.Log("Disconnected");
        }
    }
}
