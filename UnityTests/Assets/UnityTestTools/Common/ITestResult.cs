using System;
using System.Collections.Generic;
using UnityEngine;
using UnityTest;

public interface ITestResult
{
    TestResultState ResultState { get; }

    string Message { get; }

    string Logs { get; }

    bool Executed { get; }

    string Name { get; }

    string FullName { get; }

    string Id { get; }

    bool IsSuccess { get; }

    double Duration { get; }

    string StackTrace { get; }
    
    bool IsIgnored { get; }
}
