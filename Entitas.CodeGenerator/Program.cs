using Entitas.CodeGenerator;
using System.Reflection;

public class Program {
    public static void Main(string[] args) {
        var assembly = Assembly.GetAssembly(typeof(CodeGenerator));
        CodeGenerator.Generate(assembly.GetTypes(), new string[0], "Generated/");
    }
}
