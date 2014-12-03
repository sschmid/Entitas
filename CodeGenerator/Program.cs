using Entitas.CodeGenerator;
using System.Reflection;

namespace CodeGenerator {
    public class Program {
        public static void Main(string[] args) {
            var assembly = Assembly.GetAssembly(typeof(EntitasCodeGenerator));
            EntitasCodeGenerator.Generate(assembly, "Generated/");
        }
    }
}
