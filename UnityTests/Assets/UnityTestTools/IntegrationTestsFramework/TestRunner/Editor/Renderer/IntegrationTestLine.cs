using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    class IntegrationTestLine : IntegrationTestRendererBase
    {
        public static List<TestResult> Results;
        protected TestResult m_Result;

        public IntegrationTestLine(GameObject gameObject, TestResult testResult) : base(gameObject)
        {
            m_Result = testResult;
        }

        protected internal override void DrawLine(Rect rect, GUIContent label, bool isSelected, RenderingOptions options)
        {
            if(Event.current.type != EventType.repaint)
				return;

			Styles.testName.Draw (rect, label, false, false, false, isSelected);

            if (m_Result.IsTimeout)
            {
				float min, max;
				Styles.testName.CalcMinMaxWidth(label, out min, out max);
				var timeoutRect = new Rect(rect);
				timeoutRect.x += min - 12;
				Styles.testName.Draw(timeoutRect, s_GUITimeoutIcon, false, false, false, isSelected);
            }
        }

        protected internal override TestResult.ResultType GetResult()
        {
            if (!m_Result.Executed && test.ignored) return TestResult.ResultType.Ignored;
            return m_Result.resultType;
        }

        protected internal override bool IsVisible(RenderingOptions options)
        {
            if (!string.IsNullOrEmpty(options.nameFilter) && !m_GameObject.name.ToLower().Contains(options.nameFilter.ToLower())) return false;
            if (!options.showSucceeded && m_Result.IsSuccess) return false;
            if (!options.showFailed && m_Result.IsFailure) return false;
            if (!options.showNotRunned && !m_Result.Executed) return false;
            if (!options.showIgnored && test.ignored) return false;
            return true;
        }

        public override bool SetCurrentTest(TestComponent tc)
        {
            m_IsRunning = test == tc;
            return m_IsRunning;
        }
    }
}
