using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;
using DesperateDevs.Unity.Editor;
using Entitas.Generators.Attributes;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public static class EntitasMenuItems
    {
        const string EntitasDisableVisualDebugging = "ENTITAS_DISABLE_VISUAL_DEBUGGING";
        const string EntitasDisableDeepProfiling = "ENTITAS_DISABLE_DEEP_PROFILING";
        const string EntitasFastAndUnsafe = "ENTITAS_FAST_AND_UNSAFE";

        [MenuItem("Tools/Entitas/Settings...", false, 1)]
        public static void EntitasSettings() => Selection.activeObject = Editor.EntitasSettings.Instance;

        [MenuItem("Tools/Entitas/Enable VisualDebugging", false, 2)]
        public static void EnableVisualDebugging()
        {
            if (IsVisualDebuggingEnabled)
                new ScriptingDefineSymbols().AddForAll(EntitasDisableVisualDebugging);
            else
                new ScriptingDefineSymbols().RemoveForAll(EntitasDisableVisualDebugging);
        }

        [MenuItem("Tools/Entitas/Enable VisualDebugging", true)]
        public static bool ValidateEnableVisualDebugging()
        {
            Menu.SetChecked("Tools/Entitas/Enable VisualDebugging", IsVisualDebuggingEnabled);
            return true;
        }

        static bool IsVisualDebuggingEnabled => !ScriptingDefineSymbols.BuildTargetGroups.All(buildTarget =>
            PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Contains(EntitasDisableVisualDebugging));

        [MenuItem("Tools/Entitas/Enable Deep Profiling", false, 3)]
        public static void EnableDeepProfiling()
        {
            if (IsDeepProfilingEnabled)
                new ScriptingDefineSymbols().AddForAll(EntitasDisableDeepProfiling);
            else
                new ScriptingDefineSymbols().RemoveForAll(EntitasDisableDeepProfiling);
        }

        [MenuItem("Tools/Entitas/Enable Deep Profiling", true)]
        public static bool ValidateEnableDeepProfiling()
        {
            Menu.SetChecked("Tools/Entitas/Enable Deep Profiling", IsDeepProfilingEnabled);
            return true;
        }

        static bool IsDeepProfilingEnabled => !ScriptingDefineSymbols.BuildTargetGroups
            .All(buildTarget => PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Contains(EntitasDisableDeepProfiling));

        [MenuItem("Tools/Entitas/AERC - Safe", false, 4)]
        public static void SetSafeAerc()
        {
            new ScriptingDefineSymbols().RemoveForAll(EntitasFastAndUnsafe);
        }

        [MenuItem("Tools/Entitas/AERC - Safe", true)]
        public static bool ValidateSetSafeAerc()
        {
            var isChecked = GetAercMode == 0;
            Menu.SetChecked("Tools/Entitas/AERC - Safe", isChecked);
            return !isChecked;
        }

        [MenuItem("Tools/Entitas/AERC - FastAndUnsafe", false, 5)]
        public static void SetUnsafeAerc()
        {
            new ScriptingDefineSymbols().AddForAll(EntitasFastAndUnsafe);
        }

        [MenuItem("Tools/Entitas/AERC - FastAndUnsafe", true)]
        public static bool ValidateSetUnsafeAerc()
        {
            var isChecked = GetAercMode == 1;
            Menu.SetChecked("Tools/Entitas/AERC - FastAndUnsafe", isChecked);
            return !isChecked;
        }

        static int GetAercMode => ScriptingDefineSymbols.BuildTargetGroups
            .All(buildTarget => PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTarget).Contains(EntitasFastAndUnsafe))
            ? 1
            : 0;

        [MenuItem("Tools/Entitas/Generate/DefaultInstanceCreator", false, 6)]
        public static void GenerateDefaultInstanceCreator() => EntityDrawer.GenerateIDefaultInstanceCreator("MyType");

        [MenuItem("Tools/Entitas/Generate/TypeDrawer", false, 7)]
        public static void GenerateTypeDrawer() => EntityDrawer.GenerateITypeDrawer("MyType");

        [MenuItem("Tools/Entitas/Show Statistics", false, 8)]
        public static void ShowStatistics()
        {
            var stats = string.Join("\n", GetStatistics().Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            Debug.Log(stats);
            EditorUtility.DisplayDialog("Entitas Statistics", stats, "Close");
        }

        [MenuItem("Tools/Entitas/Open Entitas Wiki...", false, 50)]
        public static void EntitasWiki() => Application.OpenURL("https://github.com/sschmid/Entitas/wiki");

        [MenuItem("Tools/Entitas/Join the Entitas Discord Server...", false, 51)]
        public static void EntitasChat() => Application.OpenURL("https://discord.gg/uHrVx5Z");

        [MenuItem("Tools/Entitas/Feedback", false, 100)]
        public static void Feedback() => Application.OpenURL("https://github.com/sschmid/Entitas/issues");

        [MenuItem("Tools/Entitas/Feedback", true)]
        public static bool ValidateFeedback() => false;

        [MenuItem("Tools/Entitas/Report a bug...", false, 101)]
        public static void ReportBug() => Application.OpenURL("https://github.com/sschmid/Entitas/issues");

        [MenuItem("Tools/Entitas/Request a feature...", false, 102)]
        public static void RequestFeature() => Application.OpenURL("https://github.com/sschmid/Entitas/issues");

        [MenuItem("Tools/Entitas/Donate...", false, 103)]
        public static void Donate() => Application.OpenURL("https://www.paypal.com/donate/?hosted_button_id=BTMLSDQULZ852");

        public static Dictionary<string, int> GetStatistics()
        {
            var types = AppDomain.CurrentDomain.GetAllTypes();

            var components = types
                .Where(type => type.ImplementsInterface<IComponent>())
                .ToArray();

            var systems = types
                .Where(IsSystem)
                .ToArray();

            var contexts = GetContexts(components);

            var stats = new Dictionary<string, int>
            {
                { "Total Components", components.Length },
                { "Systems", systems.Length }
            };

            foreach (var context in contexts)
                stats.Add($"Components in {context.Key}", context.Value);

            return stats;
        }

        static Dictionary<string, int> GetContexts(Type[] components) => components
            .Aggregate(new Dictionary<string, int>(), (contexts, type) =>
            {
                var contextNames = GetContextNamesOrDefault(type);
                foreach (var contextName in contextNames)
                {
                    contexts.TryAdd(contextName, 0);
                    contexts[contextName] += 1;
                }

                return contexts;
            });

        static string[] GetContextNames(Type type) => Attribute
            .GetCustomAttributes(type)
            .OfType<ContextAttribute>()
            .Select(attr => attr.Type.FullName)
            .ToArray();

        static string[] GetContextNamesOrDefault(Type type)
        {
            var contextNames = GetContextNames(type);
            if (contextNames.Length == 0)
                contextNames = new[] { "Default" };

            return contextNames;
        }

        static bool IsSystem(Type type) =>
            type.ImplementsInterface<ISystem>()
            && type != typeof(ReactiveSystem<>)
            && type != typeof(Systems)
            && type != typeof(DebugSystems)
            && type != typeof(ParallelSystem<>)
            && type.FullName != "Feature";
    }
}
