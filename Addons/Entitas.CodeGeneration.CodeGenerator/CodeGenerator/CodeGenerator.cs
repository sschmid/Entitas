using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGeneration.CodeGenerator {

    public delegate void GeneratorProgress(string title, string info, float progress);

    public class CodeGenerator {

        public event GeneratorProgress OnProgress;

        readonly ICodeGeneratorDataProvider[] _dataProviders;
        readonly ICodeGenerator[] _codeGenerators;
        readonly ICodeGenFilePostProcessor[] _postProcessors;

        bool _cancel;

        public CodeGenerator(ICodeGeneratorDataProvider[] dataProviders,
                             ICodeGenerator[] codeGenerators,
                             ICodeGenFilePostProcessor[] postProcessors) {

            _dataProviders = dataProviders;
            _codeGenerators = codeGenerators;
            _postProcessors = postProcessors
                .OrderBy(pp => pp.priority)
                .ToArray();
        }

        public CodeGenFile[] DryRun() {
            return generate(
                "[Dry Run] ",
                _dataProviders.Where(i => i.runInDryMode).ToArray(),
                _codeGenerators.Where(i => i.runInDryMode).ToArray(),
                _postProcessors.Where(i => i.runInDryMode).ToArray()
            );
        }

        public CodeGenFile[] Generate() {
            return generate(
                string.Empty,
                _dataProviders,
                _codeGenerators,
                _postProcessors
            );
        }

        CodeGenFile[] generate(string messagePrefix, ICodeGeneratorDataProvider[] dataProviders, ICodeGenerator[] codeGenerators, ICodeGenFilePostProcessor[] postProcessors) {
            _cancel = false;

            var data = new List<CodeGeneratorData>();

            var total = dataProviders.Length + codeGenerators.Length + postProcessors.Length;
            int progress = 0;

            foreach(var dataProvider in dataProviders) {
                if(_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if(OnProgress != null) {
                    OnProgress(messagePrefix + "Creating model", dataProvider.name, (float)progress / total);
                }

                data.AddRange(dataProvider.GetData());
            }

            var files = new List<CodeGenFile>();
            var dataArray = data.ToArray();
            foreach(var generator in codeGenerators) {
                if(_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if(OnProgress != null) {
                    OnProgress(messagePrefix + "Creating files", generator.name, (float)progress / total);
                }

                files.AddRange(generator.Generate(dataArray));
            }

            var generatedFiles = files.ToArray();
            foreach(var postProcessor in postProcessors) {
                if(_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if(OnProgress != null) {
                    OnProgress(messagePrefix + "Processing files", postProcessor.name, (float)progress / total);
                }

                generatedFiles = postProcessor.PostProcess(generatedFiles);
            }

            return generatedFiles;
        }

        public void Cancel() {
            _cancel = true;
        }
    }
}
