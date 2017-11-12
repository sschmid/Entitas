using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public class Program {

        static Dictionary<LogLevel, ConsoleColor> _consoleColors = new Dictionary<LogLevel, ConsoleColor> {
            { LogLevel.Warn, ConsoleColor.DarkYellow },
            { LogLevel.Error, ConsoleColor.Red },
            { LogLevel.Fatal, ConsoleColor.DarkRed }
        };

        static ICommand[] _commands;

        public static void Main(string[] args) {
            _commands = AppDomain.CurrentDomain
                                    .GetInstancesOf<ICommand>()
                                    .OrderBy(c => c.trigger)
                                    .ToArray();

            if (args == null || args.Length == 0) {
                printUsage();
                return;
            }

            setupLogging(args);

            try {
                GetCommand(args[0]).Run(args);
            } catch (Exception ex) {
                PrintException(ex, args);
            }
        }

        static bool shouldKeepAlive() {
            fabl.Info("Again ? (y / n)");
            var userDecision = Helper.GetUserDecision();
            if (userDecision) {
                fabl.Info("👍");
            } else {
                fabl.Info("👋");
            }
            return userDecision;
        }

        public static ICommand GetCommand(string trigger) {
            var command = _commands.SingleOrDefault(c => c.trigger == trigger);
            if (command == null) {
                throw new Exception("command not found: " + trigger);
            }

            return command;
        }

        public static void PrintException(Exception ex, string[] args) {
            var loadException = ex as ReflectionTypeLoadException;
            if (loadException != null) {
                foreach (var e in loadException.LoaderExceptions) {
                    fabl.Error(e.ToString());
                }
            } else {
                if (args.isVerbose()) {
                    fabl.Error(ex.ToString());
                } else {
                    fabl.Error(ex.Message);
                }
            }
        }

        static void printUsage() {
            var commands = _commands
                .Where(c => c.description != null)
                .ToArray();
            var pad = commands.Max(c => c.example.Length);
            var commandList = commands
                .Select(c => c.example.PadRight(pad) + " - " + c.description)
                .Aggregate(new List<string>(), (acc, c) => { acc.Add(c); return acc; });

            commandList.Add("[-v]".PadRight(pad) + " - " + "verbose output");
            commandList.Add("[-s]".PadRight(pad) + " - " + "silent output (errors only)");

            var header =
@"#   _____       _   _ _
#  | ____|_ __ | |_(_) |_ __ _ ___
#  |  _| | '_ \| __| | __/ _` / __|
#  | |___| | | | |_| | || (_| \__ \
#  |_____|_| |_|\__|_|\__\__,_|___/
#
#  Entitas Code Generator version " + EntitasResources.GetVersion() + "\n";

            var footer = "All preferences will be stored in " + Preferences.DEFAULT_PROPERTIES_PATH + " by default.\n" +
                         "Each command also supports specifying a custom properties file.\n\n" +
                         "EXAMPLE\n" +
                         "  entitas new My.properties\n" +
                         "  entitas doctor My.properties\n" +
                         "  entitas fix My.properties\n" +
                         "  entitas gen My.properties";

            Console.WriteLine(header);
            Console.WriteLine("usage:\n{0}", string.Join("\n", commandList) + "\n\n" + footer);
        }

        static void setupLogging(string[] args) {
            if (args.isVerbose()) {
                fabl.globalLogLevel = LogLevel.On;
            } else if (args.isSilent()) {
                fabl.globalLogLevel = LogLevel.Error;
            } else {
                fabl.globalLogLevel = LogLevel.Info;
            }

            fabl.AddAppender((logger, logLevel, message) => {
                if (_consoleColors.ContainsKey(logLevel)) {
                    Console.ForegroundColor = _consoleColors[logLevel];
                    Console.WriteLine(message);
                    Console.ResetColor();
                } else {
                    Console.WriteLine(message);
                }
            });
        }
    }
}
