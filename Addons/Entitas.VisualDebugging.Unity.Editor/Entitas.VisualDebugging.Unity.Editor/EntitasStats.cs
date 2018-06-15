using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Utils;
using Entitas.CodeGeneration.Attributes;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor {

    public static class EntitasStats {

        [MenuItem("Tools/Entitas/Show Stats", false, 200)]
        public static void ShowStats() {
            var stats = string.Join("\n", GetStats()
                .Select(kv => kv.Key + ": " + kv.Value)
                .ToArray());

            Debug.Log(stats);
            EditorUtility.DisplayDialog("Entitas Stats", stats, "Close");
        }

        public static Dictionary<string, int> GetStats() {
            var types = AppDomain.CurrentDomain.GetAllTypes();

            var components = types
                .Where(type => type.ImplementsInterface<IComponent>())
                .ToArray();

            var systems = types
                .Where(isSystem)
                .ToArray();

            var contexts = getContexts(components);

            var stats = new Dictionary<string, int> {
                { "Total Components", components.Length },
                { "Systems", systems.Length }
            };

            foreach (var context in contexts) {
                stats.Add("Components in " + context.Key, context.Value);
            }

            return stats;
        }

        static Dictionary<string, int> getContexts(Type[] components) {
            return components.Aggregate(new Dictionary<string, int>(), (contexts, type) => {
                var contextNames = getContextNamesOrDefault(type);
                foreach (var contextName in contextNames) {
                    if (!contexts.ContainsKey(contextName)) {
                        contexts.Add(contextName, 0);
                    }

                    contexts[contextName] += 1;
                }
                return contexts;
            });
        }


        static string[] getContextNames(Type type) {
            return Attribute
                .GetCustomAttributes(type)
                .OfType<ContextAttribute>()
                .Select(attr => attr.contextName)
                .ToArray();
        }

        static string[] getContextNamesOrDefault(Type type) {
            var contextNames = getContextNames(type);
            if (contextNames.Length == 0) {
                contextNames = new[] { "Default" };
            }

            return contextNames;
        }

        static bool isSystem(Type type) {
            return type.ImplementsInterface<ISystem>()
                   && type != typeof(ReactiveSystem<>)
                   && type != typeof(MultiReactiveSystem<,>)
                   && type != typeof(Systems)
                   && type != typeof(DebugSystems)
                   && type != typeof(JobSystem<>)
                   && type.FullName != "Feature";
        }
    }
}
