using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{
    public enum TestResultState : byte
    {
        Inconclusive = 0,

        /// <summary>
        /// The test was not runnable.
        /// </summary>
        NotRunnable = 1,

        /// <summary>
        /// The test has been skipped.
        /// </summary>
        Skipped = 2,

        /// <summary>
        /// The test has been ignored.
        /// </summary>
        Ignored = 3,

        /// <summary>
        /// The test succeeded
        /// </summary>
        Success = 4,

        /// <summary>
        /// The test failed
        /// </summary>
        Failure = 5,

        /// <summary>
        /// The test encountered an unexpected exception
        /// </summary>
        Error = 6,

        /// <summary>
        /// The test was cancelled by the user
        /// </summary>
        Cancelled = 7
    }
}
