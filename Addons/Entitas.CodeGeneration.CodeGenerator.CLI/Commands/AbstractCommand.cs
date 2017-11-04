using System.IO;
using System.Linq;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public abstract class AbstractCommand : ICommand {

        public abstract string trigger { get; }
        public abstract string description { get; }
        public abstract string example { get; }

        public abstract void Run(string[] args);

        protected Preferences loadPreferences(string[] args) {
            var customPreferences = getCustomPreferences(args);

            Preferences preferences;
            if (customPreferences != null) {
                preferences = Preferences.sharedInstance = customPreferences;
            } else {
                preferences = Preferences.sharedInstance;
                preferences.Refresh();
            }

            return preferences;
        }

        protected bool assertPreferences(string[] args) {
            var path = findPath(args) ?? Preferences.DEFAULT_PROPERTIES_PATH;
            if (File.Exists(path)) {
                return true;
            }

            fabl.Warn("Couldn't find " + path);
            fabl.Info("Run 'entitas new' to create Entitas.properties with default values");

            return false;
        }

        static string findPath(string[] args) {
            return args.FirstOrDefault(arg => arg.Contains(".properties"));
        }

        static Preferences getCustomPreferences(string[] args) {
            var path = findPath(args);
            return path != null
                ? new Preferences(path, Preferences.DEFAULT_USER_PROPERTIES_PATH)
                : null;
        }
    }
}
