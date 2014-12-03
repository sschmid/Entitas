using Entitas.CodeGenerator;
using System.IO;
using System.Reflection;

namespace Readme {
    class MainClass {
        public static void Main(string[] args) {
            var assembly = Assembly.GetAssembly(typeof(ReadmeSnippets));
            var generatedFolder = getProjectDir() + "/Components/Generated/";
            EntitasCodeGenerator.Generate(assembly, generatedFolder);
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

