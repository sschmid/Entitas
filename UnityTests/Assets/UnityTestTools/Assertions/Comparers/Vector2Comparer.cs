using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class Vector2Comparer : VectorComparerBase<Vector2>
    {
        public enum CompareType
        {
            MagnitudeEquals,
            MagnitudeNotEquals
        }

        public CompareType compareType;
        public float floatingPointError = 0.0001f;

        protected override bool Compare(Vector2 a, Vector2 b)
        {
            switch (compareType)
            {
                case CompareType.MagnitudeEquals:
                    return AreVectorMagnitudeEqual(a.magnitude,
                                                   b.magnitude, floatingPointError);
                case CompareType.MagnitudeNotEquals:
                    return !AreVectorMagnitudeEqual(a.magnitude,
                                                    b.magnitude, floatingPointError);
            }
            throw new Exception();
        }
        public override int GetDepthOfSearch()
        {
            return 3;
        }
    }
}
