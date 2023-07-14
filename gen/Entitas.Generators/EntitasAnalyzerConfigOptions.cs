using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Entitas.Generators
{
    public static class EntitasAnalyzerConfigOptions
    {
        public const string ComponentComponentIndexKey = "entitas_component_generator_component_index";
        public const string ComponentMatcherKey = "entitas_component_generator_matcher";
        public const string ComponentEntityExtensionKey = "entitas_component_generator_entity_extension";
        public const string ComponentContextExtensionKey = "entitas_component_generator_context_extension";
        public const string ComponentEventsKey = "entitas_component_generator_events";
        public const string ComponentContextInitializationMethodKey = "entitas_component_generator_context_initialization_method";
        public const string ComponentEventSystemsContextExtensionKey = "entitas_component_generator_event_systems_context_extension";

        public const string ContextComponentIndexKey = "entitas_context_generator_component_index";
        public const string ContextEntityKey = "entitas_context_generator_entity";
        public const string ContextMatcherKey = "entitas_context_generator_matcher";
        public const string ContextContextKey = "entitas_context_generator_context";

        public static bool ComponentComponentIndex(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ComponentComponentIndexKey);
        public static bool ComponentMatcher(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ComponentMatcherKey);
        public static bool ComponentEntityExtension(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ComponentEntityExtensionKey);
        public static bool ComponentContextExtension(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ComponentContextExtensionKey);
        public static bool ComponentEvents(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ComponentEventsKey);
        public static bool ComponentContextInitializationMethod(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ComponentContextInitializationMethodKey);
        public static bool ComponentEventSystemsContextExtension(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ComponentEventSystemsContextExtensionKey);

        public static bool ContextComponentIndex(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ContextComponentIndexKey);
        public static bool ContextEntity(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ContextEntityKey);
        public static bool ContextMatcher(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ContextMatcherKey);
        public static bool ContextContext(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree) => IsTrue(optionsProvider, syntaxTree, ContextContextKey);

        static bool IsTrue(AnalyzerConfigOptionsProvider optionsProvider, SyntaxTree? syntaxTree, string key)
        {
            return syntaxTree is null || !optionsProvider.GetOptions(syntaxTree).TryGetValue(key, out var value) || value.Equals("true", StringComparison.OrdinalIgnoreCase);
        }
    }
}
