using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public static class NUnitExtensions
    {
        public static UnitTestResult UnitTestResult(this NUnit.Core.TestResult result, string logs)
        {
            return new UnitTestResult
                   {
                       Executed = result.Executed,
                       ResultState = (TestResultState)result.ResultState,
                       Message = result.Message,
                       Logs = logs, 
                       StackTrace = result.StackTrace,
                       Duration = result.Time,
                       Test = new UnitTestInfo(result.Test.TestName.TestID.ToString()),
                       IsIgnored = (result.ResultState == NUnit.Core.ResultState.Ignored) || result.Test.RunState == NUnit.Core.RunState.Ignored
                   };
        }
    }
}
