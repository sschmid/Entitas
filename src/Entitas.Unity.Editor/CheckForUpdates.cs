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
        public UpdateState updateState => _updateState;

        public readonly string localVersionString;
        public readonly string remoteVersionString;

        readonly UpdateState _updateState;

        public UpdateInfo(string localVersionString, string remoteVersionString)
        {
            this.localVersionString = localVersionString.Trim();
            this.remoteVersionString = remoteVersionString.Trim();

            if (remoteVersionString != string.Empty)
            {
                var localVersion = new Version(localVersionString);
                var remoteVersion = new Version(remoteVersionString);

                switch (remoteVersion.CompareTo(localVersion))
                {
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
            }
            else
            {
                _updateState = UpdateState.NoConnection;
            }
        }
    }

    public static class CheckForUpdates
    {
        const string URL_GITHUB_API_LATEST_RELEASE = "https://api.github.com/repos/sschmid/Entitas/releases/latest";
        const string URL_GITHUB_RELEASES = "https://github.com/sschmid/Entitas/releases";
        const string URL_ASSET_STORE = "http://u3d.as/NuJ";

        [MenuItem(EntitasMenuItems.check_for_updates, false, EntitasMenuItemPriorities.check_for_updates)]
        public static void DisplayUpdates() => displayUpdateInfo(GetUpdateInfo());

        public static UpdateInfo GetUpdateInfo() => new UpdateInfo(GetLocalVersion(), GetRemoteVersion());

        public static string GetLocalVersion() => EntitasResources.GetVersion();

        public static string GetRemoteVersion()
        {
            try
            {
                return JsonUtility.FromJson<ResponseData>(requestLatestRelease()).tag_name;
            }
            catch (Exception)
            {
                // ignored
            }

            return string.Empty;
        }

        static string requestLatestRelease()
        {
            var response = string.Empty;
            using (var www = UnityWebRequest.Get(URL_GITHUB_API_LATEST_RELEASE))
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

        static void displayUpdateInfo(UpdateInfo info)
        {
            switch (info.updateState)
            {
                case UpdateState.UpdateAvailable:
                    if (EditorUtility.DisplayDialog("Entitas Update",
                            $"A newer version of Entitas is available!\n\nCurrently installed version: {info.localVersionString}\nNew version: {info.remoteVersionString}",
                            "Show in Unity Asset Store",
                            "Cancel"))
                    {
                        Application.OpenURL(URL_ASSET_STORE);
                    }

                    break;
                case UpdateState.UpToDate:
                    EditorUtility.DisplayDialog("Entitas Update",
                        $"Entitas is up to date ({info.localVersionString})",
                        "Ok");
                    break;
                case UpdateState.AheadOfLatestRelease:
                    if (EditorUtility.DisplayDialog("Entitas Update",
                            $"Your Entitas version seems to be newer than the latest release?!?\n\nCurrently installed version: {info.localVersionString}\nLatest release: {info.remoteVersionString}",
                            "Show in Unity Asset Store",
                            "Cancel"))
                    {
                        Application.OpenURL(URL_ASSET_STORE);
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
#pragma warning disable CS0649
            public string tag_name;
#pragma warning restore CS0649
        }
    }
}
