using System;
using CSharpCodeGenerator.Deployer;
using Entitas.CodeGeneration;

namespace CodeGenerator.Roslyn.PostProcessors
{
    public class DeployToProjectProcessor: IPostProcessor
    {
        private ProjectDeployer _deployer;

        public DeployToProjectProcessor (string projectPath, string outputDirectory)
        {
            _deployer = new ProjectDeployer(projectPath, outputDirectory);
        }

        public void Process (CodeGenFile [] codegenFiles)
        {
            _deployer.ClearTargetFolder();
            foreach (var codeGenFile in codegenFiles)
            {
                _deployer.AddFileWithFormatting(new SourceCodeFile(codeGenFile.fileName, codeGenFile.fileContent));
            }
            _deployer.TryApplyChangesToProject();
        }
    }
}

