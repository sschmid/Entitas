using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    [Serializable]
    public class ResultDTO
    {
        public MessageType messageType;
        public int levelCount;
        public int loadedLevel;
        public string loadedLevelName;
        public string testName;
        public float testTimeout;
        public ITestResult testResult;

        private ResultDTO(MessageType messageType)
        {
            this.messageType = messageType;
            levelCount = Application.levelCount;
            loadedLevel = Application.loadedLevel;
            loadedLevelName = Application.loadedLevelName;
        }

        public enum MessageType : byte
        {
            Ping,
            RunStarted,
            RunFinished,
            TestStarted,
            TestFinished,
            RunInterrupted,
            AllScenesFinished
        }

        public static ResultDTO CreatePing()
        {
            var dto = new ResultDTO(MessageType.Ping);
            return dto;
        }

        public static ResultDTO CreateRunStarted()
        {
            var dto = new ResultDTO(MessageType.RunStarted);
            return dto;
        }

        public static ResultDTO CreateRunFinished(List<TestResult> testResults)
        {
            var dto = new ResultDTO(MessageType.RunFinished);
            return dto;
        }

        public static ResultDTO CreateTestStarted(TestResult test)
        {
            var dto = new ResultDTO(MessageType.TestStarted);
            dto.testName = test.FullName;
            dto.testTimeout = test.TestComponent.timeout;
            return dto;
        }

        public static ResultDTO CreateTestFinished(TestResult test)
        {
            var dto = new ResultDTO(MessageType.TestFinished);
            dto.testName = test.FullName;
            dto.testResult = GetSerializableTestResult(test);
            return dto;
        }

        public static ResultDTO CreateAllScenesFinished()
        {
            var dto = new ResultDTO(MessageType.AllScenesFinished);
            return dto;
        }

        private static ITestResult GetSerializableTestResult(TestResult test)
        {
            var str = new SerializableTestResult();

            str.resultState = test.ResultState;
            str.message = test.messages;
            str.executed = test.Executed;
            str.name = test.Name;
            str.fullName = test.FullName;
            str.id = test.id;
            str.isSuccess = test.IsSuccess;
            str.duration = test.duration;
            str.stackTrace = test.stacktrace;
            str.isIgnored = test.IsIgnored;

            return str;
        }
    }

    #region SerializableTestResult
    [Serializable]
    internal class SerializableTestResult : ITestResult
    {
        public TestResultState resultState;
        public string message;
        public bool executed;
        public string name;
        public string fullName;
        public string id;
        public bool isSuccess;
        public double duration;
        public string stackTrace;
        public bool isIgnored;

        public TestResultState ResultState
        {
            get { return resultState; }
        }

        public string Message
        {
            get { return message; }
        }

        public string Logs
        {
            get { return null; }
        }

        public bool Executed
        {
            get { return executed; }
        }

        public string Name
        {
            get { return name; }
        }

        public string FullName
        {
            get { return fullName; }
        }

        public string Id
        {
            get { return id; }
        }

        public bool IsSuccess
        {
            get { return isSuccess; }
        }

        public double Duration
        {
            get { return duration; }
        }

        public string StackTrace
        {
            get { return stackTrace; }
        }
        
        public bool IsIgnored 
        {
            get { return isIgnored; }
        }
    }
    #endregion
}
