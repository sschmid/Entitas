using System;
using System.Collections.Generic;
using System.Linq;
using DesperateDevs.Extensions;
using DesperateDevs.Reflection;
using Entitas.CodeGeneration.Attributes;
using UnityEditor;
using UnityEngine;

namespace Entitas.VisualDebugging.Unity.Editor
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
                .Where(isSystem)
                .ToArray();

            var contexts = GetContexts(components);

            var stats = new Dictionary<string, int>
            {
                {"Total Components", components.Length},
                {"Systems", systems.Length}
            };

            foreach (var context in contexts)
                stats.Add($"Components in {context.Key}", context.Value);

            return stats;
        }

        static Dictionary<string, int> GetContexts(Type[] components) => components
            .Aggregate(new Dictionary<string, int>(), (contexts, type) =>
            {
                foreach (var context in GetContextOrDefault(type))
                {
                    if (!contexts.ContainsKey(context))
                        contexts.Add(context, 0);

                    contexts[context] += 1;
                }

                return contexts;
            });

        static string[] GetContexts(Type type) => Attribute
            .GetCustomAttributes(type)
            .OfType<ContextAttribute>()
            .Select(attr => attr.Name)
            .ToArray();

        static string[] GetContextOrDefault(Type type)
        {
            var contexts = GetContexts(type);
            if (contexts.Length == 0)
                contexts = new[] {"Default"};

            return contexts;
        }

        static bool isSystem(Type type) =>
            type.ImplementsInterface<ISystem>()
            && type != typeof(ReactiveSystem<>)
            && type != typeof(MultiReactiveSystem<,>)
            && type != typeof(Systems)
            && type != typeof(DebugSystems)
            && type != typeof(JobSystem<>)
            && type.FullName != "Feature";
    }
}
