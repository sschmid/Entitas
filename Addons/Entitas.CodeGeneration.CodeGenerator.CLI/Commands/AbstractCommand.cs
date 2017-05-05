using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public abstract class AbstractCommand : ICommand {

        public abstract string trigger { get; }
        public abstract string description { get; }
        public abstract string example { get; }

        public abstract void Run(string[] args);

        protected Properties loadProperties() {
            return new Properties(File.ReadAllText(Preferences.PATH));
        }

        protected bool assertProperties() {
            if (File.Exists(Preferences.PATH)) {
                return true;
            }

            fabl.Warn("Couldn't find " + Preferences.PATH);
            fabl.Info("Run 'entitas new' to create Entitas.properties with default values");

            return false;
        }
    }
}
