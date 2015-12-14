using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest.IntegrationTestRunner
{
    public interface ITestRunnerCallback
    {
        void RunStarted(string platform, List<TestComponent> testsToRun);
        void RunFinished(List<TestResult> testResults);
        void AllScenesFinished();
        void TestStarted(TestResult test);
        void TestFinished(TestResult test);
        void TestRunInterrupted(List<ITestComponent> testsNotRun);
    }
}
