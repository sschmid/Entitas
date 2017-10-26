using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public abstract class AbstractCommand : ICommand {

        public abstract string trigger { get; }
        public abstract string description { get; }
        public abstract string example { get; }

        public abstract void Run(string[] args);

        protected Preferences loadPreferences() {
            var preferences = new Preferences();
            preferences.Load();
            return preferences;
        }

        protected bool assertPreferences() {
            if (File.Exists(Preferences.PATH)) {
                return true;
            }

            fabl.Warn("Couldn't find " + Preferences.PATH);
            fabl.Info("Run 'entitas new' to create Entitas.properties with default values");

            return false;
        }
    }
}
