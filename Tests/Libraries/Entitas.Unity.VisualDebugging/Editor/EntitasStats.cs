using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entitas.CodeGenerator;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public static class EntitasStats {

        public const string keyComponents = "Components";
        public const string keySystems = "Systems";

        [MenuItem("Entitas/Stats")]
        public static void LogStats() {
            foreach (var stat in GetStats()) {
                Debug.Log(stat.Key + ": " + stat.Value);
            }
        }

        public static Dictionary<string, int> GetStats() {
            var types = Assembly.GetAssembly(typeof(Entity)).GetTypes();
            var components = types.Where(implementsComponent).ToArray();
            var pools = getPools(components);

            var stats = new Dictionary<string, int> {
                { keyComponents, components.Length },
                { keySystems, types.Count(implementsSystem) }
            };

            foreach (var pool in pools) {
                stats.Add(pool.Key, pool.Value);
            }

            return stats;
        }

        static Dictionary<string, int> getPools(Type[] components) {
            return components.Aggregate(new Dictionary<string, int>(), (lookups, type) => {
                var lookupTag = type.PoolName();
                if (lookupTag == string.Empty) {
                    lookupTag = "DefaultPool";
                }
                if (!lookups.ContainsKey(lookupTag)) {
                    lookups.Add(lookupTag, 0);
                }

                lookups[lookupTag] += 1;
                return lookups;
            });
        }

        static bool implementsComponent(Type type) {
            return type.GetInterfaces().Contains(typeof(IComponent))
                && type != typeof(IComponent);
        }

        static bool implementsSystem(Type type) {
            return type.GetInterfaces().Contains(typeof(ISystem))
                && type != typeof(ISystem)
                && type != typeof(IStartSystem)
                && type != typeof(IExecuteSystem)
                && type != typeof(IReactiveSystem)
                && type != typeof(ReactiveSystem)
                && type != typeof(Systems)
                && type != typeof(DebugSystems);
        }
    }
}

