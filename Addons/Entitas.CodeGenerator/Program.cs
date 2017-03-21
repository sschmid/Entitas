using System;
using System.Linq;
using System.Reflection;
using System.IO;

namespace Entitas.CodeGenerator {

    class MainClass {

        const string fileName = "Entitas.properties";

        public static void Main(string[] args) {
            if(args == null || args.Length != 1) {
                printUsage();
                return;
            }

            try {
                switch(args[0]) {
                    case "init":
                        init();
                        break;
                    case "diff":
                        diff();
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
                Console.WriteLine(ex.Message);
            }
        }

        static void init() {
            var types = getTypes();
            var defaultConfig = new CodeGeneratorConfig(
                new EntitasPreferencesConfig(string.Empty),
                CodeGeneratorUtil.GetOrderedTypeNames<ICodeGeneratorDataProvider>(types).ToArray(),
                CodeGeneratorUtil.GetOrderedTypeNames<ICodeGenerator>(types).ToArray(),
                CodeGeneratorUtil.GetOrderedTypeNames<ICodeGenFilePostProcessor>(types).ToArray()
            );

            var currentDir = Directory.GetCurrentDirectory();

            var path = currentDir + Path.DirectorySeparatorChar + fileName;
            File.WriteAllText(path, defaultConfig.ToString());
            Console.WriteLine("Created " + path);
        }

        static void diff() {
            if(File.Exists(fileName)) {
                var fileContent = File.ReadAllText(fileName);
                var config = new CodeGeneratorConfig(new EntitasPreferencesConfig(fileContent));

                var types = getTypes();

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

        static Type[] getTypes() {
            return Assembly.GetAssembly(typeof(CodeGenerator)).GetTypes();
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

        static void generate() {
            if(File.Exists(fileName)) {
                var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromConfig(fileName);
                codeGenerator.Generate();
            } else {
                printNoConfig();
            }
        }

        static void dryRun() {
            if(File.Exists(fileName)) {
                var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromConfig(fileName);
                codeGenerator.DryRun();
            } else {
                printNoConfig();
            }
        }

        static void printNoConfig() {
            Console.WriteLine("Couldn't find " + fileName);
            Console.WriteLine("Run entitas init to create Entitas.properties with default values");
        }

        static void printUsage() {
            Console.WriteLine(
@"usage: entitas init     - Creates Entitas.properties with default values
       entitas diff     - List of unused or invalid data providers, code generators and post processors
       entitas dry      - Simulates generating files without running post processors
       entitas gen      - Generates files based on Entitas.properties"
            );
        }
    }
}
