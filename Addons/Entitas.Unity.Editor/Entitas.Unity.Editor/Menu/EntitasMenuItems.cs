using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor {

    public static class EntitasMenuItems {
        
        public const string preferences                      = "Tools/Entitas/Preferences... #%e";

        public const string check_for_updates                = "Tools/Entitas/Check for Updates...";

        public const string documentation                    = "Tools/Entitas/Documentation...";

        public const string feedback_report_a_bug            = "Tools/Entitas/Feedback/Report a bug...";
        public const string feedback_request_a_feature       = "Tools/Entitas/Feedback/Request a feature...";
        public const string feedback_join_the_entitas_chat   = "Tools/Entitas/Feedback/Join the Entitas chat...";
        public const string feedback_entitas_wiki            = "Tools/Entitas/Feedback/Entitas Wiki...";
        public const string feedback_donate                  = "Tools/Entitas/Feedback/Donate...";
    }

    public static class EntitasMenuItemPriorities {

        public const int preferences                         = 1;

        public const int check_for_updates                   = 10;

        public const int documentation                       = 11;

        public const int feedback_report_a_bug               = 20;
        public const int feedback_request_a_feature          = 21;
        public const int feedback_join_the_entitas_chat      = 22;
        public const int feedback_entitas_wiki               = 23;
        public const int feedback_donate                     = 24;
    }

    public static class EntitasFeedback {

        [MenuItem(EntitasMenuItems.documentation, false, EntitasMenuItemPriorities.documentation)]
        public static void EntitasDocs() {
            Application.OpenURL("http://sschmid.github.io/Entitas-CSharp/");
        }

        [MenuItem(EntitasMenuItems.feedback_report_a_bug, false, EntitasMenuItemPriorities.feedback_report_a_bug)]
        public static void ReportBug() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/issues");
        }

        [MenuItem(EntitasMenuItems.feedback_request_a_feature, false, EntitasMenuItemPriorities.feedback_request_a_feature)]
        public static void RequestFeature() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/issues");
        }

        [MenuItem(EntitasMenuItems.feedback_join_the_entitas_chat, false, EntitasMenuItemPriorities.feedback_join_the_entitas_chat)]
        public static void EntitasChat() {
            Application.OpenURL("https://gitter.im/sschmid/Entitas-CSharp");
        }

        [MenuItem(EntitasMenuItems.feedback_entitas_wiki, false, EntitasMenuItemPriorities.feedback_entitas_wiki)]
        public static void EntitasWiki() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/wiki");
        }

        [MenuItem(EntitasMenuItems.feedback_donate, false, EntitasMenuItemPriorities.feedback_donate)]
        public static void Donate() {
            Application.OpenURL("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852");
        }
    }
}
