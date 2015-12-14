using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace UnityTest
{
    public interface ITestComponent : IComparable<ITestComponent>
    {
        void EnableTest(bool enable);
        bool IsTestGroup();
        GameObject gameObject { get; }
        string Name { get; }
        ITestComponent GetTestGroup();
        bool IsExceptionExpected(string exceptionType);
        bool ShouldSucceedOnException();
        double GetTimeout();
        bool IsIgnored();
        bool ShouldSucceedOnAssertions();
        bool IsExludedOnThisPlatform();
    }

    public class TestComponent : MonoBehaviour, ITestComponent
    {
        public static ITestComponent NullTestComponent = new NullTestComponentImpl();

        public float timeout = 5;
        public bool ignored = false;
        public bool succeedAfterAllAssertionsAreExecuted = false;
        public bool expectException = false;
        public string expectedExceptionList = "";
        public bool succeedWhenExceptionIsThrown = false;
        public IncludedPlatforms includedPlatforms = (IncludedPlatforms) ~0L;
        public string[] platformsToIgnore = null;

        public bool dynamic;
        public string dynamicTypeName;

        public bool IsExludedOnThisPlatform()
        {
            return platformsToIgnore != null && platformsToIgnore.Any(platform => platform == Application.platform.ToString());
        }

        static bool IsAssignableFrom(Type a, Type b)
        {
#if !UNITY_METRO
            return a.IsAssignableFrom(b);
#else
            return false;
#endif
        }

        public bool IsExceptionExpected(string exception)
		{
			exception = exception.Trim();
            if (!expectException) 
				return false;
			if(string.IsNullOrEmpty(expectedExceptionList.Trim())) 
				return true;
            foreach (var expectedException in expectedExceptionList.Split(',').Select(e => e.Trim()))
            {
                if (exception == expectedException) 
					return true;
                var exceptionType = Type.GetType(exception) ?? GetTypeByName(exception);
                var expectedExceptionType = Type.GetType(expectedException) ?? GetTypeByName(expectedException);
                if (exceptionType != null && expectedExceptionType != null && IsAssignableFrom(expectedExceptionType, exceptionType))
                    return true;
            }
            return false;
        }

        public bool ShouldSucceedOnException()
        {
            return succeedWhenExceptionIsThrown;
        }

        public double GetTimeout()
        {
            return timeout;
        }

        public bool IsIgnored()
        {
            return ignored;
        }

        public bool ShouldSucceedOnAssertions()
        {
            return succeedAfterAllAssertionsAreExecuted;
        }

        private static Type GetTypeByName(string className)
        {
#if !UNITY_METRO
            return AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes()).FirstOrDefault(type => type.Name == className);
#else
            return null;
#endif
        }

        public void OnValidate()
        {
            if (timeout < 0.01f) timeout = 0.01f;
        }

        // Legacy
        [Flags]
        public enum IncludedPlatforms
        {
            WindowsEditor       = 1 << 0,
                OSXEditor           = 1 << 1,
                WindowsPlayer       = 1 << 2,
                OSXPlayer           = 1 << 3,
                LinuxPlayer         = 1 << 4,
                MetroPlayerX86      = 1 << 5,
                MetroPlayerX64      = 1 << 6,
                MetroPlayerARM      = 1 << 7,
                WindowsWebPlayer    = 1 << 8,
                OSXWebPlayer        = 1 << 9,
                Android             = 1 << 10,
// ReSharper disable once InconsistentNaming
                IPhonePlayer        = 1 << 11,
                TizenPlayer         = 1 << 12,
                WP8Player           = 1 << 13,
                BB10Player          = 1 << 14,
                NaCl                = 1 << 15,
                PS3                 = 1 << 16,
                XBOX360             = 1 << 17,
                WiiPlayer           = 1 << 18,
                PSP2                = 1 << 19,
                PS4                 = 1 << 20,
                PSMPlayer           = 1 << 21,
                XboxOne             = 1 << 22,
        }

        #region ITestComponent implementation

        public void EnableTest(bool enable)
        {
            if (enable && dynamic)
            {
                Type t = Type.GetType(dynamicTypeName);
                var s = gameObject.GetComponent(t) as MonoBehaviour;
                if (s != null)
                    DestroyImmediate(s);

                gameObject.AddComponent(t);
            }

            if (gameObject.activeSelf != enable) gameObject.SetActive(enable);
        }

        public int CompareTo(ITestComponent obj)
        {
            if (obj == NullTestComponent)
                return 1;
            var result = gameObject.name.CompareTo(obj.gameObject.name);
            if (result == 0)
                result = gameObject.GetInstanceID().CompareTo(obj.gameObject.GetInstanceID());
            return result;
        }

        public bool IsTestGroup()
        {
            for (int i = 0; i < gameObject.transform.childCount; i++)
            {
                var childTc = gameObject.transform.GetChild(i).GetComponent(typeof(TestComponent));
                if (childTc != null)
                    return true;
            }
            return false;
        }

        public string Name { get { return gameObject == null ? "" : gameObject.name; } }

        public ITestComponent GetTestGroup()
        {
            var parent = gameObject.transform.parent;
            if (parent == null)
                return NullTestComponent;
            return parent.GetComponent<TestComponent>();
        }

        public override bool Equals(object o)
        {
            if (o is TestComponent)
                return this == (o as TestComponent);
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(TestComponent a, TestComponent b)
        {
            if (ReferenceEquals(a, b))
                return true;
            if (((object)a == null) || ((object)b == null))
                return false;
            if (a.dynamic && b.dynamic)
                return a.dynamicTypeName == b.dynamicTypeName;
            if (a.dynamic || b.dynamic)
                return false;
            return a.gameObject == b.gameObject;
        }

        public static bool operator !=(TestComponent a, TestComponent b)
        {
            return !(a == b);
        }

        #endregion

        #region Static helpers

        public static TestComponent CreateDynamicTest(Type type)
        {
            var go = CreateTest(type.Name);
            go.hideFlags |= HideFlags.DontSave;
            go.SetActive(false);

            var tc = go.GetComponent<TestComponent>();
            tc.dynamic = true;
            tc.dynamicTypeName = type.AssemblyQualifiedName;

#if !UNITY_METRO
            foreach (var typeAttribute in type.GetCustomAttributes(false))
            {
                if (typeAttribute is IntegrationTest.TimeoutAttribute)
                    tc.timeout = (typeAttribute as IntegrationTest.TimeoutAttribute).timeout;
                else if (typeAttribute is IntegrationTest.IgnoreAttribute)
                    tc.ignored = true;
                else if (typeAttribute is IntegrationTest.SucceedWithAssertions)
                    tc.succeedAfterAllAssertionsAreExecuted = true;
                else if (typeAttribute is IntegrationTest.ExcludePlatformAttribute)
                    tc.platformsToIgnore = (typeAttribute as IntegrationTest.ExcludePlatformAttribute).platformsToExclude;
                else if (typeAttribute is IntegrationTest.ExpectExceptions)
                {
                    var attribute = (typeAttribute as IntegrationTest.ExpectExceptions);
                    tc.expectException = true;
                    tc.expectedExceptionList = string.Join(",", attribute.exceptionTypeNames);
                    tc.succeedWhenExceptionIsThrown = attribute.succeedOnException;
                }
            }
            go.AddComponent(type);
#endif  // if !UNITY_METRO
            return tc;
        }

        public static GameObject CreateTest()
        {
            return CreateTest("New Test");
        }

        private static GameObject CreateTest(string name)
        {
            var go = new GameObject(name);
            go.AddComponent<TestComponent>();
#if UNITY_EDITOR
            Undo.RegisterCreatedObjectUndo(go, "Created test");
#endif
            return go;
        }

        public static List<TestComponent> FindAllTestsOnScene()
        {
            var tests = Resources.FindObjectsOfTypeAll (typeof(TestComponent)).Cast<TestComponent> ();
#if UNITY_EDITOR
            tests = tests.Where( t => {var p = PrefabUtility.GetPrefabType(t); return p != PrefabType.Prefab && p != PrefabType.ModelPrefab;} );

#endif
            return tests.ToList ();
        }

        public static List<TestComponent> FindAllTopTestsOnScene()
        {
            return FindAllTestsOnScene().Where(component => component.gameObject.transform.parent == null).ToList();
        }

        public static List<TestComponent> FindAllDynamicTestsOnScene()
        {
            return FindAllTestsOnScene().Where(t => t.dynamic).ToList();
        }

        public static void DestroyAllDynamicTests()
        {
            foreach (var dynamicTestComponent in FindAllDynamicTestsOnScene())
                DestroyImmediate(dynamicTestComponent.gameObject);
        }

        public static void DisableAllTests()
        {
            foreach (var t in FindAllTestsOnScene()) t.EnableTest(false);
        }

        public static bool AnyTestsOnScene()
        {
            return FindAllTestsOnScene().Any();
        }

        public static bool AnyDynamicTestForCurrentScene()
        {
#if UNITY_EDITOR
                return TestComponent.GetTypesWithHelpAttribute(EditorApplication.currentScene).Any();
#else
                return TestComponent.GetTypesWithHelpAttribute(Application.loadedLevelName).Any();
#endif
        }

        #endregion

        private sealed class NullTestComponentImpl : ITestComponent
        {
            public int CompareTo(ITestComponent other)
            {
                if (other == this) return 0;
                return -1;
            }

            public void EnableTest(bool enable)
            {
            }

            public bool IsTestGroup()
            {
                throw new NotImplementedException();
            }

            public GameObject gameObject { get; private set; }
            public string Name { get { return ""; } }

            public ITestComponent GetTestGroup()
            {
                return null;
            }

            public bool IsExceptionExpected(string exceptionType)
            {
                throw new NotImplementedException();
            }

            public bool ShouldSucceedOnException()
            {
                throw new NotImplementedException();
            }

            public double GetTimeout()
            {
                throw new NotImplementedException();
            }

            public bool IsIgnored()
            {
                throw new NotImplementedException();
            }

            public bool ShouldSucceedOnAssertions()
            {
                throw new NotImplementedException();
            }

            public bool IsExludedOnThisPlatform()
            {
                throw new NotImplementedException();
            }
        }

        public static IEnumerable<Type> GetTypesWithHelpAttribute(string sceneName)
        {
#if !UNITY_METRO
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                Type[] types = null;

                try
                {
                    types = assembly.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    Debug.LogError("Failed to load types from: " + assembly.FullName);
                    foreach (Exception loadEx in ex.LoaderExceptions)
                        Debug.LogException(loadEx);
                }

                if (types == null)
                    continue;

                foreach (Type type in types)
                {
                    var attributes = type.GetCustomAttributes(typeof(IntegrationTest.DynamicTestAttribute), true);
                    if (attributes.Length == 1)
                    {
                        var a = attributes.Single() as IntegrationTest.DynamicTestAttribute;
                        if (a.IncludeOnScene(sceneName)) yield return type;
                    }
                }
            }
#else   // if !UNITY_METRO
            yield break;
#endif  // if !UNITY_METRO
        }
    }
}
