using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class GeneralComparer : ComparerBase
    {
        public enum CompareType { AEqualsB, ANotEqualsB }

        public CompareType compareType;

        protected override bool Compare(object a, object b)
        {
            if (compareType == CompareType.AEqualsB)
                return a.Equals(b);
            if (compareType == CompareType.ANotEqualsB)
                return !a.Equals(b);
            throw new Exception();
        }
    }
}
