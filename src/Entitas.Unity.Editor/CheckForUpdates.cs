using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Entitas.Unity.Editor
{
    public enum UpdateState
    {
        UpToDate,
        UpdateAvailable,
        AheadOfLatestRelease,
        NoConnection
    }

    public class UpdateInfo
    {
        public readonly UpdateState UpdateState;
        public readonly string LocalVersionString;
        public readonly string RemoteVersionString;

        public UpdateInfo(string localVersionString, string remoteVersionString)
        {
            LocalVersionString = localVersionString.Trim();
            RemoteVersionString = remoteVersionString.Trim();

            if (remoteVersionString != string.Empty)
            {
                var localVersion = new Version(localVersionString);
                var remoteVersion = new Version(remoteVersionString);

                switch (remoteVersion.CompareTo(localVersion))
                {
                    case 1:
                        UpdateState = UpdateState.UpdateAvailable;
                        break;
                    case 0:
                        UpdateState = UpdateState.UpToDate;
                        break;
                    case -1:
                        UpdateState = UpdateState.AheadOfLatestRelease;
                        break;
                }
            }
            else
            {
                UpdateState = UpdateState.NoConnection;
            }
        }
    }

    public static class CheckForUpdates
    {
        const string GithubAPILatestReleaseUrl = "https://api.github.com/repos/sschmid/Entitas/releases/latest";

        const string GithubReleasesUrl = "https://github.com/sschmid/Entitas/releases";
        // const string AssetStoreUrl = "http://u3d.as/NuJ";

        [MenuItem(EntitasMenuItems.check_for_updates, false, EntitasMenuItemPriorities.check_for_updates)]
        public static void DisplayUpdates() => DisplayUpdateInfo(GetUpdateInfo());

        public static UpdateInfo GetUpdateInfo() => new UpdateInfo(GetLocalVersion(), GetRemoteVersion());

        public static string GetLocalVersion() => EntitasResources.GetVersion();

        public static string GetRemoteVersion()
        {
            try
            {
                return JsonUtility.FromJson<ResponseData>(RequestLatestRelease()).tag_name;
            }
            catch (Exception)
            {
                // ignored
            }

            return string.Empty;
        }

        static string RequestLatestRelease()
        {
            var response = string.Empty;
            using (var www = UnityWebRequest.Get(GithubAPILatestReleaseUrl))
            {
                var asyncOperation = www.SendWebRequest();
                while (!asyncOperation.isDone) { }

                if (www.result != UnityWebRequest.Result.ConnectionError &&
                    www.result != UnityWebRequest.Result.ProtocolError)
                {
                    response = asyncOperation.webRequest.downloadHandler.text;
                }
            }

            return response;
        }

        static void DisplayUpdateInfo(UpdateInfo info)
        {
            switch (info.UpdateState)
            {
                case UpdateState.UpdateAvailable:
                    if (EditorUtility.DisplayDialog("Entitas Update",
                            $"A newer version of Entitas is available!\n\nCurrently installed version: {info.LocalVersionString}\nNew version: {info.RemoteVersionString}",
                            "Show Entitas GitHub releases",
                            "Cancel"))
                    {
                        Application.OpenURL(GithubReleasesUrl);
                    }

                    break;
                case UpdateState.UpToDate:
                    EditorUtility.DisplayDialog("Entitas Update",
                        $"Entitas is up to date ({info.LocalVersionString})",
                        "Ok");
                    break;
                case UpdateState.AheadOfLatestRelease:
                    if (EditorUtility.DisplayDialog("Entitas Update",
                            $"Your Entitas version seems to be newer than the latest release?!?\n\nCurrently installed version: {info.LocalVersionString}\nLatest release: {info.RemoteVersionString}",
                            "Show Entitas GitHub releases",
                            "Cancel"))
                    {
                        Application.OpenURL(GithubReleasesUrl);
                    }

                    break;
                case UpdateState.NoConnection:
                    if (EditorUtility.DisplayDialog("Entitas Update",
                            "Could not request latest Entitas version!\n\nMake sure that you are connected to the internet.\n",
                            "Try again",
                            "Cancel"
                        ))
                    {
                        DisplayUpdates();
                    }

                    break;
            }
        }

        // ReSharper disable once InconsistentNaming
        struct ResponseData
        {
#pragma warning disable CS0649
            public string tag_name;
#pragma warning restore CS0649
        }
    }
}
