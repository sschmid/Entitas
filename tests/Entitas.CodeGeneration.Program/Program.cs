using DesperateDevs.Cli.Utils;
using DesperateDevs.Serialization.Cli.Utils;
using Jenny.Generator;
using Jenny.Generator.Cli;

namespace Entitas.CodeGeneration.Program;

public class Program
{
    public static void Main(string[] args)
    {
        AbstractPreferencesCommand.DefaultPropertiesPath = CodeGenerator.DefaultPropertiesPath;
        new CliProgram("Jenny", typeof(GenerateCommand), args).Run();
    }
}
