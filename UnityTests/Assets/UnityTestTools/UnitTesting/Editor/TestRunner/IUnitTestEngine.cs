using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTest.UnitTestRunner;

namespace UnityTest
{
    public interface IUnitTestEngine
    {
        UnitTestRendererLine GetTests(out UnitTestResult[] results, out string[] categories);
        void RunTests(TestFilter filter, ITestRunnerCallback testRunnerEventListener);
    }
}
