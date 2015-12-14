using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

namespace UnityTest
{
    public class TestFilterSettings
    {
        public bool ShowSucceeded;
        public bool ShowFailed;
        public bool ShowIgnored;
        public bool ShowNotRun;
        
        public string FilterByName;
        public int FilterByCategory;
        
        private GUIContent _succeededBtn;
        private GUIContent _failedBtn;
        private GUIContent _ignoredBtn;
        private GUIContent _notRunBtn;
        
        public string[] AvailableCategories;
        
        private readonly string _prefsKey;
        
        public TestFilterSettings(string prefsKey)
        {
            _prefsKey = prefsKey;
            Load();
            UpdateCounters(Enumerable.Empty<ITestResult>());
        }
            
        public void Load()
        {
            ShowSucceeded = EditorPrefs.GetBool(_prefsKey + ".ShowSucceeded", true);
            ShowFailed = EditorPrefs.GetBool(_prefsKey + ".ShowFailed", true);
            ShowIgnored = EditorPrefs.GetBool(_prefsKey + ".ShowIgnored", true);
            ShowNotRun = EditorPrefs.GetBool(_prefsKey + ".ShowNotRun", true);
            FilterByName = EditorPrefs.GetString(_prefsKey + ".FilterByName", string.Empty);
            FilterByCategory = EditorPrefs.GetInt(_prefsKey + ".FilterByCategory", 0);
        }
        
        public void Save()
        {
            EditorPrefs.SetBool(_prefsKey + ".ShowSucceeded", ShowSucceeded);
            EditorPrefs.SetBool(_prefsKey + ".ShowFailed", ShowFailed);
            EditorPrefs.SetBool(_prefsKey + ".ShowIgnored", ShowIgnored);
            EditorPrefs.SetBool(_prefsKey + ".ShowNotRun", ShowNotRun);
            EditorPrefs.SetString(_prefsKey + ".FilterByName", FilterByName);
            EditorPrefs.SetInt(_prefsKey + ".FilterByCategory", FilterByCategory);
        }
        
        public void UpdateCounters(IEnumerable<ITestResult> results)
        {
            var summary = new ResultSummarizer(results);
            
            _succeededBtn = new GUIContent(summary.Passed.ToString(), Icons.SuccessImg, "Show tests that succeeded");
            _failedBtn = new GUIContent((summary.Errors + summary.Failures + summary.Inconclusive).ToString(), Icons.FailImg, "Show tests that failed");
            _ignoredBtn = new GUIContent((summary.Ignored + summary.NotRunnable).ToString(), Icons.IgnoreImg, "Show tests that are ignored");
            _notRunBtn = new GUIContent((summary.TestsNotRun - summary.Ignored - summary.NotRunnable).ToString(), Icons.UnknownImg, "Show tests that didn't run");
        }
        
        public string[] GetSelectedCategories()
        {
            if(AvailableCategories == null) return new string[0];
            
            return AvailableCategories.Where ((c, i) => (FilterByCategory & (1 << i)) != 0).ToArray();
        }
        
        public void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            
            FilterByName = GUILayout.TextField(FilterByName, "ToolbarSeachTextField", GUILayout.MinWidth(100), GUILayout.MaxWidth(250), GUILayout.ExpandWidth(true));
            if(GUILayout.Button (GUIContent.none, string.IsNullOrEmpty(FilterByName) ? "ToolbarSeachCancelButtonEmpty" : "ToolbarSeachCancelButton"))
                FilterByName = string.Empty;
            
            if (AvailableCategories != null && AvailableCategories.Length > 0)
                FilterByCategory = EditorGUILayout.MaskField(FilterByCategory, AvailableCategories, EditorStyles.toolbarDropDown, GUILayout.MaxWidth(90));
            
            ShowSucceeded = GUILayout.Toggle(ShowSucceeded, _succeededBtn, EditorStyles.toolbarButton);
            ShowFailed = GUILayout.Toggle(ShowFailed, _failedBtn, EditorStyles.toolbarButton);
            ShowIgnored = GUILayout.Toggle(ShowIgnored, _ignoredBtn, EditorStyles.toolbarButton);
            ShowNotRun = GUILayout.Toggle(ShowNotRun, _notRunBtn, EditorStyles.toolbarButton);
            
            if(EditorGUI.EndChangeCheck()) Save ();
        }
        
        public RenderingOptions BuildRenderingOptions()
        {
            var options = new RenderingOptions();
            options.showSucceeded = ShowSucceeded;
            options.showFailed = ShowFailed;
            options.showIgnored = ShowIgnored;
            options.showNotRunned = ShowNotRun;
            options.nameFilter = FilterByName;
            options.categories = GetSelectedCategories();
            return options;
        }
    }
    
}
