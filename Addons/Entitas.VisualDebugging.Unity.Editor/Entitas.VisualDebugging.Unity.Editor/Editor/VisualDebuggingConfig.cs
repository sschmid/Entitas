using System.Collections.Generic;
using Entitas.Utils;

namespace Entitas.VisualDebugging.Unity.Editor {

    public class VisualDebuggingConfig : AbstractConfigurableConfig {

        const string SYSTEM_WARNING_THRESHOLD_KEY = "Entitas.VisualDebugging.Unity.Editor.SystemWarningThreshold";
        const string DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY = "Entitas.VisualDebugging.Unity.Editor.DefaultInstanceCreatorFolderPath";
        const string TYPE_DRAWER_FOLDER_PATH_KEY = "Entitas.VisualDebugging.Unity.Editor.TypeDrawerFolderPath";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { SYSTEM_WARNING_THRESHOLD_KEY, "5" },
                    { DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY, "Assets/Editor/DefaultInstanceCreator" },
                    { TYPE_DRAWER_FOLDER_PATH_KEY, "Assets/Editor/TypeDrawer" }
                };
            }
        }

        public int systemWarningThreshold { 
            get { return int.Parse(properties[SYSTEM_WARNING_THRESHOLD_KEY]); }
            set { properties[SYSTEM_WARNING_THRESHOLD_KEY] = value.ToString(); }
        }

        public string defaultInstanceCreatorFolderPath { 
            get { return properties[DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY]; }
            set { properties[DEFAULT_INSTANCE_CREATOR_FOLDER_PATH_KEY] = value; }
        }

        public string typeDrawerFolderPath { 
            get { return properties[TYPE_DRAWER_FOLDER_PATH_KEY]; }
            set { properties[TYPE_DRAWER_FOLDER_PATH_KEY] = value; }
        }
    }
}
