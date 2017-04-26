using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor {

    public enum UpdateState {
        UpToDate,
        UpdateAvailable,
        AheadOfLatestRelease,
        NoConnection
    }

    public class UpdateInfo {

        public UpdateState updateState { get { return _updateState; } }

        public readonly string localVersionString;
        public readonly string remoteVersionString;

        readonly UpdateState _updateState;

        public UpdateInfo(string localVersionString, string remoteVersionString) {
            this.localVersionString = localVersionString.Trim();
            this.remoteVersionString = remoteVersionString.Trim();

            if (remoteVersionString != string.Empty) {
                var localVersion = new Version(localVersionString);
                var remoteVersion = new Version(remoteVersionString);

                switch (remoteVersion.CompareTo(localVersion)) {
                    case 1:
                        _updateState = UpdateState.UpdateAvailable;
                        break;
                    case 0:
                        _updateState = UpdateState.UpToDate;
                        break;
                    case -1:
                        _updateState = UpdateState.AheadOfLatestRelease;
                        break;
                }
            } else {
                _updateState = UpdateState.NoConnection;
            }
        }
    }

    public static class CheckForUpdates {

        const string URL_GITHUB_API_LATEST_RELEASE = "https://api.github.com/repos/sschmid/Entitas-CSharp/releases/latest";
        const string URL_GITHUB_RELEASES = "https://github.com/sschmid/Entitas-CSharp/releases";
        const string URL_ASSET_STORE = "https://www.assetstore.unity3d.com/#!/content/87638";

        [MenuItem(EntitasMenuItems.check_for_updates, false, EntitasMenuItemPriorities.check_for_updates)]
        public static void DisplayUpdates() {
            var info = GetUpdateInfo();
            displayUpdateInfo(info);
        }

        public static UpdateInfo GetUpdateInfo() {
            var localVersion = GetLocalVersion();
            var remoteVersion = GetRemoteVersion();
            return new UpdateInfo(localVersion, remoteVersion);
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

        static void displayUpdateInfo(UpdateInfo info) {
            switch (info.updateState) {
                case UpdateState.UpdateAvailable:
                    if (EditorUtility.DisplayDialog("Entitas Update",
                            string.Format("A newer version of Entitas is available!\n\n" +
                            "Currently installed version: {0}\n" +
                            "New version: {1}", info.localVersionString, info.remoteVersionString),
                            "Show in Unity Asset Store",
                            "Cancel"
                        )) {
                        Application.OpenURL(URL_ASSET_STORE);
                    }
                    break;
                case UpdateState.UpToDate:
                    EditorUtility.DisplayDialog("Entitas Update",
                        "Entitas is up to date (" + info.localVersionString + ")",
                        "Ok"
                    );
                    break;
                case UpdateState.AheadOfLatestRelease:
                    if (EditorUtility.DisplayDialog("Entitas Update",
                            string.Format("Your Entitas version seems to be newer than the latest release?!?\n\n" +
                            "Currently installed version: {0}\n" +
                            "Latest release: {1}", info.localVersionString, info.remoteVersionString),
                            "Show in Unity Asset Store",
                            "Cancel"
                        )) {
                        Application.OpenURL(URL_ASSET_STORE);
                    }
                    break;
                case UpdateState.NoConnection:
                    if (EditorUtility.DisplayDialog("Entitas Update",
                            "Could not request latest Entitas version!\n\n" +
                            "Make sure that you are connected to the internet.\n",
                            "Try again",
                            "Cancel"
                        )) {
                        DisplayUpdates();
                    }
                    break;
            }
        }

        static bool trustSource(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
            return true;
        }
    }
}
