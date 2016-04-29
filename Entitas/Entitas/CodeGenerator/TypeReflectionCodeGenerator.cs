﻿using System.Reflection;

namespace Entitas.CodeGenerator {
    public static class TypeReflectionCodeGenerator {
        public static CodeGenFile[] Generate(Assembly assembly, string[] poolNames, string[] blueprintNames, string directory, ICodeGenerator[] codeGenerators) {
            var provider = new TypeReflectionProvider(assembly.GetTypes(), poolNames, blueprintNames);
            IPostProcessor[] postProcessors = {
                new AddHeaderToFileProcessor(),
                new WriteToDirectoryProcessor(directory)
            };

            return CodeGenerator.Generate(provider, codeGenerators, postProcessors);
        }
    }
}

