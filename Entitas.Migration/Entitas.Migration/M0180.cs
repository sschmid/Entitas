using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Migration {
    public class M0180 : IMigration {

        public string version { get { return "0.18.0"; } }

        public string description { get { return "Migrates IReactiveSystem API"; } }

        const string methodEndPattern = @"(\s|.)*?\}";
        const string triggerPattern = @"public\s*IMatcher\s*GetTriggeringMatcher\s*\(\s*\)\s*\{\s*";
        const string triggerEndPattern = triggerPattern + methodEndPattern;
        const string triggerReplacement = "public IMatcher trigger { get { ";

        const string eventTypePattern = @"public\s*GroupEventType\s*GetEventType\s*\(\s*\)\s*\{\s*";
        const string eventTypePatternEnd = eventTypePattern + methodEndPattern;
        const string eventTypeReplacement = "public GroupEventType eventType { get { ";

        public MigrationFile[] Migrate(string path) {
            var files = MigrationUtils.GetSourceFiles(path)
                .Where(file => Regex.IsMatch(file.fileContent, triggerPattern) || Regex.IsMatch(file.fileContent, eventTypePattern))
                .ToArray();

            for (int i = 0; i < files.Length; i++) {
                var file = files[i];
                file.fileContent = Regex.Replace(file.fileContent, triggerEndPattern, match => match.Value + " }", RegexOptions.Multiline);
                file.fileContent = Regex.Replace(file.fileContent, eventTypePatternEnd, match => match.Value + " }", RegexOptions.Multiline);
                file.fileContent = Regex.Replace(file.fileContent, triggerPattern, triggerReplacement, RegexOptions.Multiline);
                file.fileContent = Regex.Replace(file.fileContent, eventTypePattern, eventTypeReplacement, RegexOptions.Multiline);
                files[i] = file;
            }

            return files;
        }

    }
}

