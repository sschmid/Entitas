using System.Runtime.CompilerServices;
using VerifyTests;

namespace Entitas.Generators.Tests
{
    public static class ModuleInitializer
    {
        [ModuleInitializer]
        public static void Init()
        {
            VerifySourceGenerators.Initialize();
        }
    }
}
