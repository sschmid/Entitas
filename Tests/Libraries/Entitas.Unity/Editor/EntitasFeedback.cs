using UnityEditor;
using UnityEngine;

namespace Entitas.Unity {
    public static class EntitasFeedback {

        [MenuItem("Entitas/Feedback/Report a bug...", false, 3)]
        public static void ReportBug() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/issues");
        }

        [MenuItem("Entitas/Feedback/Request a feature...", false, 4)]
        public static void RequestFeature() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/issues");
        }

        [MenuItem("Entitas/Feedback/Join the Entitas chat...", false, 5)]
        public static void EntitasChat() {
            Application.OpenURL("https://gitter.im/sschmid/Entitas-CSharp");
        }

        [MenuItem("Entitas/Feedback/Entitas wiki...", false, 6)]
        public static void EntitasWiki() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/wiki");
        }

        [MenuItem("Entitas/Feedback/Donate...", false, 7)]
        public static void Donate() {
            Application.OpenURL("https://www.paypal.com/cgi-bin/webscr?cmd=_s-xclick&hosted_button_id=Y9HGYPFMLG2W4");
        }
    }
}

