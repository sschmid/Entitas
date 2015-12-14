using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public class AssertionException : Exception
    {
        private readonly AssertionComponent m_Assertion;

        public AssertionException(AssertionComponent assertion) : base(assertion.Action.GetFailureMessage())
        {
            m_Assertion = assertion;
        }

        public override string StackTrace
        {
            get
            {
                return "Created in " + m_Assertion.GetCreationLocation();
            }
        }
    }
}
