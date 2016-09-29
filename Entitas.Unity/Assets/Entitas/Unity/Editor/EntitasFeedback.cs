using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {

    public static class EntitasFeedback {

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
