using UnityEngine;
using System.Collections;
using UnityEditor;

namespace UnityTest
{

    [InitializeOnLoad]
    public class IntegrationTestsHierarchyAnnotation {
    
        static IntegrationTestsHierarchyAnnotation()
        {
            EditorApplication.hierarchyWindowItemOnGUI += DoAnnotationGUI;
        }
    
        public static void DoAnnotationGUI(int id, Rect rect)
        {
            var obj = EditorUtility.InstanceIDToObject(id) as GameObject;
            if(!obj) return;
            
            var tc = obj.GetComponent<TestComponent>();
            if(!tc) return;
            
            if (!EditorApplication.isPlayingOrWillChangePlaymode
                && rect.Contains(Event.current.mousePosition)
                && Event.current.type == EventType.MouseDown
                && Event.current.button == 1)
            {
                IntegrationTestRendererBase.DrawContextMenu(tc);
                Event.current.Use ();
            }
            
            EditorGUIUtility.SetIconSize(new Vector2(15, 15));
            var result = IntegrationTestsRunnerWindow.GetResultForTest(tc);
            if (result != null)
            {
                var icon = result.Executed ? IntegrationTestRendererBase.GetIconForResult(result.resultType) : Icons.UnknownImg;
                EditorGUI.LabelField(new Rect(rect.xMax - 18, rect.yMin - 2, rect.width, rect.height), new GUIContent(icon));
            }
            EditorGUIUtility.SetIconSize(Vector2.zero);
        }
    }

}