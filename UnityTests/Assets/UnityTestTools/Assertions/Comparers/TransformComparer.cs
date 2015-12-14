using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class TransformComparer : ComparerBaseGeneric<Transform>
    {
        public enum CompareType { Equals, NotEquals }

        public CompareType compareType;

        protected override bool Compare(Transform a, Transform b)
        {
            if (compareType == CompareType.Equals)
            {
                return a.position == b.position;
            }
            if (compareType == CompareType.NotEquals)
            {
                return a.position != b.position;
            }
            throw new Exception();
        }
    }
}
