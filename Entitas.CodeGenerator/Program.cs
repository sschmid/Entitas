using System;
using System.Reflection;
using Entitas.CodeGenerator;

public class Program {
    public static void Main(string[] args) {
        var assembly = Assembly.GetAssembly(typeof(CodeGenerator));

        var componentCodeGenerators = new IComponentCodeGenerator[] {
            new IndicesLookupGenerator(),
            new ComponentExtensionsGenerator()
        };

        var systemCodeGenerators = new ISystemCodeGenerator[] {
            new SystemExtensionsGenerator()
        };

        var poolCodeGenerators = new IPoolCodeGenerator[] {
            new PoolAttributeGenerator(),
            new PoolsGenerator()
        };

        CodeGenerator.Generate(assembly.GetTypes(), new string[0], "Generated/",
            componentCodeGenerators, systemCodeGenerators, poolCodeGenerators);

        Console.WriteLine("Done. Press any key...");
        Console.Read();
    }
}
