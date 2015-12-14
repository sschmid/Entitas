using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityTest
{
    public class GroupByExecutionMethodRenderer : AssertionListRenderer<CheckMethod>
    {
        protected override IEnumerable<IGrouping<CheckMethod, AssertionComponent>> GroupResult(IEnumerable<AssertionComponent> assertionComponents)
        {
            var enumVals = Enum.GetValues(typeof(CheckMethod)).Cast<CheckMethod>();
            var pairs = new List<CheckFunctionAssertionPair>();

            foreach (var checkMethod in enumVals)
            {
                var components = assertionComponents.Where(c => (c.checkMethods & checkMethod) == checkMethod);
                var componentPairs = components.Select(a => new CheckFunctionAssertionPair {checkMethod = checkMethod, assertionComponent = a});
                pairs.AddRange(componentPairs);
            }
            return pairs.GroupBy(pair => pair.checkMethod,
                                 pair => pair.assertionComponent);
        }

        private class CheckFunctionAssertionPair
        {
            public AssertionComponent assertionComponent;
            public CheckMethod checkMethod;
        }
    }
}
