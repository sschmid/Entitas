using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor {

    public enum EntitasUpdateState {
        UpToDate,
        UpdateAvailable,
        AheadOfLatestRelease,
        NoConnection
    }

    public class EntitasUpdateInfo {

        public EntitasUpdateState updateState { get { return _updateState; } }

        public readonly string localVersionString;
        public readonly string remoteVersionString;

        readonly EntitasUpdateState _updateState;

        public EntitasUpdateInfo(string localVersionString, string remoteVersionString) {
            this.localVersionString = localVersionString.Trim();
            this.remoteVersionString = remoteVersionString.Trim();

            if(remoteVersionString != string.Empty) {
                var localVersion = new Version(localVersionString);
                var remoteVersion = new Version(remoteVersionString);

                switch(remoteVersion.CompareTo(localVersion)) {
                    case 1:
                        _updateState = EntitasUpdateState.UpdateAvailable;
                        break;
                    case 0:
                        _updateState = EntitasUpdateState.UpToDate;
                        break;
                    case -1:
                        _updateState = EntitasUpdateState.AheadOfLatestRelease;
                        break;
                }
            } else {
                _updateState = EntitasUpdateState.NoConnection;
            }
        }
    }

    public static class EntitasCheckForUpdates {

        const string URL_GITHUB_API_LATEST_RELEASE = "https://api.github.com/repos/sschmid/Entitas-CSharp/releases/latest";
        const string URL_GITHUB_RELEASES = "https://github.com/sschmid/Entitas-CSharp/releases";

        [MenuItem(EntitasMenuItems.check_for_updates, false, EntitasMenuItemPriorities.check_for_updates)]
        public static void CheckForUpdates() {
            var info = GetUpdateInfo();
            displayUpdateInfo(info);
        }

        public static EntitasUpdateInfo GetUpdateInfo() {
            var localVersion = GetLocalVersion();
            var remoteVersion = GetRemoteVersion();
            return new EntitasUpdateInfo(localVersion, remoteVersion);
        }

        public static string GetLocalVersion() {
            return EntitasResources.GetVersion();
        }

        public static string GetRemoteVersion() {
            string latestRelease = null;
            try {
                latestRelease = requestLatestRelease();
            } catch(Exception) {
                latestRelease = string.Empty;
            }

            return parseVersion(latestRelease);
        }

        static string requestLatestRelease() {
            ServicePointManager.ServerCertificateValidationCallback += trustSource;
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL_GITHUB_API_LATEST_RELEASE);
            httpWebRequest.UserAgent = Environment.UserName + "sschmid/Entitas-CSharp/Entitas.Unity/CheckForUpdates";
            httpWebRequest.Timeout = 15000;
            var webResponse = httpWebRequest.GetResponse();
            ServicePointManager.ServerCertificateValidationCallback -= trustSource;
            var response = string.Empty;
            using(var streamReader = new StreamReader(webResponse.GetResponseStream())) {
                response = streamReader.ReadToEnd();
            }
            return response;
        }

        static string parseVersion(string response) {
            const string versionPattern = @"(?<=""tag_name"":"").*?(?="")";
            return Regex.Match(response, versionPattern).Value;
        }

        static void displayUpdateInfo(EntitasUpdateInfo info) {
            switch(info.updateState) {
                case EntitasUpdateState.UpdateAvailable:
                    if(EditorUtility.DisplayDialog("Entitas Update",
                            string.Format("A newer version of Entitas is available!\n\n" +
                            "Currently installed version: {0}\n" +
                            "New version: {1}", info.localVersionString, info.remoteVersionString),
                            "Show release",
                            "Cancel"
                        )) {
                        Application.OpenURL(URL_GITHUB_RELEASES);
                    }
                    break;
                case EntitasUpdateState.UpToDate:
                    EditorUtility.DisplayDialog("Entitas Update",
                        "Entitas is up to date (" + info.localVersionString + ")",
                        "Ok"
                    );
                    break;
                case EntitasUpdateState.AheadOfLatestRelease:
                    if(EditorUtility.DisplayDialog("Entitas Update",
                            string.Format("Your Entitas version seems to be newer than the latest release?!?\n\n" +
                            "Currently installed version: {0}\n" +
                            "Latest release: {1}", info.localVersionString, info.remoteVersionString),
                            "Show release",
                            "Cancel"
                        )) {
                        Application.OpenURL(URL_GITHUB_RELEASES);
                    }
                    break;
                case EntitasUpdateState.NoConnection:
                    if(EditorUtility.DisplayDialog("Entitas Update",
                            "Could not request latest Entitas version!\n\n" +
                            "Make sure that you are connected to the internet.\n",
                            "Try again",
                            "Cancel"
                        )) {
                        CheckForUpdates();
                    }
                    break;
            }
        }

        static bool trustSource(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        }
    }
}
