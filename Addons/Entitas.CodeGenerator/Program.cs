using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Entitas.CodeGenerator {

    class MainClass {

        public static void Main(string[] args) {
            if(args == null || args.Length != 1) {
                printUsage();
                return;
            }

            try {
                switch(args[0]) {
                    case "new":
                        newConfig();
                        break;
                    case "edit":
                        editConfig();
                        break;
                    case "doctor":
                        doctor();
                        break;
                    case "diff":
                        diff();
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
                        Console.WriteLine(e);
                    }
                } else {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static void printUsage() {
            Console.WriteLine(
@"usage: entitas new      - Creates new Entitas.properties with default values
       entitas edit     - Opens Entitas.properties
       entitas doctor   - Checks the config for potential problems
       entitas diff     - List of unused and invalid data providers, code generators and post processors
       entitas scan     - Scans and prints available types found in specified assemblies
       entitas dry      - Simulates generating files without running post processors
       entitas gen      - Generates files based on Entitas.properties"
            );
        }

        static void newConfig() {
            var types = Assembly.GetAssembly(typeof(CodeGenerator)).GetTypes();
            var defaultConfig = new CodeGeneratorConfig(
                new EntitasPreferencesConfig(string.Empty),
                CodeGeneratorUtil.GetOrderedTypeNames<ICodeGeneratorDataProvider>(types).ToArray(),
                CodeGeneratorUtil.GetOrderedTypeNames<ICodeGenerator>(types).ToArray(),
                CodeGeneratorUtil.GetOrderedTypeNames<ICodeGenFilePostProcessor>(types).ToArray()
            );

            var currentDir = Directory.GetCurrentDirectory();

            var path = currentDir + Path.DirectorySeparatorChar + EntitasPreferences.GetConfigPath();
            File.WriteAllText(path, defaultConfig.ToString());
            Console.WriteLine("Created " + path);
        }

        static void editConfig() {
            System.Diagnostics.Process.Start(EntitasPreferences.GetConfigPath());
        }

        static void doctor() {
            if(File.Exists(EntitasPreferences.GetConfigPath())) {
                var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromConfig(EntitasPreferences.GetConfigPath());
                codeGenerator.DryRun();
                Console.WriteLine("You're ready to generate.");
            } else {
                printNoConfig();
            }
        }

        static void diff() {
            if(File.Exists(EntitasPreferences.GetConfigPath())) {
                var fileContent = File.ReadAllText(EntitasPreferences.GetConfigPath());
                var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(fileContent));

                var types = CodeGeneratorUtil.GetTypesInAllAssemblies(config);

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

        static void scanDlls() {
            if(File.Exists(EntitasPreferences.GetConfigPath())) {
                var types = CodeGeneratorUtil.GetTypesInAllAssemblies(EntitasPreferences.GetConfigPath());
                foreach(var type in types) {
                    Console.WriteLine(type);
                }
            } else {
                printNoConfig();
            }
        }

        static void dryRun() {
            if(File.Exists(EntitasPreferences.GetConfigPath())) {
                var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromConfig(EntitasPreferences.GetConfigPath());
                codeGenerator.DryRun();
            } else {
                printNoConfig();
            }
        }

        static void generate() {
            if(File.Exists(EntitasPreferences.GetConfigPath())) {
                var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromConfig(EntitasPreferences.GetConfigPath());
                codeGenerator.Generate();
            } else {
                printNoConfig();
            }
        }

        static void printUnavailable(string[] names) {
            foreach(var name in names) {
                Console.WriteLine("Unavailable " + name);
            }
        }

        static void printAvailable(string[] names) {
            foreach(var name in names) {
                Console.WriteLine("Available " + name);
            }
        }

        static void printNoConfig() {
            Console.WriteLine("Couldn't find " + EntitasPreferences.GetConfigPath());
            Console.WriteLine("Run 'entitas new' to create Entitas.properties with default values");
        }
    }
}
