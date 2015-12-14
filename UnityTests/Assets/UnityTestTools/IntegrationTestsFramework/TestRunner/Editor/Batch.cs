using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using UnityTest.IntegrationTests;

namespace UnityTest
{
    public static partial class Batch
    {
        private const string k_TestScenesParam = "-testscenes=";
        private const string k_OtherBuildScenesParam = "-includeBuildScenes=";
        const string k_TargetPlatformParam = "-targetPlatform=";
        const string k_ResultFileDirParam = "-resultsFileDirectory=";

        public static void RunIntegrationTests()
        {
            var targetPlatform = GetTargetPlatform();
            var otherBuildScenes = GetSceneListFromParam (k_OtherBuildScenesParam);

            var testScenes = GetSceneListFromParam(k_TestScenesParam);
            if (testScenes.Count == 0)
                testScenes = FindTestScenesInProject();

            RunIntegrationTests(targetPlatform, testScenes, otherBuildScenes);
        }
        
        public static void RunIntegrationTests(BuildTarget ? targetPlatform)
        {
            var sceneList = FindTestScenesInProject();
            RunIntegrationTests(targetPlatform, sceneList, new List<string>());
        }


        public static void RunIntegrationTests(BuildTarget? targetPlatform, List<string> testScenes, List<string> otherBuildScenes)
        {
            if (targetPlatform.HasValue)
                BuildAndRun(targetPlatform.Value, testScenes, otherBuildScenes);
            else
                RunInEditor(testScenes,  otherBuildScenes);
        }
        
        private static void BuildAndRun(BuildTarget target, List<string> testScenes, List<string> otherBuildScenes)
        {
            var resultFilePath = GetParameterArgument(k_ResultFileDirParam);

            const int port = 0;
            var ipList = TestRunnerConfigurator.GetAvailableNetworkIPs();

            var config = new PlatformRunnerConfiguration
            {
                buildTarget = target,
                buildScenes = otherBuildScenes,
                testScenes = testScenes,
                projectName = "IntegrationTests",
                resultsDir = resultFilePath,
                sendResultsOverNetwork = InternalEditorUtility.inBatchMode,
                ipList = ipList,
                port = port
            };

            if (Application.isWebPlayer)
            {
                config.sendResultsOverNetwork = false;
                Debug.Log("You can't use WebPlayer as active platform for running integration tests. Switching to Standalone");
                EditorUserBuildSettings.SwitchActiveBuildTarget(BuildTarget.StandaloneWindows);
            }

            PlatformRunner.BuildAndRunInPlayer(config);
        }

        private static void RunInEditor(List<string> testScenes, List<string> otherBuildScenes)
        {
            CheckActiveBuildTarget();

            NetworkResultsReceiver.StopReceiver();
            if (testScenes == null || testScenes.Count == 0)
            {
                Debug.Log("No test scenes on the list");
                EditorApplication.Exit(returnCodeRunError);
                return;
            }
            EditorBuildSettings.scenes = (testScenes.Concat(otherBuildScenes).ToList()).Select(s => new EditorBuildSettingsScene(s, true)).ToArray();
            EditorApplication.OpenScene(testScenes.First());
            GuiHelper.SetConsoleErrorPause(false);

            var config = new PlatformRunnerConfiguration
            {
                resultsDir = GetParameterArgument(k_ResultFileDirParam),
                ipList = TestRunnerConfigurator.GetAvailableNetworkIPs(),
                port = PlatformRunnerConfiguration.TryToGetFreePort(),
                runInEditor = true
            };

            var settings = new PlayerSettingConfigurator(true);
            settings.AddConfigurationFile(TestRunnerConfigurator.integrationTestsNetwork, string.Join("\n", config.GetConnectionIPs()));

            NetworkResultsReceiver.StartReceiver(config);

            EditorApplication.isPlaying = true;
        }

        static void CheckActiveBuildTarget()
        {
            var notSupportedPlatforms = new[] { "MetroPlayer", "WebPlayer", "WebPlayerStreamed" };
            if (notSupportedPlatforms.Contains(EditorUserBuildSettings.activeBuildTarget.ToString()))
            {
                Debug.Log("activeBuildTarget can not be  "
                    + EditorUserBuildSettings.activeBuildTarget + 
                    " use buildTarget parameter to open Unity.");
            }
        }

        private static BuildTarget ? GetTargetPlatform()
        {
            string platformString = null;
            BuildTarget buildTarget;
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (arg.ToLower().StartsWith(k_TargetPlatformParam.ToLower()))
                {
                    platformString = arg.Substring(k_ResultFilePathParam.Length);
                    break;
                }
            }
            try
            {
                if (platformString == null) return null;
                buildTarget = (BuildTarget)Enum.Parse(typeof(BuildTarget), platformString);
            }
            catch
            {
                return null;
            }
            return buildTarget;
        }

        private static List<string> FindTestScenesInProject()
        {
            var integrationTestScenePattern = "*Test?.unity";
            return Directory.GetFiles("Assets", integrationTestScenePattern, SearchOption.AllDirectories).ToList();
        }

        private static List<string> GetSceneListFromParam(string param)
        {
            var sceneList = new List<string>();
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (arg.ToLower().StartsWith(param.ToLower()))
                {
                    var scenesFromParam = arg.Substring(param.Length).Split(',');
                    foreach (var scene in scenesFromParam)
                    {
                        var sceneName = scene;
                        if (!sceneName.EndsWith(".unity"))
                            sceneName += ".unity";
                        var foundScenes = Directory.GetFiles(Directory.GetCurrentDirectory(), sceneName, SearchOption.AllDirectories);
                        if (foundScenes.Length == 1)
                            sceneList.Add(foundScenes[0].Substring(Directory.GetCurrentDirectory().Length + 1));
                        else
                            Debug.Log(sceneName + " not found or multiple entries found");
                    }
                }
            }
            return sceneList.Where(s => !string.IsNullOrEmpty(s)).Distinct().ToList();
        }
    }
}
