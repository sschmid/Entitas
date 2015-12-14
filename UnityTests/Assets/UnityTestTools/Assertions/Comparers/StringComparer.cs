using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class StringComparer : ComparerBaseGeneric<string>
    {
        public enum CompareType
        {
            Equal,
            NotEqual,
            Shorter,
            Longer
        }

        public CompareType compareType;
        public StringComparison comparisonType = StringComparison.Ordinal;
        public bool ignoreCase = false;

        protected override bool Compare(string a, string b)
        {
            if (ignoreCase)
            {
                a = a.ToLower();
                b = b.ToLower();
            }
            switch (compareType)
            {
                case CompareType.Equal:
                    return String.Compare(a, b, comparisonType) == 0;
                case CompareType.NotEqual:
                    return String.Compare(a, b, comparisonType) != 0;
                case CompareType.Longer:
                    return String.Compare(a, b, comparisonType) > 0;
                case CompareType.Shorter:
                    return String.Compare(a, b, comparisonType) < 0;
            }
            throw new Exception();
        }
    }
}
