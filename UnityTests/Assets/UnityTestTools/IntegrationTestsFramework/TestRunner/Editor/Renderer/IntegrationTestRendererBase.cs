using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityTest
{
    public abstract class IntegrationTestRendererBase : IComparable<IntegrationTestRendererBase>
    {
        public static Action<IList<ITestComponent>> RunTest;

        protected static bool s_Refresh;

        private static readonly GUIContent k_GUIRunSelected = new GUIContent("Run Selected");
        private static readonly GUIContent k_GUIRun = new GUIContent("Run");
        private static readonly GUIContent k_GUIDelete = new GUIContent("Delete");
        private static readonly GUIContent k_GUIDeleteSelected = new GUIContent("Delete selected");

        protected static GUIContent s_GUITimeoutIcon = new GUIContent(Icons.StopwatchImg, "Timeout");

        protected GameObject m_GameObject;
        public TestComponent test;
        private readonly string m_Name;

        protected IntegrationTestRendererBase(GameObject gameObject)
        {
            test = gameObject.GetComponent(typeof(TestComponent)) as TestComponent;
            if (test == null) throw new ArgumentException("Provided GameObject is not a test object");
            m_GameObject = gameObject;
            m_Name = test.Name;
        }

        public int CompareTo(IntegrationTestRendererBase other)
        {
            return test.CompareTo(other.test);
        }

        public bool Render(RenderingOptions options)
        {
            s_Refresh = false;
            EditorGUIUtility.SetIconSize(new Vector2(15, 15));
            Render(0, options);
            EditorGUIUtility.SetIconSize(Vector2.zero);
            return s_Refresh;
        }

        protected internal virtual void Render(int indend, RenderingOptions options)
        {
            if (!IsVisible(options)) return;
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indend * 10);

            var tempColor = GUI.color;
            if (m_IsRunning)
            {
                var frame = Mathf.Abs(Mathf.Cos(Time.realtimeSinceStartup * 4)) * 0.6f + 0.4f;
                GUI.color = new Color(1, 1, 1, frame);
            }

            var isSelected = Selection.gameObjects.Contains(m_GameObject);

            var value = GetResult();
            var icon = GetIconForResult(value);

            var label = new GUIContent(m_Name, icon);
            var labelRect = GUILayoutUtility.GetRect(label, EditorStyles.label, GUILayout.ExpandWidth(true), GUILayout.Height(18));

            OnLeftMouseButtonClick(labelRect);
            OnContextClick(labelRect);
            DrawLine(labelRect, label, isSelected, options);

            if (m_IsRunning) GUI.color = tempColor;
            EditorGUILayout.EndHorizontal();
        }

        protected void OnSelect()
        {
			if (!Event.current.control && !Event.current.command) 
			{
				Selection.objects = new Object[0];
				GUIUtility.keyboardControl = 0;
			}

			if ((Event.current.control || Event.current.command) && Selection.gameObjects.Contains(test.gameObject))
                Selection.objects = Selection.gameObjects.Where(o => o != test.gameObject).ToArray();
            else
                Selection.objects = Selection.gameObjects.Concat(new[] { test.gameObject }).ToArray();
        }

        protected void OnLeftMouseButtonClick(Rect rect)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown && Event.current.button == 0)
            {
                rect.width = 20;
                if (rect.Contains(Event.current.mousePosition)) return;
                Event.current.Use();
                OnSelect();
            }
        }

        protected void OnContextClick(Rect rect)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
            {
                DrawContextMenu(test);
            }
        }

        public static void DrawContextMenu(TestComponent testComponent)
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;

            var selectedTests = Selection.gameObjects.Where(go => go.GetComponent(typeof(TestComponent)));
            var manySelected = selectedTests.Count() > 1;

            var m = new GenericMenu();
            if (manySelected)
            {
                // var testsToRun
                m.AddItem(k_GUIRunSelected, false, data => RunTest(selectedTests.Select(o => o.GetComponent(typeof(TestComponent))).Cast<ITestComponent>().ToList()), null);
            }
            m.AddItem(k_GUIRun, false, data => RunTest(new[] { testComponent }), null);
            m.AddSeparator("");
            m.AddItem(manySelected ? k_GUIDeleteSelected : k_GUIDelete, false, data => RemoveTests(selectedTests.ToArray()), null);
            m.ShowAsContext();
        }

        private static void RemoveTests(GameObject[] testsToDelete)
        {
            foreach (var t in testsToDelete)
            {
                Undo.DestroyObjectImmediate(t);
            }
        }

        public static Texture GetIconForResult(TestResult.ResultType resultState)
        {
            switch (resultState)
            {
                case TestResult.ResultType.Success:
                    return Icons.SuccessImg;
                case TestResult.ResultType.Timeout:
                case TestResult.ResultType.Failed:
                case TestResult.ResultType.FailedException:
                    return Icons.FailImg;
                case TestResult.ResultType.Ignored:
                    return Icons.IgnoreImg;
                default:
                    return Icons.UnknownImg;
            }
        }

        protected internal bool m_IsRunning;
        protected internal abstract void DrawLine(Rect rect, GUIContent label, bool isSelected, RenderingOptions options);
        protected internal abstract TestResult.ResultType GetResult();
        protected internal abstract bool IsVisible(RenderingOptions options);
        public abstract bool SetCurrentTest(TestComponent tc);
    }
}
