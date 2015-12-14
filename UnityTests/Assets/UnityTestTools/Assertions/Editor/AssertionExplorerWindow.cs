using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

#if UNITY_METRO
#warning Assertion component is not supported on Windows Store apps
#endif

namespace UnityTest
{
    [Serializable]
    public class AssertionExplorerWindow : EditorWindow
    {
        private List<AssertionComponent> m_AllAssertions = new List<AssertionComponent>();
        [SerializeField]
        private string m_FilterText = "";
        [SerializeField]
        private FilterType m_FilterType;
        [SerializeField]
        private List<string> m_FoldMarkers = new List<string>();
        [SerializeField]
        private GroupByType m_GroupBy;
        [SerializeField]
        private Vector2 m_ScrollPosition = Vector2.zero;
        private DateTime m_NextReload = DateTime.Now;
        [SerializeField]
        private static bool s_ShouldReload;
        [SerializeField]
        private ShowType m_ShowType;

        public AssertionExplorerWindow()
        {
            titleContent = new GUIContent("Assertion Explorer");
        }

        public void OnDidOpenScene()
        {
            ReloadAssertionList();
        }

        public void OnFocus()
        {
            ReloadAssertionList();
        }

        private void ReloadAssertionList()
        {
            m_NextReload = DateTime.Now.AddSeconds(1);
            s_ShouldReload = true;
        }

        public void OnHierarchyChange()
        {
            ReloadAssertionList();
        }

        public void OnInspectorUpdate()
        {
            if (s_ShouldReload && m_NextReload < DateTime.Now)
            {
                s_ShouldReload = false;
                m_AllAssertions = new List<AssertionComponent>((AssertionComponent[])Resources.FindObjectsOfTypeAll(typeof(AssertionComponent)));
                Repaint();
            }
        }

        public void OnGUI()
        {
            DrawMenuPanel();

            m_ScrollPosition = EditorGUILayout.BeginScrollView(m_ScrollPosition);
            if (m_AllAssertions != null)
                GetResultRendere().Render(FilterResults(m_AllAssertions, m_FilterText.ToLower()), m_FoldMarkers);
            EditorGUILayout.EndScrollView();
        }

        private IEnumerable<AssertionComponent> FilterResults(List<AssertionComponent> assertionComponents, string text)
        {
            if (m_ShowType == ShowType.ShowDisabled)
                assertionComponents = assertionComponents.Where(c => !c.enabled).ToList();
            else if (m_ShowType == ShowType.ShowEnabled)
                assertionComponents = assertionComponents.Where(c => c.enabled).ToList();

            if (string.IsNullOrEmpty(text))
                return assertionComponents;

            switch (m_FilterType)
            {
                case FilterType.ComparerName:
                    return assertionComponents.Where(c => c.Action.GetType().Name.ToLower().Contains(text));
                case FilterType.AttachedGameObject:
                    return assertionComponents.Where(c => c.gameObject.name.ToLower().Contains(text));
                case FilterType.FirstComparedGameObjectPath:
                    return assertionComponents.Where(c => c.Action.thisPropertyPath.ToLower().Contains(text));
                case FilterType.FirstComparedGameObject:
                    return assertionComponents.Where(c => c.Action.go != null
                                                     && c.Action.go.name.ToLower().Contains(text));
                case FilterType.SecondComparedGameObjectPath:
                    return assertionComponents.Where(c =>
                                                     c.Action is ComparerBase
                                                     && (c.Action as ComparerBase).otherPropertyPath.ToLower().Contains(text));
                case FilterType.SecondComparedGameObject:
                    return assertionComponents.Where(c =>
                                                     c.Action is ComparerBase
                                                     && (c.Action as ComparerBase).other != null
                                                     && (c.Action as ComparerBase).other.name.ToLower().Contains(text));
                default:
                    return assertionComponents;
            }
        }

        private readonly IListRenderer m_GroupByComparerRenderer = new GroupByComparerRenderer();
        private readonly IListRenderer m_GroupByExecutionMethodRenderer = new GroupByExecutionMethodRenderer();
        private readonly IListRenderer m_GroupByGoRenderer = new GroupByGoRenderer();
        private readonly IListRenderer m_GroupByTestsRenderer = new GroupByTestsRenderer();
        private readonly IListRenderer m_GroupByNothingRenderer = new GroupByNothingRenderer();

        private IListRenderer GetResultRendere()
        {
            switch (m_GroupBy)
            {
                case GroupByType.Comparer:
                    return m_GroupByComparerRenderer;
                case GroupByType.ExecutionMethod:
                    return m_GroupByExecutionMethodRenderer;
                case GroupByType.GameObjects:
                    return m_GroupByGoRenderer;
                case GroupByType.Tests:
                    return m_GroupByTestsRenderer;
                default:
                    return m_GroupByNothingRenderer;
            }
        }

        private void DrawMenuPanel()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            EditorGUILayout.LabelField("Group by:", Styles.toolbarLabel, GUILayout.MaxWidth(60));
            m_GroupBy = (GroupByType)EditorGUILayout.EnumPopup(m_GroupBy, EditorStyles.toolbarPopup, GUILayout.MaxWidth(150));

            GUILayout.FlexibleSpace();

            m_ShowType = (ShowType)EditorGUILayout.EnumPopup(m_ShowType, EditorStyles.toolbarPopup, GUILayout.MaxWidth(100));

            EditorGUILayout.LabelField("Filter by:", Styles.toolbarLabel, GUILayout.MaxWidth(50));
            m_FilterType = (FilterType)EditorGUILayout.EnumPopup(m_FilterType, EditorStyles.toolbarPopup, GUILayout.MaxWidth(100));
            m_FilterText = GUILayout.TextField(m_FilterText, "ToolbarSeachTextField", GUILayout.MaxWidth(100));
            if (GUILayout.Button(GUIContent.none, string.IsNullOrEmpty(m_FilterText) ? "ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton", GUILayout.ExpandWidth(false)))
                m_FilterText = "";
            EditorGUILayout.EndHorizontal();
        }

        [MenuItem("Unity Test Tools/Assertion Explorer")]
        public static AssertionExplorerWindow ShowWindow()
        {
            var w = GetWindow(typeof(AssertionExplorerWindow));
            w.Show();
            return w as AssertionExplorerWindow;
        }

        private enum FilterType
        {
            ComparerName,
            FirstComparedGameObject,
            FirstComparedGameObjectPath,
            SecondComparedGameObject,
            SecondComparedGameObjectPath,
            AttachedGameObject
        }

        private enum ShowType
        {
            ShowAll,
            ShowEnabled,
            ShowDisabled
        }

        private enum GroupByType
        {
            Nothing,
            Comparer,
            GameObjects,
            ExecutionMethod,
            Tests
        }

        public static void Reload()
        {
            s_ShouldReload = true;
        }
    }
}
