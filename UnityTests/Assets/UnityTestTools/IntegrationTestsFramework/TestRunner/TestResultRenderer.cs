using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TestResultRenderer
{
    private static class Styles
    {
        public static readonly GUIStyle SucceedLabelStyle;
        public static readonly GUIStyle FailedLabelStyle;
        public static readonly GUIStyle FailedMessagesStyle;

        static Styles()
        {
            SucceedLabelStyle = new GUIStyle("label");
            SucceedLabelStyle.normal.textColor = Color.green;
            SucceedLabelStyle.fontSize = 48;

            FailedLabelStyle = new GUIStyle("label");
            FailedLabelStyle.normal.textColor = Color.red;
            FailedLabelStyle.fontSize = 32;

            FailedMessagesStyle = new GUIStyle("label");
            FailedMessagesStyle.wordWrap = false;
            FailedMessagesStyle.richText = true;
        }
    }
    private readonly Dictionary<string, List<ITestResult>> m_TestCollection = new Dictionary<string, List<ITestResult>>();

    private bool m_ShowResults;
    Vector2 m_ScrollPosition;
    private int m_FailureCount;

    public void ShowResults()
    {
        m_ShowResults = true;
        Cursor.visible = true;
    }

    public void AddResults(string sceneName, ITestResult result)
    {
        if (!m_TestCollection.ContainsKey(sceneName))
            m_TestCollection.Add(sceneName, new List<ITestResult>());
        m_TestCollection[sceneName].Add(result);
        if (result.Executed && !result.IsSuccess)
            m_FailureCount++;
    }

    public void Draw()
    {
        if (!m_ShowResults) return;
        if (m_TestCollection.Count == 0)
        {
            GUILayout.Label("All test succeeded", Styles.SucceedLabelStyle, GUILayout.Width(600));
        }
        else
        {
            int count = m_TestCollection.Sum (testGroup => testGroup.Value.Count);
            GUILayout.Label(count + " tests failed!", Styles.FailedLabelStyle);

            m_ScrollPosition = GUILayout.BeginScrollView(m_ScrollPosition, GUILayout.ExpandWidth(true));
            var text = "";
            foreach (var testGroup in m_TestCollection)
            {
                text += "<b><size=18>" + testGroup.Key + "</size></b>\n";
                text += string.Join("\n", testGroup.Value
                                    .Where(result => !result.IsSuccess)
                                    .Select(result => result.Name + " " + result.ResultState + "\n" + result.Message)
                                    .ToArray());
            }
            GUILayout.TextArea(text, Styles.FailedMessagesStyle);
            GUILayout.EndScrollView();
        }
        if (GUILayout.Button("Close"))
            Application.Quit();
    }

    public int FailureCount()
    {
        return m_FailureCount;
    }
}
