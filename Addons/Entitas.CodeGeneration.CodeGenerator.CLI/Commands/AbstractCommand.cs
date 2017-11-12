using System.IO;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public abstract class AbstractCommand : ICommand {

        public abstract string trigger { get; }
        public abstract string description { get; }
        public abstract string example { get; }

        protected Preferences _preferences;
        protected string[] _rawArgs;
        protected string[] _args;

        public void Run(string[] args) {
            assertPreferences(args);
            _preferences = new Preferences(args.GetPropertiesPath(), Preferences.DEFAULT_USER_PROPERTIES_PATH);
            _rawArgs = args;
            _args = args.Filter();
            run();
        }

        protected abstract void run();

        static void assertPreferences(string[] args) {
            var path = args.GetPropertiesPath();
            if (File.Exists(path)) {
                return;
            }

            fabl.Warn("Couldn't find " + path);
            fabl.Info("Run 'entitas new' to create Entitas.properties with default values");
        }
    }
}
