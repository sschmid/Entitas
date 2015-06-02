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

            var componentCodeGenerators = new IComponentCodeGenerator [] {
                new IndicesLookupGenerator(),
                new ComponentExtensionsGenerator()
            };

            var poolCodeGenerators = new IPoolCodeGenerator [] {
                new PoolAttributeGenerator()
            };

            Entitas.CodeGenerator.CodeGenerator.Generate(types, config.pools, config.generatedFolderPath,
                componentCodeGenerators, poolCodeGenerators);

            AssetDatabase.Refresh();
        }
    }
}
