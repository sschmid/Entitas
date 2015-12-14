using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public static class IntegrationTest
{
    public const string passMessage = "IntegrationTest Pass";
    public const string failMessage = "IntegrationTest Fail";

    public static void Pass()
    {
        LogResult(passMessage);
    }

    public static void Pass(GameObject go)
    {
        LogResult(go, passMessage);
    }

    public static void Fail(string reason)
    {
        Fail();
        if (!string.IsNullOrEmpty(reason)) Debug.Log(reason);
    }

    public static void Fail(GameObject go, string reason)
    {
        Fail(go);
        if (!string.IsNullOrEmpty(reason)) Debug.Log(reason);
    }

    public static void Fail()
    {
        LogResult(failMessage);
    }

    public static void Fail(GameObject go)
    {
        LogResult(go, failMessage);
    }

    public static void Assert(bool condition)
    {
        Assert(condition, "");
    }

    public static void Assert(GameObject go, bool condition)
    {
        Assert(go, condition, "");
    }

    public static void Assert(bool condition, string message)
    {
        if (!condition) 
            Fail(message);
    }

    public static void Assert(GameObject go, bool condition, string message)
    {
        if (!condition) 
            Fail(go, message);
    }

    private static void LogResult(string message)
    {
        Debug.Log(message);
    }

    private static void LogResult(GameObject go, string message)
    {
        Debug.Log(message + " (" + FindTestObject(go).name + ")", go);
    }

    private static GameObject FindTestObject(GameObject go)
    {
        var temp = go;
        while (temp.transform.parent != null)
        {
            if (temp.GetComponent("TestComponent") != null)
                return temp;
            temp = temp.transform.parent.gameObject;
        }
        return go;
    }

    #region Dynamic test attributes

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExcludePlatformAttribute : Attribute
    {
        public string[] platformsToExclude;

        public ExcludePlatformAttribute(params RuntimePlatform[] platformsToExclude)
        {
            this.platformsToExclude = platformsToExclude.Select(platform => platform.ToString()).ToArray();
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ExpectExceptions : Attribute
    {
        public string[] exceptionTypeNames;
        public bool succeedOnException;

        public ExpectExceptions() : this(false)
        {
        }

        public ExpectExceptions(bool succeedOnException) : this(succeedOnException, new string[0])
        {
        }

        public ExpectExceptions(bool succeedOnException, params string[] exceptionTypeNames)
        {
            this.succeedOnException = succeedOnException;
            this.exceptionTypeNames = exceptionTypeNames;
        }

        public ExpectExceptions(bool succeedOnException, params Type[] exceptionTypes)
            : this(succeedOnException, exceptionTypes.Select(type => type.FullName).ToArray())
        {
        }

        public ExpectExceptions(params string[] exceptionTypeNames) : this(false, exceptionTypeNames)
        {
        }

        public ExpectExceptions(params Type[] exceptionTypes) : this(false, exceptionTypes)
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class IgnoreAttribute : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class DynamicTestAttribute : Attribute
    {
        private readonly string m_SceneName;

        public DynamicTestAttribute(string sceneName)
        {
            if (sceneName.EndsWith(".unity"))
                sceneName = sceneName.Substring(0, sceneName.Length - ".unity".Length);
            m_SceneName = sceneName;
        }

        public bool IncludeOnScene(string sceneName)
        {
            var fileName = Path.GetFileNameWithoutExtension(sceneName);
            return fileName == m_SceneName;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SucceedWithAssertions : Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TimeoutAttribute : Attribute
    {
        public float timeout;

        public TimeoutAttribute(float seconds)
        {
            timeout = seconds;
        }
    }

    #endregion
}
