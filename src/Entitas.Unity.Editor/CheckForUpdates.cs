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
        public UpdateState UpdateState => _updateState;

        public readonly string LocalVersionString;
        public readonly string RemoteVersionString;

        readonly UpdateState _updateState;

        public UpdateInfo(string localVersionString, string remoteVersionString)
        {
            LocalVersionString = localVersionString.Trim();
            RemoteVersionString = remoteVersionString.Trim();

            if (remoteVersionString != string.Empty)
            {
                _updateState = new Version(remoteVersionString).CompareTo(new Version(localVersionString)) switch
                {
                    1 => UpdateState.UpdateAvailable,
                    0 => UpdateState.UpToDate,
                    -1 => UpdateState.AheadOfLatestRelease,
                    _ => _updateState
                };
            }
            else
            {
                _updateState = UpdateState.NoConnection;
            }
        }
    }

    public static class CheckForUpdates
    {
        const string URLGitHubAPILatestRelease = "https://api.github.com/repos/sschmid/Entitas/releases/latest";
        const string URLGitHubReleases = "https://github.com/sschmid/Entitas/releases";
        const string URLAssetStore = "http://u3d.as/NuJ";

        [MenuItem(EntitasMenuItems.CheckForUpdates, false, EntitasMenuItemPriorities.CheckForUpdates)]
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
            using (var www = UnityWebRequest.Get(URLGitHubAPILatestRelease))
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
                            "Show in Unity Asset Store",
                            "Cancel"))
                    {
                        Application.OpenURL(URLAssetStore);
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
                            "Show in Unity Asset Store",
                            "Cancel"))
                    {
                        Application.OpenURL(URLAssetStore);
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

        struct ResponseData
        {
            public string tag_name;
        }
    }
}
