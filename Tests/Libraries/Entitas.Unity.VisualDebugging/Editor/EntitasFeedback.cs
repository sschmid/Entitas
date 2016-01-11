using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public static class EntitasFeedback {

        [MenuItem("Entitas/Feedback/Report a bug...", false, 201)]
        public static void ReportBug() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/issues");
        }

        [MenuItem("Entitas/Feedback/Request a feature...", false, 202)]
        public static void RequestFeature() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/issues");
        }

        [MenuItem("Entitas/Feedback/Join the Entitas chat...", false, 203)]
        public static void EntitasChat() {
            Application.OpenURL("https://gitter.im/sschmid/Entitas-CSharp");
        }

        [MenuItem("Entitas/Feedback/Entitas wiki...", false, 204)]
        public static void EntitasWiki() {
            Application.OpenURL("https://github.com/sschmid/Entitas-CSharp/wiki");
        }
    }
}

