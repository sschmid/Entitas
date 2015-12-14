using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEditor.Callbacks;

namespace UnityTest
{
    [Serializable]
    public partial class UnitTestView : EditorWindow, IHasCustomMenu
    {
        private static UnitTestView s_Instance;
        private IUnitTestEngine k_TestEngine = new NUnitTestEngine();

        [SerializeField] private List<UnitTestResult> m_ResultList = new List<UnitTestResult>();
        [SerializeField] private List<string> m_FoldMarkers = new List<string>();
        [SerializeField] private List<UnitTestRendererLine> m_SelectedLines = new List<UnitTestRendererLine>();
        UnitTestRendererLine m_TestLines;
        
        private TestFilterSettings m_FilterSettings;

        #region runner steering vars
        private Vector2 m_TestListScroll, m_TestInfoScroll;
        private float m_HorizontalSplitBarPosition = 200;
        private float m_VerticalSplitBarPosition = 300;
        #endregion

        private UnitTestsRunnerSettings m_Settings;

        #region GUI Contents
        private readonly GUIContent m_GUIRunSelectedTestsIcon = new GUIContent("Run Selected", "Run selected tests");
        private readonly GUIContent m_GUIRunAllTestsIcon = new GUIContent("Run All", "Run all tests");
        private readonly GUIContent m_GUIRerunFailedTestsIcon = new GUIContent("Rerun Failed", "Rerun failed tests");
        private readonly GUIContent m_GUIRunOnRecompile = new GUIContent("Run on recompile", "Run all tests after recompilation");
        private readonly GUIContent m_GUIShowDetailsBelowTests = new GUIContent("Show details below tests", "Show run details below test list");
        private readonly GUIContent m_GUIRunTestsOnNewScene = new GUIContent("Run tests on a new scene", "Run tests on a new scene");
        private readonly GUIContent m_GUIAutoSaveSceneBeforeRun = new GUIContent("Autosave scene", "The runner will automatically save the current scene changes before it starts");
		private readonly GUIContent m_GUIReloadTestData = new GUIContent("Reload test list", "Reloads the test list. Useful with external data sources.");
        #endregion

        public UnitTestView()
        {
            m_ResultList.Clear();
        }

        public void OnEnable()
        {
			titleContent = new GUIContent("Unit Tests");
            s_Instance = this;
            m_Settings = ProjectSettingsBase.Load<UnitTestsRunnerSettings>();
            m_FilterSettings = new TestFilterSettings("UnityTest.UnitTestView");
            RefreshTests();
        }

		[DidReloadScripts]
		public static void OnDidReloadScripts()
		{
			if (s_Instance != null && s_Instance.m_Settings.runOnRecompilation) 
			{
				s_Instance.RunTests();
				s_Instance.Repaint();
			}
		}

        public void OnDestroy()
        {
            s_Instance = null;
        }
        
        public void OnGUI()
        {
            EditorGUILayout.BeginVertical();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            if (GUILayout.Button(m_GUIRunAllTestsIcon, EditorStyles.toolbarButton))
            {
                RunTests();
                GUIUtility.ExitGUI();
            }
            EditorGUI.BeginDisabledGroup(!m_TestLines.IsAnySelected);
            if (GUILayout.Button(m_GUIRunSelectedTestsIcon, EditorStyles.toolbarButton))
            {
                m_TestLines.RunSelectedTests();
            }
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(m_GUIRerunFailedTestsIcon, EditorStyles.toolbarButton))
            {
                m_TestLines.RunTests(m_ResultList.Where(result => result.IsFailure || result.IsError).Select(l => l.FullName).ToArray());
            }

            GUILayout.FlexibleSpace();
            
            m_FilterSettings.OnGUI ();

            EditorGUILayout.EndHorizontal();

            if (m_Settings.horizontalSplit)
                EditorGUILayout.BeginVertical();
            else
                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

            RenderTestList();
            RenderTestInfo();

            if (m_Settings.horizontalSplit)
                EditorGUILayout.EndVertical();
            else
                EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void RenderTestList()
        {
            EditorGUILayout.BeginVertical(Styles.testList);
            m_TestListScroll = EditorGUILayout.BeginScrollView(m_TestListScroll,
                                                               GUILayout.ExpandWidth(true),
                                                               GUILayout.MaxWidth(2000));
            if (m_TestLines != null)
            {
                if (m_TestLines.Render(m_FilterSettings.BuildRenderingOptions())) Repaint();
            }
            EditorGUILayout.EndScrollView();
            EditorGUILayout.EndVertical();
        }

        private void RenderTestInfo()
        {
            var ctrlId = GUIUtility.GetControlID(FocusType.Passive);
            var rect = GUILayoutUtility.GetLastRect();
            if (m_Settings.horizontalSplit)
            {
                rect.y = rect.height + rect.y - 1;
                rect.height = 3;
            }
            else
            {
                rect.x = rect.width + rect.x - 1;
                rect.width = 3;
            }

            EditorGUIUtility.AddCursorRect(rect, m_Settings.horizontalSplit ? MouseCursor.ResizeVertical : MouseCursor.ResizeHorizontal);
            var e = Event.current;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (GUIUtility.hotControl == 0 && rect.Contains(e.mousePosition))
                        GUIUtility.hotControl = ctrlId;
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == ctrlId)
                    {
                        m_HorizontalSplitBarPosition -= e.delta.y;
                        if (m_HorizontalSplitBarPosition < 20) m_HorizontalSplitBarPosition = 20;
                        m_VerticalSplitBarPosition -= e.delta.x;
                        if (m_VerticalSplitBarPosition < 20) m_VerticalSplitBarPosition = 20;
                        Repaint();
                    }

                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == ctrlId)
                        GUIUtility.hotControl = 0;
                    break;
            }
            m_TestInfoScroll = EditorGUILayout.BeginScrollView(m_TestInfoScroll, m_Settings.horizontalSplit
                                                               ? GUILayout.MinHeight(m_HorizontalSplitBarPosition)
                                                               : GUILayout.Width(m_VerticalSplitBarPosition));

            var text = "";
            if (m_SelectedLines.Any())
            {
                text = m_SelectedLines.First().GetResultText();
            }

			var resultTextSize = Styles.info.CalcSize(new GUIContent(text));
			EditorGUILayout.SelectableLabel(text, Styles.info,
			                                GUILayout.ExpandHeight(true), 
			                                GUILayout.ExpandWidth(true), 
			                                GUILayout.MinWidth(resultTextSize.x), 
			                                GUILayout.MinHeight(resultTextSize.y));
			
			EditorGUILayout.EndScrollView();
        }
        
        private void ToggleRunOnRecompilation()
        {
            m_Settings.runOnRecompilation = !m_Settings.runOnRecompilation;
        }
        
        public void AddItemsToMenu (GenericMenu menu)
        {
            menu.AddItem(m_GUIRunOnRecompile, m_Settings.runOnRecompilation, ToggleRunOnRecompilation);
            menu.AddItem(m_GUIRunTestsOnNewScene, m_Settings.runTestOnANewScene, m_Settings.ToggleRunTestOnANewScene);
            if(!m_Settings.runTestOnANewScene)
                menu.AddDisabledItem(m_GUIAutoSaveSceneBeforeRun);
            else
                menu.AddItem(m_GUIAutoSaveSceneBeforeRun, m_Settings.autoSaveSceneBeforeRun, m_Settings.ToggleAutoSaveSceneBeforeRun);
            menu.AddItem(m_GUIShowDetailsBelowTests, m_Settings.horizontalSplit, m_Settings.ToggleHorizontalSplit);
            menu.AddSeparator("");
            menu.AddItem(m_GUIReloadTestData, false, ReloadTestList);
        }

        private void ReloadTestList()
        {
            k_TestEngine = new NUnitTestEngine();
            RefreshTests();
        }

        private void RefreshTests()
        {
            UnitTestResult[] newResults;
            m_TestLines = k_TestEngine.GetTests(out newResults, out m_FilterSettings.AvailableCategories);

            foreach (var newResult in newResults)
            {
                var result = m_ResultList.Where(t => t.Test == newResult.Test && t.FullName == newResult.FullName).ToArray();
                if (result.Count() != 1) continue;
                newResult.Update(result.Single(), true);
            }

            UnitTestRendererLine.SelectedLines = m_SelectedLines;
            UnitTestRendererLine.RunTest = RunTests;
            GroupLine.FoldMarkers = m_FoldMarkers;
            TestLine.GetUnitTestResult = FindTestResult;

            m_ResultList = new List<UnitTestResult>(newResults);
            
            m_FilterSettings.UpdateCounters(m_ResultList.Cast<ITestResult>());

            Repaint();
        }
    }
}
