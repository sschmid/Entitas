using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.Utils;

namespace Entitas.CodeGeneration.Plugins {

    public class ContextNamesConfig : AbstractConfigurableConfig {

        const string CONTEXTS_KEY = "Entitas.CodeGeneration.Plugins.Contexts";

        public override Dictionary<string, string> defaultProperties {
            get {
                return new Dictionary<string, string> {
                    { CONTEXTS_KEY, "Game, Input" }
                };
            }
        }

        public string[] contextNames {
            get {
                return _preferences[CONTEXTS_KEY]
                    .ArrayFromCSV()
                    .Select(context => extractContextName(context))
                    .ToArray();
            }
        }

        public Dictionary<string, string[]> contextsWithKits {
            get {
                return _preferences[CONTEXTS_KEY]
                    .ArrayFromCSV()
                    .ToDictionary(
                        context => extractContextName(context),
                        context => extractKits(context)
                    );
            }
        }

        static string extractContextName(string context) {
            return context
                .Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries)[0]
                .Trim();
        }

        static string[] extractKits(string context) {
            var contextSplit = context
                .Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);

            return contextSplit.Length > 1
                ? contextSplit[1]
                    .Split(new[] { "+" }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(kit => kit.Trim())
                    .ToArray()
                : new string[0];
        }
    }
}
