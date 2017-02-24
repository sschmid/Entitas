using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    public static class EntitasStats {

        [MenuItem("Entitas/Log Stats", false, 200)]
        public static void ShowStats() {
            var stats = string.Join("\n", GetStats()
                                    .Select(kv => kv.Key + ": " + kv.Value)
                                    .ToArray());

            EditorUtility.DisplayDialog("Entitas Stats", stats, "Close");
            Debug.Log(stats);
        }

        public static Dictionary<string, int> GetStats() {
            var types = Assembly.GetAssembly(typeof(IEntity)).GetTypes();
            var components = types.Where(type => type.ImplementsInterface<IComponent>()).ToArray();
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
                var contextNames = ContextsComponentDataProvider.GetContextNames(type);
                if(contextNames.Length == 0) {
                    contextNames = new [] { "Unassigned" };
                }
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
                && type != typeof(DebugSystems)
                && type != typeof(Feature);
        }
    }
}
