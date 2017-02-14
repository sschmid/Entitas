namespace Entitas.Unity {

    public static class EntitasMenuItems {
        
        public const string preferences                      = "Entitas/Preferences... #%e";

        public const string check_for_updates                = "Entitas/Check for Updates...";

        public const string feedback_report_a_bug            = "Entitas/Feedback/Report a bug...";
        public const string feedback_request_a_feature       = "Entitas/Feedback/Request a feature...";
        public const string feedback_join_the_entitas_chat   = "Entitas/Feedback/Join the Entitas chat...";
        public const string feedback_entitas_wiki            = "Entitas/Feedback/Entitas wiki...";
        public const string feedback_donate                  = "Entitas/Feedback/Donate...";

        public const string log_stats                        = "Entitas/Log Stats";
    }

    public static class EntitasMenuItemPriorities {

        public const int preferences                         = 1;

        public const int check_for_updates                   = 10;

        public const int feedback_report_a_bug               = 20;
        public const int feedback_request_a_feature          = 21;
        public const int feedback_join_the_entitas_chat      = 22;
        public const int feedback_entitas_wiki               = 23;
        public const int feedback_donate                     = 24;

        public const int log_stats                           = 100;
    }
}
