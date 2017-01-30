namespace Entitas.Unity {

    public static class EntitasMenuItems {
        
        public const string preferences                      = "Entitas/Preferences... #%e";

        public const string check_for_updates                = "Entitas/Check for Updates...";

        public const string feedback_report_a_bug            = "Entitas/Feedback/Report a bug...";
        public const string feedback_request_a_feature       = "Entitas/Feedback/Request a feature...";
        public const string feedback_join_the_entitas_chat   = "Entitas/Feedback/Join the Entitas chat...";
        public const string feedback_entitas_wiki            = "Entitas/Feedback/Entitas wiki...";
        public const string feedback_donate                  = "Entitas/Feedback/Donate...";

        public const string generate                         = "Entitas/Generate #%g";

        public const string log_stats                        = "Entitas/Log Stats";

		public const string blueprints_update_all_blueprints = "Entitas/Blueprints/Update all Blueprints";

        public const string migrate                          = "Entitas/Migrate...";
    }

    public static class EntitasMenuItemPriorities {

        public const int preferences                         = 1;

        public const int check_for_updates                   = 10;

        public const int feedback_report_a_bug               = 20;
        public const int feedback_request_a_feature          = 21;
        public const int feedback_join_the_entitas_chat      = 22;
        public const int feedback_entitas_wiki               = 23;
        public const int feedback_donate                     = 24;

        public const int generate                            = 100;

        public const int log_stats                           = 200;

        public const int blueprints_update_all_blueprints    = 300;

        public const int migrate                             = 1000;
    }
}
