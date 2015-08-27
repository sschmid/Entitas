using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Migration {
    public class M0220 : IMigration {

        public string version { get { return "0.22.0"; } }

        public string description { get { return "Migrates IReactiveSystem to combine trigger and eventTypes to TriggerOnEvent"; } }

        const string triggerPattern = @"public\s*IMatcher\s*trigger\s*\{\s*get\s*\{\s*return\s*(?<matcher>.*?)\s*;\s*\}\s*\}";
        const string eventTypePattern = @"^\s*public\s*GroupEventType\s*eventType\s*\{\s*get\s*\{\s*return\s*GroupEventType\.(?<eventType>\w*)\s*;\s*\}\s*\}";

        const string triggerReplacementFormat = @"public TriggerOnEvent trigger {{ get {{ return {0}.{1}(); }} }}";

        public MigrationFile[] Migrate(string path) {
            var files = MigrationUtils.GetSourceFiles(path)
                .Where(file => Regex.IsMatch(file.fileContent, triggerPattern))
                .ToArray();

            for (int i = 0; i < files.Length; i++) {
                var file = files[i];

                var eventTypeMatch = Regex.Match(file.fileContent, eventTypePattern, RegexOptions.Multiline);
                var eventType = eventTypeMatch.Groups["eventType"].Value;
                file.fileContent = Regex.Replace(file.fileContent, eventTypePattern, string.Empty, RegexOptions.Multiline);

                file.fileContent = Regex.Replace(file.fileContent, triggerPattern,
                    match => string.Format(triggerReplacementFormat, match.Groups["matcher"].Value, eventType),
                    RegexOptions.Multiline);

                files[i] = file;
            }

            return files;
        }
    }
}

