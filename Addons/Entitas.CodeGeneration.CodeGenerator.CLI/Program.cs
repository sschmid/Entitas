using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    class MainClass {

        static Dictionary<LogLevel, ConsoleColor> _consoleColors = new Dictionary<LogLevel, ConsoleColor> {
            { LogLevel.Warn, ConsoleColor.DarkYellow },
            { LogLevel.Error, ConsoleColor.Red },
            { LogLevel.Fatal, ConsoleColor.DarkRed }
        };

        public static void Main(string[] args) {
            var commands = AppDomain.CurrentDomain
                                    .GetInstancesOf<ICommand>()
                                    .OrderBy(c => c.trigger)
                                    .ToArray();

            if (args == null || args.Length == 0) {
                printUsage(commands);
                return;
            }

            setupLogging(args);

            try {
                getCommand(commands, args[0]).Run(args);

                if (args.keepAlive()) {
                    while (shouldKeepAlive()) {
                        getCommand(commands, args[0]).Run(args);
                    }
                    fabl.Info("Goodbye, see you soon!");
                }
            } catch (Exception ex) {
                printException(ex, args);
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

        static ICommand getCommand(ICommand[] commands, string trigger) {
            var command = commands.SingleOrDefault(c => c.trigger == trigger);
            if (command == null) {
                throw new Exception("command not found: " + trigger);
            }

            return command;
        }

        static void printException(Exception ex, string[] args) {
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

        static void printUsage(ICommand[] commands) {
            var pad = commands.Max(c => c.example.Length);
            var commandList = commands
                .Select(c => c.example.PadRight(pad) + " - " + c.description)
                .Aggregate(new List<string>(), (acc, c) => { acc.Add(c); return acc; });

            commandList.Add("[-v]".PadRight(pad) + " - " + "verbose output");
            commandList.Add("[-s]".PadRight(pad) + " - " + "silent output (errors only)");
            commandList.Add("[-a]".PadRight(pad) + " - " + "keep process alive (for faster re-generation)");

            Console.WriteLine("🎃🎃🎃  Entitas Code Generator version " + EntitasResources.GetVersion());
            Console.WriteLine("usage:\n{0}", string.Join("\n", commandList));
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
