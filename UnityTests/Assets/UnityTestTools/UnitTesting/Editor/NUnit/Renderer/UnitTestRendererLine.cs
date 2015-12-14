using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Core;
using UnityEditor;
using UnityEngine;
using Event = UnityEngine.Event;

namespace UnityTest
{
    public abstract class UnitTestRendererLine : IComparable<UnitTestRendererLine>
    {
        public static Action<TestFilter> RunTest;
        public static List<UnitTestRendererLine> SelectedLines;

        protected static bool s_Refresh;

        protected static GUIContent s_GUIRunSelected = new GUIContent("Run Selected");
        protected static GUIContent s_GUIRun = new GUIContent("Run");
        protected static GUIContent s_GUITimeoutIcon = new GUIContent(Icons.StopwatchImg, "Timeout");

        protected string m_UniqueId;
        protected internal string m_FullName;
        protected string m_RenderedName;
        protected internal Test m_Test;

        protected UnitTestRendererLine(Test test)
        {
            m_FullName = test.TestName.FullName;
            m_RenderedName = test.TestName.Name;
            m_UniqueId = test.TestName.UniqueName;

            m_Test = test;
        }

        public int CompareTo(UnitTestRendererLine other)
        {
            return m_UniqueId.CompareTo(other.m_UniqueId);
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
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(indend * 10);
            DrawLine(SelectedLines.Contains(this), options);
            EditorGUILayout.EndHorizontal();
        }

        protected void OnSelect()
        {
			if (!Event.current.control && !Event.current.command)
			{
				SelectedLines.Clear();
				GUIUtility.keyboardControl = 0;
			}
			if ((Event.current.control || Event.current.command) && SelectedLines.Contains(this))
                SelectedLines.Remove(this);
            else
                SelectedLines.Add(this);
            s_Refresh = true;
        }

        protected abstract void DrawLine(bool isSelected, RenderingOptions options);
        protected internal abstract TestResultState ? GetResult();
        protected internal abstract bool IsVisible(RenderingOptions options);

        public void RunTests(object[] testObjectsList)
        {
            RunTest(new TestFilter { objects = testObjectsList });
        }

        public void RunTests(string[] testList)
        {
            RunTest(new TestFilter {names = testList});
        }

        public void RunSelectedTests()
        {
            RunTest(new TestFilter { objects = SelectedLines.Select(line => line.m_Test.TestName).ToArray() });
        }
        
        public bool IsAnySelected
        {
            get 
            {
                return SelectedLines.Count > 0;
            }
        }

        public virtual string GetResultText()
        {
            return m_RenderedName;
        }
    }
}
