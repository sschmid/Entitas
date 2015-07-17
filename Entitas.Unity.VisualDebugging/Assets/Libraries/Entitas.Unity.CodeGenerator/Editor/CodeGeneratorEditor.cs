using System;
using System.Reflection;
using Entitas;
using Entitas.CodeGenerator;
using UnityEditor;

namespace Entitas.Unity.CodeGenerator {
    public static class CodeGeneratorEditor {

        [MenuItem("Entitas/Generate")]
        public static void Generate() {
            var types = Assembly.GetAssembly(typeof(Entity)).GetTypes();
            var config = new CodeGeneratorConfig(EntitasPreferencesEditor.LoadConfig());

            var codeGenerators = new ICodeGenerator[] {
                new ComponentExtensionsGenerator(),
                new IndicesLookupGenerator(),
                new PoolAttributeGenerator(),
                new PoolsGenerator(),
                new SystemExtensionsGenerator()
            };

            Entitas.CodeGenerator.CodeGenerator.Generate(types, config.pools, config.generatedFolderPath, codeGenerators);

            AssetDatabase.Refresh();
        }
    }
}
