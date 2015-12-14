using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityTest.IntegrationTests
{
    [Serializable]
    public class PlatformRunnerSettingsWindow : EditorWindow
    {
        private BuildTarget m_BuildTarget;

        private List<string> m_IntegrationTestScenes;
        private List<string> m_OtherScenesToBuild;
        private List<string> m_AllScenesInProject;

        private Vector2 m_ScrollPosition;
        private readonly List<string> m_Interfaces = new List<string>();
        private readonly List<string> m_SelectedScenes = new List<string>();

        private int m_SelectedInterface;
        [SerializeField]
        private bool m_AdvancedNetworkingSettings;

        private PlatformRunnerSettings m_Settings;

        private string m_SelectedSceneInAll;
        private string m_SelectedSceneInTest;
        private string m_SelectedSceneInBuild;

        readonly GUIContent m_Label = new GUIContent("Results target directory", "Directory where the results will be saved. If no value is specified, the results will be generated in project's data folder.");
        
        public PlatformRunnerSettingsWindow()
        {
            if (m_OtherScenesToBuild == null)
                m_OtherScenesToBuild = new List<string> ();

            if (m_IntegrationTestScenes == null)
                m_IntegrationTestScenes = new List<string> ();

            titleContent = new GUIContent("Platform runner");
            m_BuildTarget = PlatformRunner.defaultBuildTarget;
            position.Set(position.xMin, position.yMin, 200, position.height);
            m_AllScenesInProject = Directory.GetFiles(Directory.GetCurrentDirectory(), "*.unity", SearchOption.AllDirectories).ToList();
            m_AllScenesInProject.Sort();
            var currentScene = (Directory.GetCurrentDirectory() + EditorApplication.currentScene).Replace("\\", "").Replace("/", "");
            var currentScenePath = m_AllScenesInProject.Where(s => s.Replace("\\", "").Replace("/", "") == currentScene);
            m_SelectedScenes.AddRange(currentScenePath);

            m_Interfaces.Add("(Any)");
            m_Interfaces.AddRange(TestRunnerConfigurator.GetAvailableNetworkIPs());
            m_Interfaces.Add("127.0.0.1");

            LoadFromPrefereneces ();
        }

        public void OnEnable()
        {
            m_Settings = ProjectSettingsBase.Load<PlatformRunnerSettings>();

            // If not configured pre populate with all scenes that have test components on game objects
            // This needs to be done outsie of constructor
            if (m_IntegrationTestScenes.Count == 0)
                m_IntegrationTestScenes = GetScenesWithTestComponents (m_AllScenesInProject);
        }

        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();
                GUIContent label;

                /* We have three lists here, The tests to run, supporting scenes to include in the build and the list of all scenes so users can
                 * pick the scenes they want to include. The motiviation here is that test scenes may require to additively load other scenes as part of the tests
                 */
                EditorGUILayout.BeginHorizontal ();

                    // Integration Tests To Run
                    EditorGUILayout.BeginVertical ();

                    label = new GUIContent("Tests:", "All Integration Test scenes that you wish to run on the platform");
                    EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Height(20f));

                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_SelectedSceneInTest));
                        if (GUILayout.Button("Remove Integration Test")) {
                        m_IntegrationTestScenes.Remove(m_SelectedSceneInTest);
                        m_SelectedSceneInTest = "";
                    }
                    EditorGUI.EndDisabledGroup();

                    DrawVerticalSceneList (ref m_IntegrationTestScenes, ref m_SelectedSceneInTest);
                    EditorGUILayout.EndVertical ();
        
                    // Extra scenes to include in build
                    EditorGUILayout.BeginVertical ();
                        label = new GUIContent("Other Scenes in Build:", "If your Integration Tests additivly load any other scenes then you want to include them here so they are part of the build");
                        EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Height(20f));

            
                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_SelectedSceneInBuild));
                    if (GUILayout.Button("Remove From Build")) {
                        m_OtherScenesToBuild.Remove(m_SelectedSceneInBuild);
                        m_SelectedSceneInBuild = "";
                    }
                    EditorGUI.EndDisabledGroup();

                    DrawVerticalSceneList (ref m_OtherScenesToBuild, ref m_SelectedSceneInBuild);
                    EditorGUILayout.EndVertical ();

                    EditorGUILayout.Separator ();

                    // All Scenes
                    EditorGUILayout.BeginVertical ();
                    label = new GUIContent("Availble Scenes", "These are all the scenes within your project, please select some to run tests");
                    EditorGUILayout.LabelField(label, EditorStyles.boldLabel, GUILayout.Height(20f));

            
                    EditorGUILayout.BeginHorizontal ();
                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(m_SelectedSceneInAll));
                    if (GUILayout.Button("Add As Test")) {
                        if (!m_IntegrationTestScenes.Contains (m_SelectedSceneInAll) && !m_OtherScenesToBuild.Contains (m_SelectedSceneInAll)) {
                            m_IntegrationTestScenes.Add(m_SelectedSceneInAll);
                        }
                    }
            
                    if (GUILayout.Button("Add to Build")) {
                        if (!m_IntegrationTestScenes.Contains (m_SelectedSceneInAll) && !m_OtherScenesToBuild.Contains (m_SelectedSceneInAll)) {
                            m_OtherScenesToBuild.Add(m_SelectedSceneInAll);
                        }
                    }
                    EditorGUI.EndDisabledGroup();

                    EditorGUILayout.EndHorizontal ();

                    DrawVerticalSceneList (ref m_AllScenesInProject, ref m_SelectedSceneInAll);
                    EditorGUILayout.EndVertical ();
                    
            // ButtoNetworkResultsReceiverns to edit scenes in lists
                  

                EditorGUILayout.EndHorizontal ();
                
                GUILayout.Space(3);
                
                // Select target platform
                m_BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup("Build tests for", m_BuildTarget);

                if (PlatformRunner.defaultBuildTarget != m_BuildTarget)
                {
                    if (GUILayout.Button("Make default target platform"))
                    {
                    PlatformRunner.defaultBuildTarget = m_BuildTarget;
                    }
                }
                GUI.enabled = true;
            
                // Select various Network settings
                DrawSetting();
                var build = GUILayout.Button("Build and run tests");
            EditorGUILayout.EndVertical();

            if (build) 
            {
                BuildAndRun ();
            }
        }

        private void DrawVerticalSceneList(ref List<string> sourceList, ref string selectString)
        {
            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition, Styles.testList);
            EditorGUI.indentLevel++;
            foreach (var scenePath in sourceList)
            {
                var path = Path.GetFileNameWithoutExtension(scenePath);
                var guiContent = new GUIContent(path, scenePath);
                var rect = GUILayoutUtility.GetRect(guiContent, EditorStyles.label);
                if (rect.Contains(Event.current.mousePosition))
                {
                    if (Event.current.type == EventType.mouseDown && Event.current.button == 0)
                    {
                        selectString = scenePath;
                        Event.current.Use();
                    }
                }
                var style = new GUIStyle(EditorStyles.label);
 
                if (selectString == scenePath)
                    style.normal.textColor = new Color(0.3f, 0.5f, 0.85f);
                EditorGUI.LabelField(rect, guiContent, style);
            }
            EditorGUI.indentLevel--;
            EditorGUILayout.EndScrollView();
        }

        public static List<string> GetScenesWithTestComponents(List<string> allScenes)
        {
            List<Object> results = EditorReferencesUtil.FindScenesWhichContainAsset("TestComponent.cs");	
            List<string> integrationTestScenes = new List<string>();
            
            foreach (Object obj in results) {
                string result = allScenes.FirstOrDefault(s => s.Contains(obj.name));
                if (!string.IsNullOrEmpty(result))
                    integrationTestScenes.Add(result);
            }
            
            return integrationTestScenes;
        }

        private void DrawSetting()
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.BeginHorizontal();
            m_Settings.resultsPath = EditorGUILayout.TextField(m_Label, m_Settings.resultsPath);
            if (GUILayout.Button("Search", EditorStyles.miniButton, GUILayout.Width(50)))
            {
                var selectedPath = EditorUtility.SaveFolderPanel("Result files destination", m_Settings.resultsPath, "");
                if (!string.IsNullOrEmpty(selectedPath))
                    m_Settings.resultsPath = Path.GetFullPath(selectedPath);
            }
            EditorGUILayout.EndHorizontal();

            if (!string.IsNullOrEmpty(m_Settings.resultsPath))
            {
                Uri uri;
                if (!Uri.TryCreate(m_Settings.resultsPath, UriKind.Absolute, out uri) || !uri.IsFile || uri.IsWellFormedOriginalString())
                {
                    EditorGUILayout.HelpBox("Invalid URI path", MessageType.Warning);
                }
            }

            m_Settings.sendResultsOverNetwork = EditorGUILayout.Toggle("Send results to editor", m_Settings.sendResultsOverNetwork);
            EditorGUI.BeginDisabledGroup(!m_Settings.sendResultsOverNetwork);
            m_AdvancedNetworkingSettings = EditorGUILayout.Foldout(m_AdvancedNetworkingSettings, "Advanced network settings");
            if (m_AdvancedNetworkingSettings)
            {
                m_SelectedInterface = EditorGUILayout.Popup("Network interface", m_SelectedInterface, m_Interfaces.ToArray());
                EditorGUI.BeginChangeCheck();
                m_Settings.port = EditorGUILayout.IntField("Network port", m_Settings.port);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_Settings.port > IPEndPoint.MaxPort)
                        m_Settings.port = IPEndPoint.MaxPort;
                    else if (m_Settings.port < IPEndPoint.MinPort)
                        m_Settings.port = IPEndPoint.MinPort;
                }
                EditorGUI.EndDisabledGroup();
            }
            if (EditorGUI.EndChangeCheck())
            {
                m_Settings.Save();
            }
        }

        private void BuildAndRun()
        {
            SaveToPreferences ();

            var config = new PlatformRunnerConfiguration
            {
                buildTarget = m_BuildTarget,
                buildScenes = m_OtherScenesToBuild,
                testScenes = m_IntegrationTestScenes,
                projectName = m_IntegrationTestScenes.Count > 1 ? "IntegrationTests" : Path.GetFileNameWithoutExtension(EditorApplication.currentScene),
                resultsDir = m_Settings.resultsPath,
                sendResultsOverNetwork = m_Settings.sendResultsOverNetwork,
                ipList = m_Interfaces.Skip(1).ToList(),
                port = m_Settings.port
            };
            
            if (m_SelectedInterface > 0)
            config.ipList = new List<string> {m_Interfaces.ElementAt(m_SelectedInterface)};
            
            PlatformRunner.BuildAndRunInPlayer(config);
            Close ();
        }

        public void OnLostFocus() {
            SaveToPreferences ();
        }

        public void OnDestroy() {
            SaveToPreferences ();
        }

        private void SaveToPreferences()
        {
            EditorPrefs.SetString (Animator.StringToHash (Application.dataPath + "uttTestScenes").ToString (), String.Join (",",m_IntegrationTestScenes.ToArray()));
            EditorPrefs.SetString (Animator.StringToHash (Application.dataPath + "uttBuildScenes").ToString (), String.Join (",",m_OtherScenesToBuild.ToArray()));
        }
        
        private void LoadFromPrefereneces()
        {
            string storedTestScenes = EditorPrefs.GetString (Animator.StringToHash (Application.dataPath + "uttTestScenes").ToString ());
            string storedBuildScenes = EditorPrefs.GetString (Animator.StringToHash (Application.dataPath + "uttBuildScenes").ToString ());
            
            List<string> parsedTestScenes = storedTestScenes.Split (',').ToList ();
            List<string> parsedBuildScenes = storedBuildScenes.Split (',').ToList ();
            
            // Sanity check scenes actually exist
            foreach (string str in parsedTestScenes) {
                if (m_AllScenesInProject.Contains(str))
                    m_IntegrationTestScenes.Add(str);
            }
            
            foreach (string str in parsedBuildScenes) {
                if (m_AllScenesInProject.Contains(str))
                    m_OtherScenesToBuild.Add(str);
            }
        }
    }
}
