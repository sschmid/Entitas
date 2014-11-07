using Entitas.CodeGenerator;
using System;
using System.IO;
using System.Reflection;

namespace Readme {
    class MainClass {
        public static void Main(string[] args) {
            EntitasCodeGenerator.generatedFolder = getProjectDir() + "/Components/Generated/";
            EntitasCodeGenerator.CleanGeneratedFolder();
            var assembly = Assembly.GetAssembly(typeof(ReadmeSnippets));
            EntitasCodeGenerator.Generate(assembly);
        }

        static string getProjectDir() {
            var dirInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            const string projectName = "Readme";
            while (dirInfo.Name != projectName) {
                dirInfo = dirInfo.Parent;
            }

            return dirInfo.FullName;
        }
    }
}

