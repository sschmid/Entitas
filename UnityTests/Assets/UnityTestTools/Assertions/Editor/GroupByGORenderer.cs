using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    public class GroupByGoRenderer : AssertionListRenderer<GameObject>
    {
        protected override IEnumerable<IGrouping<GameObject, AssertionComponent>> GroupResult(IEnumerable<AssertionComponent> assertionComponents)
        {
            return assertionComponents.GroupBy(c => c.gameObject);
        }

        protected override bool PrintFoldout(bool isFolded, GameObject key)
        {
            isFolded = base.PrintFoldout(isFolded,
                                         key);

            EditorGUILayout.ObjectField(key,
                                        typeof(GameObject),
                                        true,
                                        GUILayout.ExpandWidth(false));

            return isFolded;
        }

        protected override string GetFoldoutDisplayName(GameObject key)
        {
            return key.name;
        }
    }
}
