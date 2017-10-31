using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public abstract class AbstractCommand : ICommand {

        public abstract string trigger { get; }
        public abstract string description { get; }
        public abstract string example { get; }

        public abstract void Run(string[] args);

        protected Preferences loadPreferences() {
            var preferences = Preferences.sharedInstance;
            preferences.Refresh();
            return preferences;
        }

        protected bool assertPreferences() {
            if (Preferences.sharedInstance.propertiesExist) {
                return true;
            }

            fabl.Warn("Couldn't find " + Preferences.sharedInstance.propertiesPath);
            fabl.Info("Run 'entitas new' to create Entitas.properties with default values");

            return false;
        }
    }
}
