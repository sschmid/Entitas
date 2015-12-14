using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityTest
{

    public class UnitTestsRunnerSettings : ProjectSettingsBase
    {
        public bool runOnRecompilation;
        public bool horizontalSplit = true;
        public bool autoSaveSceneBeforeRun;
        public bool runTestOnANewScene;

        public void ToggleRunTestOnANewScene() {
            runTestOnANewScene = !runTestOnANewScene;
            Save ();
        }
        
        public void ToggleAutoSaveSceneBeforeRun() {
            autoSaveSceneBeforeRun = !autoSaveSceneBeforeRun;
            Save ();
        }
        
        public void ToggleHorizontalSplit() {
            horizontalSplit = !horizontalSplit;
            Save ();
        }
    }
}
