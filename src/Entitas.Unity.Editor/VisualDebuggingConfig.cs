using System.Collections.Generic;
using DesperateDevs.Serialization;

namespace Entitas.Unity.Editor
{
    public class VisualDebuggingConfig : AbstractConfigurableConfig
    {
        const string SystemWarningThresholdKey = "Entitas.VisualDebugging.Unity.Editor.SystemWarningThreshold";
        const string DefaultInstanceCreatorFolderPathKey = "Entitas.VisualDebugging.Unity.Editor.DefaultInstanceCreatorFolderPath";
        const string TypeDrawerFolderPathKey = "Entitas.VisualDebugging.Unity.Editor.TypeDrawerFolderPath";

        public override Dictionary<string, string> DefaultProperties => new Dictionary<string, string>
        {
            {SystemWarningThresholdKey, "5"},
            {DefaultInstanceCreatorFolderPathKey, "Assets/Editor/DefaultInstanceCreator"},
            {TypeDrawerFolderPathKey, "Assets/Editor/TypeDrawer"}
        };

        public int SystemWarningThreshold
        {
            get => int.Parse(_preferences[SystemWarningThresholdKey]);
            set => _preferences[SystemWarningThresholdKey] = value.ToString();
        }

        public string DefaultInstanceCreatorFolderPath
        {
            get => _preferences[DefaultInstanceCreatorFolderPathKey];
            set => _preferences[DefaultInstanceCreatorFolderPathKey] = value;
        }

        public string TypeDrawerFolderPath
        {
            get => _preferences[TypeDrawerFolderPathKey];
            set => _preferences[TypeDrawerFolderPathKey] = value;
        }
    }
}
