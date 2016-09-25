using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {

    public static class EntitasFeedback {

        [MenuItem("Entitas/Feedback/Report a bug...", false, EntitasMenuItemPriorities.feedback_report_a_bug)]
        public static void ReportBug() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/issues");
        }

        [MenuItem("Entitas/Feedback/Request a feature...", false, EntitasMenuItemPriorities.feedback_request_a_feature)]
        public static void RequestFeature() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/issues");
        }

        [MenuItem("Entitas/Feedback/Join the Entitas chat...", false, EntitasMenuItemPriorities.feedback_join_the_entitas_chat)]
        public static void EntitasChat() {
            Application.OpenURL("https://gitter.im/sschmid/Entitas-CSharp");
        }

        [MenuItem("Entitas/Feedback/Entitas wiki...", false, EntitasMenuItemPriorities.feedback_entitas_wiki)]
        public static void EntitasWiki() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/wiki");
        }

        [MenuItem("Entitas/Feedback/Donate...", false, EntitasMenuItemPriorities.feedback__donate)]
        public static void Donate() {
            Application.OpenURL("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=BTMLSDQULZ852");
        }
    }
}

