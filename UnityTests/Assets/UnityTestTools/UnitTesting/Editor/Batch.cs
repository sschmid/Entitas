using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityTest.UnitTestRunner;

namespace UnityTest
{
    public static partial class Batch
    {
        const string k_ResultFilePathParam = "-resultFilePath=";
        const string k_TestFilterParam = "-filter=";
        const string k_CategoryParam = "-categories=";
        const string k_DefaultResultFileName = "UnitTestResults.xml";

        public static int returnCodeTestsOk = 0;
        public static int returnCodeTestsFailed = 2;
        public static int returnCodeRunError = 3;

        public static void RunUnitTests()
        {
            PlayerSettings.useMacAppStoreValidation = false;
            var filter = GetTestFilter();
            var resultFilePath = GetParameterArgument(k_ResultFilePathParam) ?? Directory.GetCurrentDirectory();
            if (Directory.Exists(resultFilePath))
                resultFilePath = Path.Combine(resultFilePath, k_DefaultResultFileName);
            EditorApplication.NewScene();
            var engine = new NUnitTestEngine();
            UnitTestResult[] results;
            string[] categories;
            engine.GetTests(out results, out categories);
            engine.RunTests(filter, new TestRunnerEventListener(resultFilePath, results.ToList()));
        }

        private static TestFilter GetTestFilter()
        {
            var testFilterArg = GetParameterArgumentArray(k_TestFilterParam);
            var testCategoryArg = GetParameterArgumentArray(k_CategoryParam);
            var filter = new TestFilter
            {
                names = testFilterArg,
                categories = testCategoryArg
            };
            return filter;
        }

        private static string[] GetParameterArgumentArray(string parameterName)
        {
            var arg = GetParameterArgument(parameterName);
            if (string.IsNullOrEmpty(arg)) return null;
            return arg.Split(',').Select(s => s.Trim()).ToArray();
        }

        private static string GetParameterArgument(string parameterName)
        {
            foreach (var arg in Environment.GetCommandLineArgs())
            {
                if (arg.ToLower().StartsWith(parameterName.ToLower()))
                {
                    return arg.Substring(parameterName.Length);
                }
            }
            return null;
        }

        private class TestRunnerEventListener : ITestRunnerCallback
        {
            private readonly string m_ResultFilePath;
            private readonly List<UnitTestResult> m_Results;

            public TestRunnerEventListener(string resultFilePath, List<UnitTestResult> resultList)
            {
                m_ResultFilePath = resultFilePath;
                m_Results = resultList;
            }

            public void TestFinished(ITestResult test)
            {
                m_Results.Single(r => r.Id == test.Id).Update(test, false);
            }

            public void RunFinished()
            {
                var resultDestiantion = Application.dataPath;
                if (!string.IsNullOrEmpty(m_ResultFilePath))
                    resultDestiantion = m_ResultFilePath;
                var fileName = Path.GetFileName(resultDestiantion);
                if (!string.IsNullOrEmpty(fileName))
                    resultDestiantion = resultDestiantion.Substring(0, resultDestiantion.Length - fileName.Length);
                else
                    fileName = "UnitTestResults.xml";
#if !UNITY_METRO
                var resultWriter = new XmlResultWriter("Unit Tests", "Editor", m_Results.ToArray());
                resultWriter.WriteToFile(resultDestiantion, fileName);
#endif
                var executed = m_Results.Where(result => result.Executed);
                if (!executed.Any())
                {
                    EditorApplication.Exit(returnCodeRunError);
                    return;
                }
                var failed = executed.Where(result => !result.IsSuccess);
                EditorApplication.Exit(failed.Any() ? returnCodeTestsFailed : returnCodeTestsOk);
            }

            public void TestStarted(string fullName)
            {
            }

            public void RunStarted(string suiteName, int testCount)
            {
            }

            public void RunFinishedException(Exception exception)
            {
                EditorApplication.Exit(returnCodeRunError);
                throw exception;
            }

            public void AllScenesFinished()
            {
            }
        }
    }
}
