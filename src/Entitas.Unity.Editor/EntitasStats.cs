using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;
using Entitas.Generators.Attributes;
using UnityEditor;
using UnityEngine;

namespace Entitas.Unity.Editor
{
    public static class EntitasStats
    {
        [MenuItem("Tools/Entitas/Show Stats", false, 200)]
        public static void ShowStats()
        {
            var stats = string.Join("\n", GetStats().Select(kvp => $"{kvp.Key}: {kvp.Value}"));
            Debug.Log(stats);
            EditorUtility.DisplayDialog("Entitas Stats", stats, "Close");
        }

        public static Dictionary<string, int> GetStats()
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
