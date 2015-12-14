using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Core;
using UnityEngine;
using Object = System.Object;

namespace UnityTest
{
    [Serializable]
    public class UnitTestInfo
    {
        public string ParamName { get; private set; }
        public string MethodName { get; private set; }
        public string FullMethodName { get; private set; }
        public string ClassName { get; private set; }
        public string FullClassName { get; private set; }
        public string Namespace { get; private set; }
        public string FullName { get; private set; }
        public string[] Categories { get; private set; }
        public string AssemblyPath { get; private set; }
        public string Id { get; private set; }
        public bool IsIgnored { get; private set; }

        public UnitTestInfo(TestMethod testMethod)
        {
            if (testMethod == null)
                throw new ArgumentException();

            MethodName = testMethod.MethodName;
            FullMethodName = testMethod.Method.ToString();
            ClassName = testMethod.FixtureType.Name;
            FullClassName = testMethod.ClassName;
            Namespace = testMethod.Method.ReflectedType.Namespace;
            FullName = testMethod.TestName.FullName;
            ParamName = ExtractMethodCallParametersString(FullName);
            Id = testMethod.TestName.TestID.ToString();

            Categories = testMethod.Categories.Cast<string>().ToArray();

            AssemblyPath = GetAssemblyPath(testMethod);
            
            IsIgnored = (testMethod.RunState == RunState.Ignored);
        }

        private string GetAssemblyPath(TestMethod testMethod)
        {
            var parent = testMethod as Test;
            var assemblyPath = "";
            while (parent != null)
            {
                parent = parent.Parent;
                if (!(parent is TestAssembly)) continue;
                var path = (parent as TestAssembly).TestName.FullName;
                if (!File.Exists(path)) continue;
                assemblyPath = path;
                break;
            }
            return assemblyPath;
        }

        public UnitTestInfo(string id)
        {
            Id = id;
        }

        public override bool Equals(Object obj)
        {
            if (!(obj is UnitTestInfo)) return false;

            var testInfo = (UnitTestInfo)obj;
            return Id == testInfo.Id;
        }

        public static bool operator ==(UnitTestInfo a, UnitTestInfo b)
        {
            if (((object)a == null) || ((object)b == null)) return false;
            return a.Id == b.Id;
        }

        public static bool operator !=(UnitTestInfo a, UnitTestInfo b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        static string ExtractMethodCallParametersString(string methodFullName)
        {
            var match = Regex.Match(methodFullName, @"\((.*)\)");
            string result = "";
            if (match.Groups[1].Success)
            {
                result = match.Groups[1].Captures[0].Value;
            }
            return result;
        }
    }
}
