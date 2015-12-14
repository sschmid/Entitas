using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityTest.UnitTestRunner;

namespace UnityTest
{
    public partial class UnitTestView
    {
        private void UpdateTestInfo(ITestResult result)
        {
            FindTestResult(result.Id).Update(result, false);
            m_FilterSettings.UpdateCounters(m_ResultList.Cast<ITestResult>());
        }

        private UnitTestResult FindTestResult(string resultId)
        {
            var idx = m_ResultList.FindIndex(testResult => testResult.Id == resultId);
            if (idx == -1)
            {
                Debug.LogWarning("Id not found for test: " + resultId);
                return null;
            }
            return m_ResultList.ElementAt(idx);
        }

        private void RunTests()
        {
            var filter = new TestFilter();
            var categories = m_FilterSettings.GetSelectedCategories();
            if (categories != null && categories.Length > 0)
                filter.categories = categories;
            RunTests(filter);
        }

        private void RunTests(TestFilter filter)
        {
            if (m_Settings.runTestOnANewScene)
            {
                if (m_Settings.autoSaveSceneBeforeRun) EditorApplication.SaveScene();
                if (!EditorApplication.SaveCurrentSceneIfUserWantsTo()) return;
            }

            string currentScene = null;
            int undoGroup = -1;
            if (m_Settings.runTestOnANewScene)
                currentScene = OpenNewScene();
            else
                undoGroup = RegisterUndo();

            StartTestRun(filter, new TestRunnerEventListener(UpdateTestInfo));

            if (m_Settings.runTestOnANewScene)
                LoadPreviousScene(currentScene);
            else
                PerformUndo(undoGroup);
        }

        private string OpenNewScene()
        {
            var currentScene = EditorApplication.currentScene;
            if (m_Settings.runTestOnANewScene)
                EditorApplication.NewScene();
            return currentScene;
        }

        private void LoadPreviousScene(string currentScene)
        {
            if (!string.IsNullOrEmpty(currentScene))
                EditorApplication.OpenScene(currentScene);
            else
                EditorApplication.NewScene();

            if (Event.current != null)
                GUIUtility.ExitGUI();
        }

        public void StartTestRun(TestFilter filter, ITestRunnerCallback eventListener)
        {
            var callbackList = new TestRunnerCallbackList();
            if (eventListener != null) callbackList.Add(eventListener);
            k_TestEngine.RunTests(filter, callbackList);
        }

        private static int RegisterUndo()
        {
            return Undo.GetCurrentGroup();
        }

        private static void PerformUndo(int undoGroup)
        {
            EditorUtility.DisplayProgressBar("Undo", "Reverting changes to the scene", 0);
            var undoStartTime = DateTime.Now;
            Undo.RevertAllDownToGroup(undoGroup);
            if ((DateTime.Now - undoStartTime).Seconds > 1)
                Debug.LogWarning("Undo after unit test run took " + (DateTime.Now - undoStartTime).Seconds + " seconds. Consider running unit tests on a new scene for better performance.");
            EditorUtility.ClearProgressBar();
        }

        public class TestRunnerEventListener : ITestRunnerCallback
        {
            private readonly Action<ITestResult> m_UpdateCallback;
			private int m_TestCount;

            public TestRunnerEventListener(Action<ITestResult> updateCallback)
            {
                m_UpdateCallback = updateCallback;
            }

            public void TestStarted(string fullName)
            {
				if(m_TestCount < 100)
                	EditorUtility.DisplayProgressBar("Unit Tests Runner", fullName, 1);
				else
					EditorUtility.DisplayProgressBar("Unit Tests Runner", "Running unit tests...", 1);
            }

            public void TestFinished(ITestResult result)
            {
                m_UpdateCallback(result);
            }

            public void RunStarted(string suiteName, int testCount)
            {
				m_TestCount = testCount;
            }

            public void AllScenesFinished()
            {
            }

            public void RunFinished()
            {
                EditorUtility.ClearProgressBar();
            }

            public void RunFinishedException(Exception exception)
            {
                RunFinished();
            }
        }

        [MenuItem("Unity Test Tools/Unit Test Runner %#&u")]
        public static void ShowWindow()
        {
            GetWindow(typeof(UnitTestView)).Show();
        }
    }

    public class TestFilter
    {
        public string[] names;
        public string[] categories;
        public object[] objects;
        public static TestFilter Empty = new TestFilter();
    }
}
