using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public static class EntitasMenuItems
    {
        public const string Preferences = "Tools/Entitas/Preferences... #%e";

        public const string CheckForUpdates = "Tools/Entitas/Check for Updates...";

        public const string FeedbackReportABug = "Tools/Entitas/Feedback/Report a bug...";
        public const string FeedbackRequestAFeature = "Tools/Entitas/Feedback/Request a feature...";
        public const string FeedbackJoinTheEntitasChat = "Tools/Entitas/Feedback/Join the Entitas chat...";
        public const string FeedbackEntitasWiki = "Tools/Entitas/Feedback/Entitas Wiki...";
        public const string FeedbackDonate = "Tools/Entitas/Feedback/Donate...";
    }

    public static class EntitasMenuItemPriorities
    {
        public const int Preferences = 1;

        public const int CheckForUpdates = 10;

        public const int FeedbackReportABug = 20;
        public const int FeedbackRequestAFeature = 21;
        public const int FeedbackJoinTheEntitasChat = 22;
        public const int FeedbackEntitasWiki = 23;
        public const int FeedbackDonate = 24;
    }

    public static class EntitasFeedback
    {
        [MenuItem(EntitasMenuItems.FeedbackReportABug, false, EntitasMenuItemPriorities.FeedbackReportABug)]
        public static void ReportBug() => Application.OpenURL("https://github.com/sschmid/Entitas/issues");

        [MenuItem(EntitasMenuItems.FeedbackRequestAFeature, false, EntitasMenuItemPriorities.FeedbackRequestAFeature)]
        public static void RequestFeature() => Application.OpenURL("https://github.com/sschmid/Entitas/issues");

        [MenuItem(EntitasMenuItems.FeedbackJoinTheEntitasChat, false, EntitasMenuItemPriorities.FeedbackJoinTheEntitasChat)]
        public static void EntitasChat() => Application.OpenURL("https://discord.gg/uHrVx5Z");

        [MenuItem(EntitasMenuItems.FeedbackEntitasWiki, false, EntitasMenuItemPriorities.FeedbackEntitasWiki)]
        public static void EntitasWiki() => Application.OpenURL("https://github.com/sschmid/Entitas/wiki");

        [MenuItem(EntitasMenuItems.FeedbackDonate, false, EntitasMenuItemPriorities.FeedbackDonate)]
        public static void Donate() => Application.OpenURL("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852");
    }
}
