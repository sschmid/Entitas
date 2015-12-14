using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    [Serializable]
    public class UnitTestResult : ITestResult
    {
        public bool Executed { get; set; }
        public string Name { get { return Test.MethodName; } }
        public string FullName { get { return Test.FullName; } }
        public TestResultState ResultState { get; set; }
        public UnitTestInfo Test { get; set; }
        public string Id { get { return Test.Id; } }
        public double Duration { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public bool IsIgnored { get; set; }

        public string Logs { get; set; }

        public bool Outdated { get; set; }

        public void Update(ITestResult source, bool outdated)
        {
            ResultState = source.ResultState;
            Duration = source.Duration;
            Message = source.Message;
            Logs = source.Logs;
            StackTrace = source.StackTrace;
            Executed = source.Executed;
            IsIgnored = source.IsIgnored || (Test != null && Test.IsIgnored);
            Outdated = outdated;
        }

        #region Helper methods

        public bool IsFailure
        {
            get { return ResultState == TestResultState.Failure; }
        }

        public bool IsError
        {
            get { return ResultState == TestResultState.Error; }
        }

        public bool IsSuccess
        {
            get { return ResultState == TestResultState.Success; }
        }

        public bool IsInconclusive
        {
            get { return ResultState == TestResultState.Inconclusive; }
        }

        #endregion
    }
}
