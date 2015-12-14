using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace UnityTest
{
    class PlayerSettingConfigurator
    {
        private string resourcesPath {
            get { return m_Temp ? k_TempPath : m_ProjectResourcesPath; }
        }

        private readonly string m_ProjectResourcesPath = Path.Combine("Assets", "Resources");
        const string k_TempPath = "Temp";
        private readonly bool m_Temp;

        private ResolutionDialogSetting m_DisplayResolutionDialog;
        private bool m_RunInBackground;
        private bool m_FullScreen;
        private bool m_ResizableWindow;
        private readonly List<string> m_TempFileList = new List<string>();

        public PlayerSettingConfigurator(bool saveInTempFolder)
        {
            m_Temp = saveInTempFolder;
        }

        public void ChangeSettingsForIntegrationTests()
        {
            m_DisplayResolutionDialog = PlayerSettings.displayResolutionDialog;
            PlayerSettings.displayResolutionDialog = ResolutionDialogSetting.Disabled;

            m_RunInBackground = PlayerSettings.runInBackground;
            PlayerSettings.runInBackground = true;

            m_FullScreen = PlayerSettings.defaultIsFullScreen;
            PlayerSettings.defaultIsFullScreen = false;

            m_ResizableWindow = PlayerSettings.resizableWindow;
            PlayerSettings.resizableWindow = true;
        }

        public void RevertSettingsChanges()
        {
            PlayerSettings.defaultIsFullScreen = m_FullScreen;
            PlayerSettings.runInBackground = m_RunInBackground;
            PlayerSettings.displayResolutionDialog = m_DisplayResolutionDialog;
            PlayerSettings.resizableWindow = m_ResizableWindow;
        }

        public void AddConfigurationFile(string fileName, string content)
        {
            var resourcesPathExists = Directory.Exists(resourcesPath);
            if (!resourcesPathExists) AssetDatabase.CreateFolder("Assets", "Resources");

            var filePath = Path.Combine(resourcesPath, fileName);
            File.WriteAllText(filePath, content);

            m_TempFileList.Add(filePath);
        }

        public void RemoveAllConfigurationFiles()
        {
            foreach (var filePath in m_TempFileList)
                AssetDatabase.DeleteAsset(filePath);
            if (Directory.Exists(resourcesPath)
                && Directory.GetFiles(resourcesPath).Length == 0)
                AssetDatabase.DeleteAsset(resourcesPath);
        }
    }
}
