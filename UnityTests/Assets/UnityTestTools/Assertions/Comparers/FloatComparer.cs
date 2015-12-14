using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class FloatComparer : ComparerBaseGeneric<float>
    {
        public enum CompareTypes
        {
            Equal,
            NotEqual,
            Greater,
            Less
        }

        public CompareTypes compareTypes;
        public double floatingPointError = 0.0001f;

        protected override bool Compare(float a, float b)
        {
            switch (compareTypes)
            {
                case CompareTypes.Equal:
                    return Math.Abs(a - b) < floatingPointError;
                case CompareTypes.NotEqual:
                    return Math.Abs(a - b) > floatingPointError;
                case CompareTypes.Greater:
                    return a > b;
                case CompareTypes.Less:
                    return a < b;
            }
            throw new Exception();
        }
        public override int GetDepthOfSearch()
        {
            return 3;
        }
    }
}
