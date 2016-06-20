using System;
using Entitas.CodeGeneration;
using System.IO;
using Entitas.Unity;
using System.Reflection;

namespace CodeGeneration.Reflection {
    class MainClass {
        public static void Main(string[] args) {
            /* Properties:
Entitas.CodeGenerator.AssemblyPath = CodeGenerator.Dependency.dll
Entitas.CodeGenerator.GeneratedFolderPath = ../../GeneratedTestFolderPath
Entitas.CodeGenerator.PoolNames = meta, world
Entitas.CodeGenerator.BlueprintNames =

            */
            var propertiesContent = File.ReadAllText(args[0]);
            var properties = new Properties(propertiesContent);
            var assemblyPath = properties["Entitas.CodeGenerator.AssemblyPath"];
            if(!File.Exists (assemblyPath)){
                throw new Exception (string.Format("Assembly at path '{0}' does not exist!", assemblyPath));
            }

            var assembly = Assembly.LoadFrom(assemblyPath);
            var outputDirectory = properties["Entitas.CodeGenerator.GeneratedFolderPath"];
            var pools = properties["Entitas.CodeGenerator.PoolNames"].Split(',');
            var blueprintNames = properties["Entitas.CodeGenerator.BlueprintNames"].Split(',');

            var codeGenerators = new ICodeGenerator[]{  
                new BlueprintsGenerator(),
                new ComponentExtensionsGenerator(),
                new ComponentIndicesGenerator(),
                new PoolAttributesGenerator(),
                new PoolsGenerator()
            };


            var output = TypeReflectionCodeGenerator.Generate(assembly, pools, blueprintNames, outputDirectory, codeGenerators);
            foreach (var file in output) {
                System.Console.WriteLine("file.fileName: " + file.fileName);
            }

        }
    }
}
