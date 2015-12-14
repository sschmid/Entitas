using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTest
{
    public class GroupByTestsRenderer : AssertionListRenderer<GameObject>
    {
        protected override IEnumerable<IGrouping<GameObject, AssertionComponent>> GroupResult(IEnumerable<AssertionComponent> assertionComponents)
        {
            return assertionComponents.GroupBy(c =>
                                               {
                                                   var temp = c.transform;
                                                   while (temp != null)
                                                   {
                                                       if (temp.GetComponent("TestComponent") != null) return c.gameObject;
                                                       temp = temp.parent.transform;
                                                   }
                                                   return null;
                                               });
        }

        protected override string GetFoldoutDisplayName(GameObject key)
        {
            return key.name;
        }
    }
}
