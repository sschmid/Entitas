using System.Collections.Generic;

namespace Entitas.CodeGenerator {

    public class CodeGenerator {

        readonly ICodeGeneratorDataProvider[] _dataProviders;
        readonly ICodeGenerator[] _codeGenerators;
        readonly ICodeGenFilePostProcessor[] _postProcessors;

        public CodeGenerator(ICodeGeneratorDataProvider[] dataProviders,
                             ICodeGenerator[] codeGenerators,
                             ICodeGenFilePostProcessor[] postProcessors) {

            _dataProviders = dataProviders;
            _codeGenerators = codeGenerators;
            _postProcessors = postProcessors;
        }

        public CodeGenFile[] DryRun() {
            var data = new List<CodeGeneratorData>();
            foreach(var dataProvider in _dataProviders) {
                data.AddRange(dataProvider.GetData());
            }

            var files = new List<CodeGenFile>();
            foreach(var generator in _codeGenerators) {
                files.AddRange(generator.Generate(data.ToArray()));
            }

            return files.ToArray();
        }

        public CodeGenFile[] Generate() {
            var data = new List<CodeGeneratorData>();
            foreach(var dataProvider in _dataProviders) {
                data.AddRange(dataProvider.GetData());
            }

            var files = new List<CodeGenFile>();
            foreach(var generator in _codeGenerators) {
                files.AddRange(generator.Generate(data.ToArray()));
            }

            var generatedFiles = files.ToArray();
            foreach(var postProcessor in _postProcessors) {
                postProcessor.PostProcess(generatedFiles);
            }

            return generatedFiles;
        }
    }
}
