using Entitas.CodeGenerator;
using System.Reflection;

namespace CodeGenerator {
    public class Program {
        public static void Main(string[] args) {
            EntitasCodeGenerator.generatedFolder = "Generated/";
            EntitasCodeGenerator.CleanGeneratedFolder();
            var assembly = Assembly.GetAssembly(typeof(EntitasCodeGenerator));
            EntitasCodeGenerator.Generate(assembly);
        }
    }
}
