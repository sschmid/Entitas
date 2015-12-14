using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Core;
using UnityEditor;
using UnityEngine;
using Event = UnityEngine.Event;

namespace UnityTest
{
    public class GroupLine : UnitTestRendererLine
    {
        public static List<string> FoldMarkers;

        protected static GUIContent s_GUIExpandAll = new GUIContent("Expand all");
        protected static GUIContent s_GUICollapseAll = new GUIContent("Collapse all");
        private readonly List<UnitTestRendererLine> m_Children = new List<UnitTestRendererLine>();

        public GroupLine(TestSuite suite)
            : base(suite)
        {
            if (suite is NamespaceSuite) m_RenderedName = m_FullName;
        }

        private bool Folded
        {
            get { return FoldMarkers.Contains(m_FullName); }

            set
            {
                if (value)
                    FoldMarkers.Add(m_FullName);
                else
                    FoldMarkers.RemoveAll(s => s == m_FullName);
            }
        }

        public void AddChildren(UnitTestRendererLine[] children)
        {
            m_Children.AddRange(children);
        }

        protected internal override void Render(int indend, RenderingOptions options)
        {
            if (!AnyVisibleChildren(options)) return;
            base.Render(indend, options);
            if (!Folded)
                foreach (var child in m_Children)
                    child.Render(indend + 1, options);
        }

        private bool AnyVisibleChildren(RenderingOptions options)
        {
            return m_Children.Any(l => l.IsVisible(options));
        }

        protected internal override bool IsVisible(RenderingOptions options)
        {
            return AnyVisibleChildren(options);
        }

        protected override void DrawLine(bool isSelected, RenderingOptions options)
        {
            var resultIcon = GetResult().HasValue ? GuiHelper.GetIconForResult(GetResult().Value) : Icons.UnknownImg;

            var guiContent = new GUIContent(m_RenderedName, resultIcon, m_FullName);

            var rect = GUILayoutUtility.GetRect(guiContent, Styles.foldout, GUILayout.MaxHeight(16));

            OnLeftMouseButtonClick(rect);
            OnContextClick(rect);

            EditorGUI.BeginChangeCheck();
            var expanded = !EditorGUI.Foldout(rect, !Folded, guiContent, false, isSelected ? Styles.selectedFoldout : Styles.foldout);
            if (EditorGUI.EndChangeCheck()) Folded = expanded;
        }

        protected internal override TestResultState ? GetResult()
        {
            TestResultState? tempResult = null;

            foreach (var child in m_Children)
            {
                var childResultState = child.GetResult();

                if (childResultState == TestResultState.Failure || childResultState == TestResultState.Error)
                {
                    tempResult = TestResultState.Failure;
                    break;
                }
                if (childResultState == TestResultState.Success)
                    tempResult = TestResultState.Success;
                else if (childResultState == TestResultState.Ignored)
                    tempResult = TestResultState.Ignored;
            }
            if (tempResult.HasValue) return tempResult.Value;

            return null;
        }

        private void OnLeftMouseButtonClick(Rect rect)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.mouseDown && Event.current.button == 0)
            {
                OnSelect();
            }
        }

        private void OnContextClick(Rect rect)
        {
            if (rect.Contains(Event.current.mousePosition) && Event.current.type == EventType.ContextClick)
            {
                PrintGroupContextMenu();
            }
        }

        private void PrintGroupContextMenu()
        {
            var multilineSelection = SelectedLines.Count() > 1;
            var m = new GenericMenu();
            if (multilineSelection)
            {
                m.AddItem(s_GUIRunSelected,
                          false,
                          data => RunTests(SelectedLines.Select(line => line.m_Test.TestName).ToArray()),
                          "");
            }
            if (!string.IsNullOrEmpty(m_FullName))
            {
                m.AddItem(s_GUIRun,
                          false,
                          data => RunTests(new[] { m_Test.TestName }),
                          "");
            }
            if (!multilineSelection)
            {
                m.AddSeparator("");

                m.AddItem(Folded ? s_GUIExpandAll : s_GUICollapseAll,
                          false,
                          data => ExpandOrCollapseAll(Folded),
                          "");
            }
            m.ShowAsContext();
        }

        private void ExpandOrCollapseAll(bool expand)
        {
            Folded = !expand;
            foreach (var child in m_Children)
            {
                if (child is GroupLine) (child as GroupLine).ExpandOrCollapseAll(expand);
            }
        }
    }
}
