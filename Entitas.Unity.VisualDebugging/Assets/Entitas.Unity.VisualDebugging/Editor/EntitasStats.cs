using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {
    public static class EntitasStats {

        [MenuItem("Entitas/Stats")]
        public static void LogStats() {
            foreach (var stat in GetStats()) {
                Debug.Log(stat.Key + ": " + stat.Value);
            }
        }

        public static Dictionary<string, int> GetStats() {
            var types = Assembly.GetAssembly(typeof(Entity)).GetTypes();
            return new Dictionary<string, int> {
                { "Components", types.Count(implementsComponent) },
                { "Systems", types.Count(implementsSystem) }
            };
        }

        static bool implementsComponent(Type type) {
            return type.GetInterfaces().Contains(typeof(IComponent))
                && type != typeof(IComponent)
                && type != typeof(DebugComponent);
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

