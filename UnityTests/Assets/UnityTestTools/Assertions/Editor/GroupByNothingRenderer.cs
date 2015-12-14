using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTest
{
    public class GroupByNothingRenderer : AssertionListRenderer<string>
    {
        protected override IEnumerable<IGrouping<string, AssertionComponent>> GroupResult(IEnumerable<AssertionComponent> assertionComponents)
        {
            return assertionComponents.GroupBy(c => "");
        }

        protected override string GetStringKey(string key)
        {
            return "";
        }
    }
}
