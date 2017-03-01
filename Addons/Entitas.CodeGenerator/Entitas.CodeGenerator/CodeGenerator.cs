using System.Collections.Generic;
using System.Linq;

namespace Entitas.CodeGenerator {

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
            _cancel = false;

            var data = new List<CodeGeneratorData>();

            var total = _dataProviders.Length + _codeGenerators.Length;
            int progress = 0;

            foreach(var dataProvider in _dataProviders) {
                if(_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if(OnProgress != null) {
                    OnProgress("[Dry Run] Creating model", dataProvider.name, (float)progress / total);
                }

                data.AddRange(dataProvider.GetData());
            }

            var files = new List<CodeGenFile>();
            var dataArray = data.ToArray();
            foreach(var generator in _codeGenerators) {
                if(_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if(OnProgress != null) {
                    OnProgress("[Dry Run] Creating files", generator.name, (float)progress / total);
                }

                files.AddRange(generator.Generate(dataArray));
            }

            return files.ToArray();
        }

        public CodeGenFile[] Generate() {
            _cancel = false;

            var data = new List<CodeGeneratorData>();

            var total = _dataProviders.Length + _codeGenerators.Length + _postProcessors.Length;
            int progress = 0;

            foreach(var dataProvider in _dataProviders) {
                if(_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if(OnProgress != null) {
                    OnProgress("Creating model", dataProvider.name, (float)progress / total);
                }

                data.AddRange(dataProvider.GetData());
            }

            var files = new List<CodeGenFile>();
            var dataArray = data.ToArray();
            foreach(var generator in _codeGenerators) {
                if(_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if(OnProgress != null) {
                    OnProgress("Creating files", generator.name, (float)progress / total);
                }

                files.AddRange(generator.Generate(dataArray));
            }

            var generatedFiles = files.ToArray();
            foreach(var postProcessor in _postProcessors) {
                if(_cancel) {
                    return new CodeGenFile[0];
                }

                progress += 1;
                if(OnProgress != null) {
                    OnProgress("Processing files", postProcessor.name, (float)progress / total);
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
