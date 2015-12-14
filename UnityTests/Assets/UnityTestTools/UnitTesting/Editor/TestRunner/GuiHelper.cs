using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Mdb;
using Mono.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace UnityTest
{
    public static class GuiHelper
    {
        public static Texture GetIconForResult(TestResultState resultState)
        {
            switch (resultState)
            {
                case TestResultState.Success:
                    return Icons.SuccessImg;
                case TestResultState.Failure:
                case TestResultState.Error:
                    return Icons.FailImg;
                case TestResultState.Ignored:
                case TestResultState.Skipped:
                    return Icons.IgnoreImg;
                case TestResultState.Inconclusive:
                case TestResultState.Cancelled:
                case TestResultState.NotRunnable:
                    return Icons.InconclusiveImg;
                default:
                    return Icons.UnknownImg;
            }
        }

        private static int ExtractSourceFileLine(string stackTrace)
        {
            int line = 0;
            if (!string.IsNullOrEmpty(stackTrace))
            {
                var regEx = new Regex(@".* in (?'path'.*):(?'line'\d+)");
                var matches = regEx.Matches(stackTrace);
                for (int i = 0; i < matches.Count; i++)
                {
                    line = int.Parse(matches[i].Groups["line"].Value);
                    if (line != 0)
                        break;
                }
            }
            return line;
        }

        private static string ExtractSourceFilePath(string stackTrace)
        {
            string path = "";
            if (!string.IsNullOrEmpty(stackTrace))
            {
                var regEx = new Regex(@".* in (?'path'.*):(?'line'\d+)");
                var matches = regEx.Matches(stackTrace);
                for (int i = 0; i < matches.Count; i++)
                {
                    path = matches[i].Groups["path"].Value;
                    if (path != "<filename unknown>")
                        break;
                }
            }
            return path;
        }

        public static void OpenInEditor(UnitTestResult test, bool openError)
        {
            var sourceFilePath = ExtractSourceFilePath(test.StackTrace);
            var sourceFileLine = ExtractSourceFileLine(test.StackTrace);

            if (!openError || sourceFileLine == 0 || string.IsNullOrEmpty(sourceFilePath))
            {
                var sp = GetSequencePointOfTest(test);
                if (sp != null)
                {
                    sourceFileLine = sp.StartLine;
                    sourceFilePath = sp.Document.Url;
                }
            }

            OpenInEditorInternal(sourceFilePath, sourceFileLine);
        }

        private static SequencePoint GetSequencePointOfTest(UnitTestResult test)
        {
            var readerParameters = new ReaderParameters
            {
                ReadSymbols = true,
                SymbolReaderProvider = new MdbReaderProvider(),
                ReadingMode = ReadingMode.Immediate
            };

            var assemblyDefinition = AssemblyDefinition.ReadAssembly(test.Test.AssemblyPath, readerParameters);
            var classModule = assemblyDefinition.MainModule.Types.Single(t => t.FullName == test.Test.FullClassName);

            Collection<MethodDefinition> methods;
            MethodDefinition method = null;
            while (classModule.BaseType != null)
            {
                methods = classModule.Methods;
                if (methods.Any(t => t.Name == test.Test.MethodName))
                {
                    method = classModule.Methods.First(t => t.Name == test.Test.MethodName);
                    break;
                }
                classModule = classModule.BaseType as TypeDefinition;
            }
            if (method != null)
            {
                var sp = method.Body.Instructions.First(i => i.SequencePoint != null).SequencePoint;
                return sp;
            }
            return null;
        }

        private static void OpenInEditorInternal(string filename, int line)
        {
			string assetPath = filename.Substring(Application.dataPath.Length - "Assets/".Length + 1);
			var scriptAsset = AssetDatabase.LoadMainAssetAtPath(assetPath);
			AssetDatabase.OpenAsset(scriptAsset, line);
        }

        public static bool GetConsoleErrorPause()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
            Type type = assembly.GetType("UnityEditorInternal.LogEntries");
            PropertyInfo method = type.GetProperty("consoleFlags");
            var result = (int)method.GetValue(new object(), new object[] { });
            return (result & (1 << 2)) != 0;
        }

        public static void SetConsoleErrorPause(bool b)
        {
            Assembly assembly = Assembly.GetAssembly(typeof(SceneView));
            Type type = assembly.GetType("UnityEditorInternal.LogEntries");
            MethodInfo method = type.GetMethod("SetConsoleFlag");
            method.Invoke(new object(), new object[] { 1 << 2, b });
        }
    }
}
