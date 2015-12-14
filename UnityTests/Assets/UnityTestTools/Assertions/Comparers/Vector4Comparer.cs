using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class Vector4Comparer : VectorComparerBase<Vector4>
    {
        public enum CompareType
        {
            MagnitudeEquals,
            MagnitudeNotEquals
        }

        public CompareType compareType;
        public double floatingPointError;

        protected override bool Compare(Vector4 a, Vector4 b)
        {
            switch (compareType)
            {
                case CompareType.MagnitudeEquals:
                    return AreVectorMagnitudeEqual(a.magnitude,
                                                   b.magnitude,
                                                   floatingPointError);
                case CompareType.MagnitudeNotEquals:
                    return !AreVectorMagnitudeEqual(a.magnitude,
                                                    b.magnitude,
                                                    floatingPointError);
            }
            throw new Exception();
        }
        public override int GetDepthOfSearch()
        {
            return 3;
        }
    }
}
