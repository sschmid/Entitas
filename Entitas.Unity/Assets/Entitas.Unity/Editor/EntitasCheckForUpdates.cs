using System;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {
    public static class EntitasCheckForUpdates {

        const string url = "https://api.github.com/repos/sschmid/Entitas-CSharp/releases/latest";

        [MenuItem("Entitas/Check for updates...")]
        public static void CheckForUpdates() {
            var response = requestLatestRelease();
            var remoteVersion = parseVersion(response);
            var localVersion = getLocalVersion();

            displayUpdateInfo(remoteVersion, localVersion);
        }

        static string requestLatestRelease() {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.UserAgent = Application.dataPath + "/sschmid/Entitas-CSharp/Entitas.Unity/CheckForUpdates";
            httpWebRequest.Timeout = 15000;
            var webResponse = httpWebRequest.GetResponse();
            var response = string.Empty;
            using (var streamReader = new StreamReader(webResponse.GetResponseStream())) {
                response = streamReader.ReadToEnd();
            }
            return response;
        }

        static string parseVersion(string response) {
            const string versionPattern = @"(?<=""tag_name"":"").*?(?="")";
            return Regex.Match(response, versionPattern).Value;
        }

        static string getLocalVersion() {
            var files = Directory.GetFiles(Application.dataPath, "entitas_version", SearchOption.AllDirectories);
            if (files.Length != 1) {
                Debug.Log("Couldn't locate file entitas_version");
                return "0.0.0";
            }

            return File.ReadAllText(files[0]);
        }

        static void displayUpdateInfo(string remoteVersionString, string localVersionString) {
            var remoteVersion = new Version(remoteVersionString);
            var localVersion = new Version(localVersionString);

            switch (remoteVersion.CompareTo(localVersion)) {
                case 1:
                    if (EditorUtility.DisplayDialog("Entitas Update",
                            string.Format("A newer version of Entitas is available!\n\n" +
                            "Currently installed version: {0}\n" +
                            "New version: {1}", localVersion, remoteVersion),
                            "Show release",
                            "Cancel"
                        )) {
                        Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/releases");
                    }
                    break;
                case 0:
                    EditorUtility.DisplayDialog("Entitas Update",
                        "Entitas is up to date (" + localVersionString + ")",
                        "Ok"
                    );
                    break;
                case -1:
                    if (EditorUtility.DisplayDialog("Entitas Update",
                            string.Format("Your Entitas version seems to be newer than the latest release?!?\n\n" +
                            "Currently installed version: {0}\n" +
                            "Latest release: {1}", localVersion, remoteVersion),
                            "Show release",
                            "Cancel"
                        )) {
                        Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/releases");
                    }
                    break;
            }
        }
    }
}