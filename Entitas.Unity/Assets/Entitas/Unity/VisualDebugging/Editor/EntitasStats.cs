using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Entitas.Api;
using Entitas.CodeGenerator;
using Entitas.Utils;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.VisualDebugging {

    public static class EntitasStats {

        [MenuItem(EntitasMenuItems.log_stats, false, EntitasMenuItemPriorities.log_stats)]
        public static void LogStats() {
            foreach(var stat in GetStats()) {
                Debug.Log(stat.Key + ": " + stat.Value);
            }
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


            // TODO

            return null;

            //return components.Aggregate(new Dictionary<string, int>(), (lookups, type) => {
            //    var lookupTags = TypeReflectionProvider.GetContexts(type, false);
            //    if(lookupTags.Length == 0) {
            //        lookupTags = new [] { "Context" };
            //    }
            //    foreach(var lookupTag in lookupTags) {
            //        if(!lookups.ContainsKey(lookupTag)) {
            //            lookups.Add(lookupTag, 0);
            //        }

            //        lookups[lookupTag] += 1;
            //    }
            //    return lookups;
            //});
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
