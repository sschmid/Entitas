using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    [CustomEditor(typeof(AssertionComponent))]
    public class AssertionComponentEditor : Editor
    {
        private readonly DropDownControl<Type> m_ComparerDropDown = new DropDownControl<Type>();

        private readonly PropertyPathSelector m_ThisPathSelector = new PropertyPathSelector("Compare");
        private readonly PropertyPathSelector m_OtherPathSelector = new PropertyPathSelector("Compare to");

        #region GUI Contents
        private readonly GUIContent m_GUICheckAfterTimeGuiContent = new GUIContent("Check after (seconds)", "After how many seconds the assertion should be checked");
        private readonly GUIContent m_GUIRepeatCheckTimeGuiContent = new GUIContent("Repeat check", "Should the check be repeated.");
        private readonly GUIContent m_GUIRepeatEveryTimeGuiContent = new GUIContent("Frequency of repetitions", "How often should the check be done");
        private readonly GUIContent m_GUICheckAfterFramesGuiContent = new GUIContent("Check after (frames)", "After how many frames the assertion should be checked");
        private readonly GUIContent m_GUIRepeatCheckFrameGuiContent = new GUIContent("Repeat check", "Should the check be repeated.");
        #endregion

		private static List<Type> allComparersList = null;

        public AssertionComponentEditor()
        {
            m_ComparerDropDown.convertForButtonLabel = type => type.Name;
            m_ComparerDropDown.convertForGUIContent = type => type.Name;
            m_ComparerDropDown.ignoreConvertForGUIContent = types => false;
            m_ComparerDropDown.tooltip = "Comparer that will be used to compare values and determine the result of assertion.";
        }

        public override void OnInspectorGUI()
        {
            var script = (AssertionComponent)target;
            EditorGUILayout.BeginHorizontal();
            var obj = DrawComparerSelection(script);
            script.checkMethods = (CheckMethod)EditorGUILayout.EnumMaskField(script.checkMethods,
                                                                             EditorStyles.popup,
                                                                             GUILayout.ExpandWidth(false));
            EditorGUILayout.EndHorizontal();

            if (script.IsCheckMethodSelected(CheckMethod.AfterPeriodOfTime))
            {
                DrawOptionsForAfterPeriodOfTime(script);
            }

            if (script.IsCheckMethodSelected(CheckMethod.Update))
            {
                DrawOptionsForOnUpdate(script);
            }

            if (obj)
            {
                EditorGUILayout.Space();

                m_ThisPathSelector.Draw(script.Action.go, script.Action,
                                        script.Action.thisPropertyPath, script.Action.GetAccepatbleTypesForA(),
                                        go =>
                                        {
                                            script.Action.go = go;
                                            AssertionExplorerWindow.Reload();
                                        },
                                        s =>
                                        {
                                            script.Action.thisPropertyPath = s;
                                            AssertionExplorerWindow.Reload();
                                        });

                EditorGUILayout.Space();

                DrawCustomFields(script);

                EditorGUILayout.Space();

                if (script.Action is ComparerBase)
                {
                    DrawCompareToType(script.Action as ComparerBase);
                }
            }
        }

        private void DrawOptionsForAfterPeriodOfTime(AssertionComponent script)
        {
            EditorGUILayout.Space();
            script.checkAfterTime = EditorGUILayout.FloatField(m_GUICheckAfterTimeGuiContent,
                                                               script.checkAfterTime);
            if (script.checkAfterTime < 0)
                script.checkAfterTime = 0;
            script.repeatCheckTime = EditorGUILayout.Toggle(m_GUIRepeatCheckTimeGuiContent,
                                                            script.repeatCheckTime);
            if (script.repeatCheckTime)
            {
                script.repeatEveryTime = EditorGUILayout.FloatField(m_GUIRepeatEveryTimeGuiContent,
                                                                    script.repeatEveryTime);
                if (script.repeatEveryTime < 0)
                    script.repeatEveryTime = 0;
            }
        }

        private void DrawOptionsForOnUpdate(AssertionComponent script)
        {
            EditorGUILayout.Space();
            script.checkAfterFrames = EditorGUILayout.IntField(m_GUICheckAfterFramesGuiContent,
                                                               script.checkAfterFrames);
            if (script.checkAfterFrames < 1)
                script.checkAfterFrames = 1;
            script.repeatCheckFrame = EditorGUILayout.Toggle(m_GUIRepeatCheckFrameGuiContent,
                                                             script.repeatCheckFrame);
            if (script.repeatCheckFrame)
            {
                script.repeatEveryFrame = EditorGUILayout.IntField(m_GUIRepeatEveryTimeGuiContent,
                                                                   script.repeatEveryFrame);
                if (script.repeatEveryFrame < 1)
                    script.repeatEveryFrame = 1;
            }
        }

        private void DrawCompareToType(ComparerBase comparer)
        {
            comparer.compareToType = (ComparerBase.CompareToType)EditorGUILayout.EnumPopup("Compare to type",
                                                                                           comparer.compareToType,
                                                                                           EditorStyles.popup);

            if (comparer.compareToType == ComparerBase.CompareToType.CompareToConstantValue)
            {
                try
                {
                    DrawConstCompareField(comparer);
                }
                catch (NotImplementedException)
                {
                    Debug.LogWarning("This comparer can't compare to static value");
                    comparer.compareToType = ComparerBase.CompareToType.CompareToObject;
                }
            }
            else if (comparer.compareToType == ComparerBase.CompareToType.CompareToObject)
            {
                DrawObjectCompareField(comparer);
            }
        }

        private void DrawObjectCompareField(ComparerBase comparer)
        {
            m_OtherPathSelector.Draw(comparer.other, comparer,
                                     comparer.otherPropertyPath, comparer.GetAccepatbleTypesForB(),
                                     go =>
                                     {
                                         comparer.other = go;
                                         AssertionExplorerWindow.Reload();
                                     },
                                     s =>
                                     {
                                         comparer.otherPropertyPath = s;
                                         AssertionExplorerWindow.Reload();
                                     }
                                     );
        }

        private void DrawConstCompareField(ComparerBase comparer)
        {
            if (comparer.ConstValue == null)
            {
                comparer.ConstValue = comparer.GetDefaultConstValue();
            }

            var so = new SerializedObject(comparer);
            var sp = so.FindProperty("constantValueGeneric");
            if (sp != null)
            {
                EditorGUILayout.PropertyField(sp, new GUIContent("Constant"), true);
                so.ApplyModifiedProperties();
            }
        }

        private bool DrawComparerSelection(AssertionComponent script)
        {
            if(allComparersList == null)
            {
                allComparersList = new List<Type>();
                var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in allAssemblies)
                {
                    var types = assembly.GetTypes();
                    allComparersList.AddRange(types.Where(type => type.IsSubclassOf(typeof(ActionBase)) && !type.IsAbstract));
                }
            }
            var allComparers = allComparersList.ToArray();

            if (script.Action == null)
                script.Action = (ActionBase)CreateInstance(allComparers.First());

            m_ComparerDropDown.Draw(script.Action.GetType(), allComparers,
                                    type =>
                                    {
                                        if (script.Action == null || script.Action.GetType().Name != type.Name)
                                        {
                                            script.Action = (ActionBase)CreateInstance(type);
                                            AssertionExplorerWindow.Reload();
                                        }
                                    });

            return script.Action != null;
        }

        private void DrawCustomFields(AssertionComponent script)
        {
            foreach (var prop in script.Action.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
            {
                var so = new SerializedObject(script.Action);
                var sp = so.FindProperty(prop.Name);
                if (sp != null)
                {
                    EditorGUILayout.PropertyField(sp, true);
                    so.ApplyModifiedProperties();
                }
            }
        }
    }
}
