using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest.UnitTestRunner
{
    public class TestRunnerCallbackList : ITestRunnerCallback
    {
        private readonly List<ITestRunnerCallback> m_CallbackList = new List<ITestRunnerCallback>();

        public void TestStarted(string fullName)
        {
            foreach (var unitTestRunnerCallback in m_CallbackList)
            {
                unitTestRunnerCallback.TestStarted(fullName);
            }
        }

        public void TestFinished(ITestResult fullName)
        {
            foreach (var unitTestRunnerCallback in m_CallbackList)
            {
                unitTestRunnerCallback.TestFinished(fullName);
            }
        }

        public void RunStarted(string suiteName, int testCount)
        {
            foreach (var unitTestRunnerCallback in m_CallbackList)
            {
                unitTestRunnerCallback.RunStarted(suiteName,
                                                  testCount);
            }
        }

        public void RunFinished()
        {
            foreach (var unitTestRunnerCallback in m_CallbackList)
            {
                unitTestRunnerCallback.RunFinished();
            }
        }

        public void RunFinishedException(Exception exception)
        {
            foreach (var unitTestRunnerCallback in m_CallbackList)
            {
                unitTestRunnerCallback.RunFinishedException(exception);
            }
        }

        public void AllScenesFinished()
        {
            foreach (var unitTestRunnerCallback in m_CallbackList)
            {
                unitTestRunnerCallback.AllScenesFinished();
            }
        }

        public void Add(ITestRunnerCallback callback)
        {
            m_CallbackList.Add(callback);
        }

        public void Remove(ITestRunnerCallback callback)
        {
            m_CallbackList.Remove(callback);
        }
    }
}
