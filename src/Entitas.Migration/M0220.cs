using System.Linq;
using System.Text.RegularExpressions;

namespace Entitas.Migration
{
    public class M0220 : IMigration
    {
        public string Version => "0.22.0";
        public string WorkingDirectory => "where all systems are located";
        public string Description => "Migrates IReactiveSystem to combine trigger and eventTypes to TriggerOnEvent";

        const string TriggerPattern = @"public\s*IMatcher\s*trigger\s*\{\s*get\s*\{\s*return\s*(?<matcher>.*?)\s*;\s*\}\s*\}";
        const string EventTypePattern = @"^\s*public\s*GroupEventType\s*eventType\s*\{\s*get\s*\{\s*return\s*GroupEventType\.(?<eventType>\w*)\s*;\s*\}\s*\}";

        const string TriggerReplacementFormat = @"public TriggerOnEvent trigger {{ get {{ return {0}.{1}(); }} }}";

        public MigrationFile[] Migrate(string path) => MigrationUtils.GetFiles(path)
            .Where(file => Regex.IsMatch(file.FileContent, TriggerPattern))
            .Select(file =>
            {
                var eventTypeMatch = Regex.Match(file.FileContent, EventTypePattern, RegexOptions.Multiline);
                var eventType = eventTypeMatch.Groups["eventType"].Value;
                file.FileContent = Regex.Replace(file.FileContent, EventTypePattern, string.Empty, RegexOptions.Multiline);
                file.FileContent = Regex.Replace(file.FileContent, TriggerPattern,
                    match => string.Format(TriggerReplacementFormat, match.Groups["matcher"].Value, eventType),
                    RegexOptions.Multiline);
                return file;
            })
            .ToArray();
    }
}
