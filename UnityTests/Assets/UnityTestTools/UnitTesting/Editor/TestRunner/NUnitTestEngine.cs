using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using NUnit.Core;
using NUnit.Core.Filters;
using UnityEditor;
using UnityEngine;
using UnityTest.UnitTestRunner;

namespace UnityTest
{
    public class NUnitTestEngine : IUnitTestEngine
    {
        static readonly string[] k_WhitelistedAssemblies =
        {
            "Assembly-CSharp-Editor",
            "Assembly-Boo-Editor",
            "Assembly-UnityScript-Editor"
        };
        private TestSuite m_TestSuite;

        public UnitTestRendererLine GetTests(out UnitTestResult[] results, out string[] categories)
        {
            if (m_TestSuite == null)
            {
                var assemblies = GetAssembliesWithTests().Select(a => a.Location).ToList();
                TestSuite suite = PrepareTestSuite(assemblies);
                m_TestSuite = suite;
            }

            var resultList = new List<UnitTestResult>();
            var categoryList = new HashSet<string>();

            UnitTestRendererLine lines = null;
            if (m_TestSuite != null)
                lines = ParseTestList(m_TestSuite, resultList, categoryList).Single();
            results = resultList.ToArray();
            categories = categoryList.ToArray();

            return lines;
        }

        private UnitTestRendererLine[] ParseTestList(Test test, List<UnitTestResult> results, HashSet<string> categories)
        {
            foreach (string category in test.Categories)
                categories.Add(category);

            if (test is TestMethod)
            {
                var result = new UnitTestResult
                {
                    Test = new UnitTestInfo(test as TestMethod)
                };

                results.Add(result);
                return new[] { new TestLine(test as TestMethod, result.Id) };
            }

            GroupLine group = null;
            if (test is TestSuite)
                group = new GroupLine(test as TestSuite);

            var namespaceList = new List<UnitTestRendererLine>(new[] {group});

            foreach (Test result in test.Tests)
            {
                if (result is NamespaceSuite || test is TestAssembly)
                    namespaceList.AddRange(ParseTestList(result, results, categories));
                else
                    group.AddChildren(ParseTestList(result, results, categories));
            }

            namespaceList.Sort();
            return namespaceList.ToArray();
        }

        public void RunTests(ITestRunnerCallback testRunnerEventListener)
        {
            RunTests(TestFilter.Empty, testRunnerEventListener);
        }

        public void RunTests(TestFilter filter, ITestRunnerCallback testRunnerEventListener)
        {
            try
            {
                if (testRunnerEventListener != null)
                    testRunnerEventListener.RunStarted(m_TestSuite.TestName.FullName, m_TestSuite.TestCount);

                ExecuteTestSuite(m_TestSuite, testRunnerEventListener, filter);

                if (testRunnerEventListener != null)
                    testRunnerEventListener.RunFinished();
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                if (testRunnerEventListener != null)
                    testRunnerEventListener.RunFinishedException(e);
            }
        }

        public static Assembly[] GetAssembliesWithTests()
        {
            var libs = new List<Assembly>();
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.GetReferencedAssemblies().All(a => a.Name != "nunit.framework"))
                    continue;
                if (assembly.Location.Replace('\\', '/').StartsWith(Application.dataPath)
                    || k_WhitelistedAssemblies.Contains(assembly.GetName().Name))
                    libs.Add(assembly);
            }
            return libs.ToArray();
        }

        private TestSuite PrepareTestSuite(List<String> assemblyList)
        {
            CoreExtensions.Host.InitializeService();
            var testPackage = new TestPackage(PlayerSettings.productName, assemblyList);
            var builder = new TestSuiteBuilder();
            TestExecutionContext.CurrentContext.TestPackage = testPackage;
            TestSuite suite = builder.Build(testPackage);
            return suite;
        }

        private void ExecuteTestSuite(TestSuite suite, ITestRunnerCallback testRunnerEventListener, TestFilter filter)
        {
            EventListener eventListener;
            if (testRunnerEventListener == null)
                eventListener = new NullListener();
            else
                eventListener = new TestRunnerEventListener(testRunnerEventListener);

            TestExecutionContext.CurrentContext.Out = new EventListenerTextWriter(eventListener, TestOutputType.Out);
            TestExecutionContext.CurrentContext.Error = new EventListenerTextWriter(eventListener, TestOutputType.Error);

            suite.Run(eventListener, GetFilter(filter));
        }

        private ITestFilter GetFilter(TestFilter filter)
        {
            var nUnitFilter = new AndFilter();

            if (filter.names != null && filter.names.Length > 0)
                nUnitFilter.Add(new SimpleNameFilter(filter.names));
            if (filter.categories != null && filter.categories.Length > 0)
                nUnitFilter.Add(new CategoryFilter(filter.categories));
            if (filter.objects != null && filter.objects.Length > 0)
                nUnitFilter.Add(new OrFilter(filter.objects.Where(o => o is TestName).Select(o => new NameFilter(o as TestName)).ToArray()));
            return nUnitFilter;
        }
    
        public class TestRunnerEventListener : EventListener
        {
            private readonly ITestRunnerCallback m_TestRunnerEventListener;
            private StringBuilder m_testLog;

            public TestRunnerEventListener(ITestRunnerCallback testRunnerEventListener)
            {
                m_TestRunnerEventListener = testRunnerEventListener;
            }

            public void RunStarted(string name, int testCount)
            {
                m_TestRunnerEventListener.RunStarted(name, testCount);
            }

            public void RunFinished(NUnit.Core.TestResult result)
            {
                m_TestRunnerEventListener.RunFinished();
            }

            public void RunFinished(Exception exception)
            {
                m_TestRunnerEventListener.RunFinishedException(exception);
            }

            public void TestStarted(TestName testName)
            {
                m_testLog = new StringBuilder();
                m_TestRunnerEventListener.TestStarted(testName.FullName);
            }

            public void TestFinished(NUnit.Core.TestResult result)
            {
                m_TestRunnerEventListener.TestFinished(result.UnitTestResult(m_testLog.ToString()));
                m_testLog = null;
            }

            public void SuiteStarted(TestName testName)
            {
            }

            public void SuiteFinished(NUnit.Core.TestResult result)
            {
            }

            public void UnhandledException(Exception exception)
            {
            }

            public void TestOutput(TestOutput testOutput)
            {
                if (m_testLog != null)
                    m_testLog.AppendLine(testOutput.Text);
            }
        }
    }
}
