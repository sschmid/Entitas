using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    class MainClass {

        static Dictionary<LogLevel, ConsoleColor> _consoleColors = new Dictionary<LogLevel, ConsoleColor> {
            { LogLevel.Warn, ConsoleColor.DarkYellow },
            { LogLevel.Error, ConsoleColor.Red },
            { LogLevel.Fatal, ConsoleColor.DarkRed }
        };

        static bool isForce(string[] args) {
            return args.Any(arg => arg == "-f");  
        }

        static bool isVerbose(string[] args) {
            return args.Any(arg => arg == "-v");  
        }

        static bool isSilent(string[] args) {
            return args.Any(arg => arg == "-s");  
        }

        public static void Main(string[] args) {
            if(args == null || args.Length == 0) {
                printUsage();
                return;
            }

            setupLogging(args);

            try {
                switch(args[0]) {
                    case "new":
                        NewConfig.Run(isForce(args));
                        break;
                    case "edit":
                        EditConfig.Run();
                        break;
                    case "doctor":
                        Doctor.Run();
                        break;
                    case "status":
                        Status.Run();
                        break;
                    case "fix":
                        FixConfig.Run();
                        break;
                    case "scan":
                        ScanDlls.Run();
                        break;
                    case "dry":
                        DryRun.Run();
                        break;
                    case "gen":
                        Generate.Run();
                        break;
                    default:
                        printUsage();
                        break;
                }
            } catch(Exception ex) {
                var loadException = ex as ReflectionTypeLoadException;
                if(loadException != null) {
                    foreach(var e in loadException.LoaderExceptions) {
                        fabl.Error(e.ToString());
                    }
                } else {
                    if(isVerbose(args)) {
                        fabl.Error(ex.ToString());
                    } else {
                        fabl.Error(ex.Message);
                    }
                }
            }
        }

        static void printUsage() {
            Console.WriteLine("Entitas Code Generator version " + EntitasResources.GetVersion());
            Console.WriteLine(
@"usage: entitas new [-f] - Creates new Entitas.properties config with default values
       entitas edit     - Opens Entitas.properties config
       entitas doctor   - Checks the config for potential problems
       entitas status   - Lists available and unavailable plugins
       entitas fix      - Adds missing or removes unused keys interactively
       entitas scan     - Scans and prints available types found in specified assemblies
       entitas dry      - Simulates generating files without writing to disk
       entitas gen      - Generates files based on Entitas.properties
       [-v]             - verbose output
       [-s]             - silent output (errors only)"
            );
        }

        static void setupLogging(string[] args) {
            if(isVerbose(args)) {
                fabl.globalLogLevel = LogLevel.On;
            } else if(isSilent(args)) {
                fabl.globalLogLevel = LogLevel.Error;
            } else {
                fabl.globalLogLevel = LogLevel.Info;
            }

            fabl.AddAppender((logger, logLevel, message) => {
                if(_consoleColors.ContainsKey(logLevel)) {
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
