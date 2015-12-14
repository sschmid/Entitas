using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public abstract class VectorComparerBase<T> : ComparerBaseGeneric<T>
    {
        protected bool AreVectorMagnitudeEqual(float a, float b, double floatingPointError)
        {
            if (Math.Abs(a) < floatingPointError && Math.Abs(b) < floatingPointError)
                return true;
            if (Math.Abs(a - b) < floatingPointError)
                return true;
            return false;
        }
    }
}
