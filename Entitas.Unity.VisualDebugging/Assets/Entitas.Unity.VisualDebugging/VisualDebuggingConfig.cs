namespace Entitas.Unity.VisualDebugging {
    public class VisualDebuggingConfig {
        public string defaultInstanceCreatorFolderPath {
            get { return _config.GetValueOrDefault(defaultInstanceCreatorFolderPathKey, defaultDefaultInstanceCreatorFolderPath); }
            set { _config[defaultInstanceCreatorFolderPathKey] = value; }
        }

        public string typeDrawerFolderPath {
            get { return _config.GetValueOrDefault(typeDrawerFolderPathKey, defaultTypeDrawerFolderPath); }
            set { _config[typeDrawerFolderPathKey] = value; }
        }

        const string defaultInstanceCreatorFolderPathKey = "Entitas.Unity.VisualDebugging.DefaultInstanceCreatorFolderPath";
        const string typeDrawerFolderPathKey = "Entitas.Unity.VisualDebugging.TypeDrawerFolderPath";

        const string defaultDefaultInstanceCreatorFolderPath = "Assets/Editor/DefaultInstanceCreator/";
        const string defaultTypeDrawerFolderPath = "Assets/Editor/TypeDrawer/";

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

