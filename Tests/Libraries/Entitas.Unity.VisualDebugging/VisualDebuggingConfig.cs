namespace Entitas.Unity.VisualDebugging {
    public class VisualDebuggingConfig {
        public string defaultInstanceCreatorFolderPath {
            get { return _config.GetValueOrDefault(DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY, DEFAULT_DEFAULT_INSTANCE_CREATOR_FOLDER_PATH); }
            set { _config[DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY] = value; }
        }

        public string typeDrawerFolderPath {
            get { return _config.GetValueOrDefault(TYPE_DRAWER_FOLDER_PATH_KEY, DEFAULT_TYPE_DRAWER_FOLDER_PATH); }
            set { _config[TYPE_DRAWER_FOLDER_PATH_KEY] = value; }
        }

        const string DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY = "Entitas.Unity.VisualDebugging.DefaultInstanceCreatorFolderPath";
        const string TYPE_DRAWER_FOLDER_PATH_KEY = "Entitas.Unity.VisualDebugging.TypeDrawerFolderPath";

        const string DEFAULT_DEFAULT_INSTANCE_CREATOR_FOLDER_PATH = "Assets/Editor/DefaultInstanceCreator/";
        const string DEFAULT_TYPE_DRAWER_FOLDER_PATH = "Assets/Editor/TypeDrawer/";

        readonly EntitasPreferencesConfig _config;

        public VisualDebuggingConfig(EntitasPreferencesConfig config) {
            _config = config;
            defaultInstanceCreatorFolderPath = defaultInstanceCreatorFolderPath;
            typeDrawerFolderPath = typeDrawerFolderPath;
        }

        public override string ToString() {
            return _config.ToString();
        }
    }
}

