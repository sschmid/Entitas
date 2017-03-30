namespace Entitas.VisualDebugging.Unity {

    public class VisualDebuggingConfig {

        public static readonly string[] keys = {
            SYSTEM_WARNING_THRESHOLD_KEY,
            DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY,
            TYPE_DRAWER_FOLDER_PATH_KEY
        };

        const string SYSTEM_WARNING_THRESHOLD_KEY = "Entitas.VisualDebugging.Unity.SystemWarningThreshold";
        const string DEFAULT_SYSTEM_WARNING_THRESHOLD = "8";
        public string systemWarningThreshold {
            get { return _config.GetValueOrDefault(SYSTEM_WARNING_THRESHOLD_KEY, DEFAULT_SYSTEM_WARNING_THRESHOLD); }
            set { _config[SYSTEM_WARNING_THRESHOLD_KEY] = value; }
        }

        const string DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY = "Entitas.VisualDebugging.Unity.DefaultInstanceCreatorFolderPath";
        const string DEFAULT_DEFAULT_INSTANCE_CREATOR_FOLDER_PATH = "Assets/Editor/DefaultInstanceCreator/";
        public string defaultInstanceCreatorFolderPath {
            get { return _config.GetValueOrDefault(DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY, DEFAULT_DEFAULT_INSTANCE_CREATOR_FOLDER_PATH); }
            set { _config[DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY] = value; }
        }

        const string TYPE_DRAWER_FOLDER_PATH_KEY = "Entitas.VisualDebugging.Unity.TypeDrawerFolderPath";
        const string DEFAULT_TYPE_DRAWER_FOLDER_PATH = "Assets/Editor/TypeDrawer/";
        public string typeDrawerFolderPath {
            get { return _config.GetValueOrDefault(TYPE_DRAWER_FOLDER_PATH_KEY, DEFAULT_TYPE_DRAWER_FOLDER_PATH); }
            set { _config[TYPE_DRAWER_FOLDER_PATH_KEY] = value; }
        }

        readonly EntitasPreferencesConfig _config;

        public VisualDebuggingConfig(EntitasPreferencesConfig config) {
            _config = config;
            systemWarningThreshold = systemWarningThreshold;
            defaultInstanceCreatorFolderPath = defaultInstanceCreatorFolderPath;
            typeDrawerFolderPath = typeDrawerFolderPath;
        }

        public override string ToString() {
            return _config.ToString();
        }
    }
}
