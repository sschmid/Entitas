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
            var assembly = Assembly.GetAssembly(typeof(CodeGenerator));
            var defaultConfig = new CodeGeneratorConfig(
                new EntitasPreferencesConfig(string.Empty),
                CodeGeneratorUtil.GetOrderedTypeNames<ICodeGeneratorDataProvider>(assembly).ToArray(),
                CodeGeneratorUtil.GetOrderedTypeNames<ICodeGenerator>(assembly).ToArray(),
                CodeGeneratorUtil.GetOrderedTypeNames<ICodeGenFilePostProcessor>(assembly).ToArray()
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

                var assembly = Assembly.GetAssembly(typeof(CodeGenerator));

                printUnavailable(CodeGeneratorUtil.GetUnavailable<ICodeGeneratorDataProvider>(assembly, config.dataProviders));
                printUnavailable(CodeGeneratorUtil.GetUnavailable<ICodeGenerator>(assembly, config.codeGenerators));
                printUnavailable(CodeGeneratorUtil.GetUnavailable<ICodeGenFilePostProcessor>(assembly, config.postProcessors));

                printAvailable(CodeGeneratorUtil.GetAvailable<ICodeGeneratorDataProvider>(assembly, config.dataProviders));
                printAvailable(CodeGeneratorUtil.GetAvailable<ICodeGenerator>(assembly, config.codeGenerators));
                printAvailable(CodeGeneratorUtil.GetAvailable<ICodeGenFilePostProcessor>(assembly, config.postProcessors));
            } else {
                Console.WriteLine("Couldn't find " + fileName);
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

        static void generate() {
            if(File.Exists(fileName)) {
                var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromConfig(fileName);
                codeGenerator.Generate();
            } else {
                Console.WriteLine("Couldn't find " + fileName);
            }
        }

        static void dryRun() {
            if(File.Exists(fileName)) {
                var codeGenerator = CodeGeneratorUtil.CodeGeneratorFromConfig(fileName);
                codeGenerator.DryRun();
            } else {
                Console.WriteLine("Couldn't find " + fileName);
            }
        }

        static void printUsage() {
            Console.WriteLine(
@"usage: Entitas init     - Creates Entitas.properties with default values
       Entitas diff     - List of unused or invalid data providers, code generators and post processors
       Entitas gen      - Generates files based on Entitas.properties"
            );
        }
    }
}
