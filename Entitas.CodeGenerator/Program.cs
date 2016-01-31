using System;
using System.Reflection;
using Entitas.CodeGenerator;

public class Program {
    public static void Main(string[] args) {
        var codeGenerators = new ICodeGenerator[] {
            new ComponentsGenerator(),
            new ComponentIndicesGenerator(),
            new PoolAttributeGenerator(),
            new PoolsGenerator()
        };

        var assembly = Assembly.GetAssembly(typeof(CodeGenerator));
        var provider = new TypeReflectionProvider(assembly.GetTypes());
        CodeGenerator.Generate(provider, "Generated/", codeGenerators);

        Console.WriteLine("Done. Press any key...");
        Console.Read();
    }
}
