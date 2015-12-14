using System;
using UnityEngine;

namespace UnityTest
{
    public class PlatformRunnerSettings : ProjectSettingsBase
    {
        public string resultsPath;
        public bool sendResultsOverNetwork = true;
        public int port = 0;
    }
}
