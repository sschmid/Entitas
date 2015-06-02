using Entitas.CodeGenerator;
using System.Reflection;

public class Program {
    public static void Main(string[] args) {
        var assembly = Assembly.GetAssembly(typeof(CodeGenerator));

        var componentCodeGenerators = new IComponentCodeGenerator[] {
            new IndicesLookupGenerator(),
            new ComponentExtensionsGenerator()
        };

        var poolCodeGenerators = new IPoolCodeGenerator[] {
            new PoolAttributeGenerator()
        };

        CodeGenerator.Generate(assembly.GetTypes(), new string[0], "Generated/",
            componentCodeGenerators, poolCodeGenerators);
    }
}
