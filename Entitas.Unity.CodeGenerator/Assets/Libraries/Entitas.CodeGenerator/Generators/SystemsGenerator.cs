using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.CodeGenerator;

namespace Entitas.CodeGenerator {
    public class SystemsGenerator : ISystemCodeGenerator {

        const string CLASS_SUFFIX = "GeneratedExtension";

        const string CLASS_TEMPLATE = @"namespace Entitas {{
    public partial class Pool {{
        public ISystem Create{0}() {{
            return this.CreateSystem<{1}>();
        }}
    }}
}}";

        public CodeGenFile[] Generate(Type[] systems) {
            return systems
                    .Where(type => type.GetConstructor(new Type[0]) != null)
                    .Aggregate(new List<CodeGenFile>(), (files, type) => {
                        files.Add(new CodeGenFile {
                            fileName = type + CLASS_SUFFIX,
                            fileContent = string.Format(CLASS_TEMPLATE, type.Name, type).ToUnixLineEndings()
                        });
                        return files;
                        })
                    .ToArray();
        }
    }
}
