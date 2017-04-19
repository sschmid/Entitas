using System.Collections.Generic;
using System.IO;
using Entitas.Utils;
using System.Text.RegularExpressions;
using System;
using System.Linq;

namespace Entitas.CodeGeneration.Plugins {

    public class UpdateCSProjPostProcessor : ICodeGenFilePostProcessor, IConfigurable {

        public string name { get { return "Update .csproj"; } }
        public int priority { get { return 0; } }
        public bool isEnabledByDefault { get { return true; } }
        public bool runInDryMode { get { return false; } }

        public Dictionary<string, string> defaultProperties { get { return _projectPathConfig.defaultProperties; } }

        readonly ProjectPathConfig _projectPathConfig = new ProjectPathConfig();
        readonly TargetDirectoryConfig _targetDirectoryConfig = new TargetDirectoryConfig();

        public void Configure(Properties properties) {
            _projectPathConfig.Configure(properties);
            _targetDirectoryConfig.Configure(properties);
        }

        public CodeGenFile[] PostProcess(CodeGenFile[] files) {
            var project = File.ReadAllText(_projectPathConfig.projectPath);
            project = removeExistingGeneratedEntries(project);
            project = addGeneratedEntries(project, files);
            File.WriteAllText(_projectPathConfig.projectPath, project);
            return files;
        }

        string escapedTargetDirectory() {
            return _targetDirectoryConfig.targetDirectory
                                         .Replace("/", "\\")
                                         .Replace("\\", "\\\\");
        }

        string removeExistingGeneratedEntries(string project) {
            var entryPattern = @"\s*<Compile Include=""" + escapedTargetDirectory() + @".* \/>";
            return Regex.Replace(project, entryPattern, string.Empty);
        }

        string addGeneratedEntries(string project, CodeGenFile[] files) {
            const string endOfItemGroupPattern = @"<\/ItemGroup>";

            const string generatedEntriesTemplate =
@"  </ItemGroup>
  <ItemGroup>
{0}
  </ItemGroup>";

            const string entryTemplate = @"    <Compile Include=""{0}"" />";

            var generatedEntries = string.Format(
                generatedEntriesTemplate,
                string.Join("\r\n", files.Select(file => string.Format(entryTemplate, file.fileName.Replace("/", "\\"))).ToArray())
            );

            var rgx = new Regex(endOfItemGroupPattern);
            return rgx.Replace(project, generatedEntries, 1);
        }
    }
}
