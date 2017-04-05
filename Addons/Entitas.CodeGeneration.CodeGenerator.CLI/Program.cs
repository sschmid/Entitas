using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Fabl;

namespace Entitas.CodeGenerator.CLI {

    class MainClass {

        static Logger _logger = fabl.GetLogger("Main");

        static Dictionary<LogLevel, ConsoleColor> _consoleColors = new Dictionary<LogLevel, ConsoleColor> {
            { LogLevel.Warn, ConsoleColor.DarkYellow },
            { LogLevel.Error, ConsoleColor.Red },
            { LogLevel.Fatal, ConsoleColor.DarkRed }
        };

        public static void Main(string[] args) {
            if(args == null || args.Length == 0) {
                printUsage();
                return;
            }

            setupLogging(args);

            try {

                switch(args[0]) {
                    case "new":
                        createNewConfig(args.Any(arg => arg == "-f"));
                        break;
                    case "edit":
                        editConfig();
                        break;
                    case "doctor":
                        doctor();
                        break;
                    case "status":
                        status();
                        break;
                    case "fix":
                        fixConfig();
                        break;
                    case "scan":
                        scanDlls();
                        break;
                    case "dry":
                        dryRun();
                        break;
                    case "gen":
                        generate();
                        break;
                    default:
                        printUsage();
                        break;
                }
            } catch(Exception ex) {
                var loadException = ex as ReflectionTypeLoadException;
                if(loadException != null) {
                    foreach(var e in loadException.LoaderExceptions) {
                        _logger.Error(e.ToString());
                    }
                } else {
                    if(isVerbose(args)) {
                        _logger.Error(ex.ToString());
                    } else {
                        _logger.Error(ex.Message);
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

        static bool isVerbose(string[] args) {
            return args.Any(arg => arg == "-v");  
        }

        static bool isSilent(string[] args) {
            return args.Any(arg => arg == "-s");  
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

        static void createNewConfig(bool force) {
            var currentDir = Directory.GetCurrentDirectory();
            var path = currentDir + Path.DirectorySeparatorChar + Preferences.configPath;

            if(!File.Exists(path) || force) {
                var types = AppDomain.CurrentDomain.GetAllTypes();
                var defaultConfig = new CodeGeneratorConfig(
                    new Config(string.Empty),
                    CodeGeneratorUtil.GetOrderedTypeNames<ICodeGeneratorDataProvider>(types).ToArray(),
                    CodeGeneratorUtil.GetOrderedTypeNames<ICodeGenerator>(types).ToArray(),
                    CodeGeneratorUtil.GetOrderedTypeNames<ICodeGenFilePostProcessor>(types).ToArray()
                );

                var config = defaultConfig.ToString();
                File.WriteAllText(path, config);
                _logger.Info("Created " + path);
                _logger.Debug(config);

                editConfig();
            } else {
                _logger.Warn(path + " already exists!");
                _logger.Info("Use entitas new -f to overwrite the exiting file.");
                _logger.Info("Use entitas edit to open the exiting file.");
            }
        }

        static void editConfig() {
            if(File.Exists(Preferences.configPath)) {
                _logger.Debug("Opening " + Preferences.configPath);
                System.Diagnostics.Process.Start(Preferences.configPath);
            } else {
                printNoConfig();
            }
        }

        static void doctor() {
            if(File.Exists(Preferences.configPath)) {
                status();
                _logger.Debug("Dry Run");
                CodeGeneratorUtil
                    .CodeGeneratorFromConfig(Preferences.configPath)
                    .DryRun();
            } else {
                printNoConfig();
            }
        }

        static void fixConfig() {
            if(File.Exists(Preferences.configPath)) {
                var fileContent = File.ReadAllText(Preferences.configPath);
                var properties = new Properties(fileContent);

                foreach(var key in getMissingKey(properties)) {
                    _logger.Info("Add missing key: '" + key + "' ? (y / n)");
                    if(getUserDecision()) {
                        properties[key] = string.Empty;
                        Preferences.SaveConfig(new Config(properties.ToString()));
                        Console.WriteLine("Added key: " + key);
                    }
                }

                foreach(var key in getUnusedKeys(properties)) {
                    _logger.Warn("Remove unused key: '" + key + "' ? (y / n)");
                    if(getUserDecision()) {
                        properties.RemoveKey(key);
                        Preferences.SaveConfig(new Config(properties.ToString()));
                        Console.WriteLine("Removed key: " + key);
                    }
                }
            } else {
                printNoConfig();
            }
        }

        static bool getUserDecision() {
            char keyChar;
            do {
                keyChar = Console.ReadKey(true).KeyChar;
            } while(keyChar != 'y' && keyChar != 'n');

            return keyChar == 'y';
        }

        static void status() {
            if(File.Exists(Preferences.configPath)) {
                var fileContent = File.ReadAllText(Preferences.configPath);
                var properties = new Properties(fileContent);
                var config = new CodeGeneratorConfig(new Config(fileContent));

                foreach(var key in getUnusedKeys(properties)) {
                    _logger.Info("Unused key: " + key);
                }

                foreach(var key in getMissingKey(properties)) {
                    _logger.Warn("Missing key: " + key);
                }

                var types = CodeGeneratorUtil.LoadTypesFromCodeGeneratorAssemblies();

                printUnavailable(CodeGeneratorUtil.GetUnavailable<ICodeGeneratorDataProvider>(types, config.dataProviders));
                printUnavailable(CodeGeneratorUtil.GetUnavailable<ICodeGenerator>(types, config.codeGenerators));
                printUnavailable(CodeGeneratorUtil.GetUnavailable<ICodeGenFilePostProcessor>(types, config.postProcessors));

                printAvailable(CodeGeneratorUtil.GetAvailable<ICodeGeneratorDataProvider>(types, config.dataProviders));
                printAvailable(CodeGeneratorUtil.GetAvailable<ICodeGenerator>(types, config.codeGenerators));
                printAvailable(CodeGeneratorUtil.GetAvailable<ICodeGenFilePostProcessor>(types, config.postProcessors));
            } else {
                printNoConfig();
            }
        }

        static string[] getUnusedKeys(Properties properties) {
            return properties.keys
                             .Where(key => !CodeGeneratorConfig.keys.Contains(key))
                             .ToArray();
        }

        static string[] getMissingKey(Properties properties) {
            return CodeGeneratorConfig.keys
                                      .Where(key => !properties.HasKey(key))
                                      .ToArray();
        }

        static void scanDlls() {
            if(File.Exists(Preferences.configPath)) {
                printTypes(CodeGeneratorUtil.LoadTypesFromCodeGeneratorAssemblies());
                printTypes(CodeGeneratorUtil.LoadTypesFromAssemblies());
            } else {
                printNoConfig();
            }
        }

        static void printTypes(Type[] types) {
            var orderedTypes = types
                .OrderBy(type => type.Assembly.GetName().Name)
                .ThenBy(type => type.FullName);
            foreach(var type in orderedTypes) {
                _logger.Info(type.Assembly.GetName().Name + ": " + type);
            }
        }

        static void dryRun() {
            if(File.Exists(Preferences.configPath)) {
                CodeGeneratorUtil
                    .CodeGeneratorFromConfig(Preferences.configPath)
                    .DryRun();
            } else {
                printNoConfig();
            }
        }

        static void generate() {
            if(File.Exists(Preferences.configPath)) {
                CodeGeneratorUtil
                    .CodeGeneratorFromConfig(Preferences.configPath)
                    .Generate();
            } else {
                printNoConfig();
            }
        }

        static void printUnavailable(string[] names) {
            foreach(var name in names) {
                _logger.Warn("Unavailable: " + name);
            }
        }

        static void printAvailable(string[] names) {
            foreach(var name in names) {
                _logger.Info("Available: " + name);
            }
        }

        static void printNoConfig() {
            _logger.Warn("Couldn't find " + Preferences.configPath);
            _logger.Info("Run 'entitas new' to create Entitas.properties with default values");
        }
    }
}
