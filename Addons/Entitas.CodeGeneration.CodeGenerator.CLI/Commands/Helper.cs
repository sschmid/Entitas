using System;
using System.Linq;
using Entitas.Utils;
using Fabl;

namespace Entitas.CodeGeneration.CodeGenerator.CLI {

    public enum UserDecision {
        Accept,
        Cancel,
        Ignore
    }

    public static class Helper {

        public static string[] GetUnusedKeys(string[] requiredKeys, Preferences preferences) {
            return preferences.keys
                              .Where(key => !requiredKeys.Contains(key))
                              .ToArray();
        }

        public static string[] GetMissingKeys(string[] requiredKeys, Preferences preferences) {
            return requiredKeys
                .Where(key => !preferences.HasKey(key))
                .ToArray();
        }

        public static bool GetUserDecision(char accept = 'y', char cancel = 'n') {
            char keyChar;
            do {
                keyChar = Console.ReadKey(true).KeyChar;
            } while(keyChar != accept && keyChar != cancel);

            return keyChar == accept;
        }

        public static UserDecision GetComplexUserDecision(char accept = 'y', char cancel = 'n', char ignore = 'i') {
            char keyChar;
            do {
                keyChar = Console.ReadKey(true).KeyChar;
            } while(keyChar != accept && keyChar != cancel && keyChar != ignore);

            if (keyChar == ignore) {
                return UserDecision.Ignore;
            }

            return keyChar == accept
                ? UserDecision.Accept
                : UserDecision.Cancel;
        }

        public static void ForceAddKey(string message, string key, string value, Preferences preferences) {
            fabl.Info(message + ": '" + key + "'");
            Console.ReadKey(true);
            preferences[key] = value;
            preferences.Save();
            fabl.Info("Added: " + key);
        }

        public static void AddKey(string question, string key, string value, Preferences preferences) {
            fabl.Info(question + ": '" + key + "' ? (y / n)");
            if (GetUserDecision()) {
                preferences[key] = value;
                preferences.Save();
                fabl.Info("Added: " + key);
            }
        }

        public static void AddValue(string question, string value, string[] values, Action<string[]> updateAction, Preferences preferences) {
            fabl.Info(question + ": '" + value + "' ? (y / n)");
            if (GetUserDecision()) {
                var valueList = values.ToList();
                valueList.Add(value);
                updateAction(valueList.ToArray());
                preferences.Save();
                fabl.Info("Added: " + value);
            }
        }

        public static void AddValueSilently(string value, string[] values, Action<string[]> updateAction, Preferences preferences) {
            var valueList = values.ToList();
            valueList.Add(value);
            updateAction(valueList.ToArray());
            preferences.Save();
            fabl.Info("Added: " + value);
        }

        public static void RemoveKey(string question, string key, Preferences preferences) {
            fabl.Warn(question + ": '" + key + "' ? (y / n)");
            if (GetUserDecision()) {
                preferences.properties.RemoveProperty(key);
                preferences.Save();
                fabl.Warn("Removed: " + key);
            }
        }

        public static void RemoveOrIgnoreKey(string question, string key, CLIConfig cliConfig, Preferences preferences) {
            fabl.Warn(question + ": '" + key + "' ? (y / n / (i)gnore)");
            var userDecision = GetComplexUserDecision();
            if (userDecision == UserDecision.Accept) {
                preferences.properties.RemoveProperty(key);
                preferences.Save();
                fabl.Warn("Removed: " + key);
            } else if (userDecision == UserDecision.Ignore) {
                var valueList = cliConfig.ignoreUnusedKeys.ToList();
                valueList.Add(key);
                cliConfig.ignoreUnusedKeys = valueList.ToArray();
                preferences.Save();
                fabl.Warn("Ignoring: " + key);
            }
        }

        public static void RemoveValue(string question, string value, string[] values, Action<string[]> updateAction, Preferences preferences) {
            fabl.Warn(question + ": '" + value + "' ? (y / n)");
            if (GetUserDecision()) {
                var valueList = values.ToList();
                if (valueList.Remove(value)) {
                    updateAction(valueList.ToArray());
                    preferences.Save();
                    fabl.Warn("Removed: " + value);
                } else {
                    fabl.Warn("Value does not exist: " + value);
                }
            }
        }

        public static void RemoveValueSilently(string value, string[] values, Action<string[]> updateAction, Preferences preferences) {
            var valueList = values.ToList();
            if (valueList.Remove(value)) {
                updateAction(valueList.ToArray());
                preferences.Save();
                fabl.Warn("Removed: " + value);
            } else {
                fabl.Warn("Value does not exist: " + value);
            }
        }
    }
}
