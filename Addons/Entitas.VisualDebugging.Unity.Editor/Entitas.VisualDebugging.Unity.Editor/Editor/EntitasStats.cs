using System;
using System.Collections.Generic;
using System.Linq;
using Entitas.CodeGeneration.Plugins;
using Entitas.Utils;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor {

    public static class EntitasStats {

        [MenuItem("Entitas/Show Stats", false, 200)]
        public static void ShowStats() {
            var stats = string.Join("\n", GetStats()
                                    .Select(kv => kv.Key + ": " + kv.Value)
                                    .ToArray());

            EditorUtility.DisplayDialog("Entitas Stats", stats, "Close");
            Debug.Log(stats);
        }

        public static Dictionary<string, int> GetStats() {
            var types = AppDomain.CurrentDomain.GetAllTypes();
            var components = types
                .Where(type => type.ImplementsInterface<IComponent>())
                .ToArray();
            var contexts = getContexts(components);

            var stats = new Dictionary<string, int> {
                { "Total Components", components.Length },
                { "Systems", types.Count(implementsSystem) }
            };

            foreach(var context in contexts) {
                stats.Add("Components in " + context.Key, context.Value);
            }

            return stats;
        }

        static Dictionary<string, int> getContexts(Type[] components) {
            return components.Aggregate(new Dictionary<string, int>(), (contexts, type) => {
                var contextNames = ContextsComponentDataProvider.GetContextNamesOrDefault(type);
                foreach(var contextName in contextNames) {
                    if(!contexts.ContainsKey(contextName)) {
                        contexts.Add(contextName, 0);
                    }

                    contexts[contextName] += 1;
                }
                return contexts;
            });
        }

        static bool implementsSystem(Type type) {
            return type.ImplementsInterface<ISystem>()
                && type != typeof(ReactiveSystem<>)
                && type != typeof(Systems)
                && type != typeof(DebugSystems);
        }
    }
}
