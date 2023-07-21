using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;
using static Entitas.Generators.Tests.TestHelper;

namespace Entitas.Generators.Tests
{
    [UsesVerify]
    public class ComponentGeneratorTests
    {
        static readonly string FixturesPath = Path.Combine(ProjectRoot, "tests", "Entitas.Generators.Tests", "fixtures");

        static Task Verify(string fixture, Dictionary<string, string> options) =>
            TestHelper.Verify(File.ReadAllText(Path.Combine(FixturesPath, $"{fixture}.cs")), new ComponentGenerator(), options);

        static Task VerifyComponent(string fixture, Dictionary<string, string> options) =>
            TestHelper.Verify(File.ReadAllText(Path.Combine(FixturesPath, "Components", $"{fixture}.cs")), new ComponentGenerator(), options);

        static Task VerifyContext(string fixture, Dictionary<string, string> options) =>
            TestHelper.Verify(File.ReadAllText(Path.Combine(FixturesPath, "Contexts", $"{fixture}.txt")), new ComponentGenerator(), options);

        static readonly Dictionary<string, string> DefaultOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentCleanupSystemsKey, "true" },
            { EntitasAnalyzerConfigOptions.ComponentComponentIndexKey, "true" },
            { EntitasAnalyzerConfigOptions.ComponentContextExtensionKey, "true" },
            { EntitasAnalyzerConfigOptions.ComponentContextInitializationMethodKey, "true" },
            { EntitasAnalyzerConfigOptions.ComponentEntityExtensionKey, "true" },
            { EntitasAnalyzerConfigOptions.ComponentEntityIndexExtensionKey, "true" },
            { EntitasAnalyzerConfigOptions.ComponentEventsKey, "true" },
            { EntitasAnalyzerConfigOptions.ComponentEventSystemsExtensionKey, "true" },
            { EntitasAnalyzerConfigOptions.ComponentMatcherKey, "true" }
        };

        static readonly Dictionary<string, string> EntityExtensionOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentEntityExtensionKey, "true" }
        };

        static readonly Dictionary<string, string> ContextExtensionOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentContextExtensionKey, "true" }
        };

        static readonly Dictionary<string, string> CleanupSystemsOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentCleanupSystemsKey, "true" }
        };

        static readonly Dictionary<string, string> EventsOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentEventsKey, "true" }
        };

        static readonly Dictionary<string, string> EventSystemsExtensionOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentEventSystemsExtensionKey, "true" }
        };

        static readonly Dictionary<string, string> EntityIndexExtensionOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentEntityIndexExtensionKey, "true" }
        };

        static readonly Dictionary<string, string> ContextInitializationMethodOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentContextInitializationMethodKey, "true" }
        };

        [Theory]
        [InlineData("ComponentGenerator")]
        [InlineData("ComponentGenerator.CleanupSystem")]
        [InlineData("ComponentGenerator.CleanupSystems")]
        [InlineData("ComponentGenerator.ComponentIndex")]
        [InlineData("ComponentGenerator.ContextExtension")]
        [InlineData("ComponentGenerator.ContextInitializationMethod")]
        [InlineData("ComponentGenerator.EntityExtension")]
        [InlineData("ComponentGenerator.EntityIndexExtension")]
        [InlineData("ComponentGenerator.Events")]
        [InlineData("ComponentGenerator.EventSystems")]
        [InlineData("ComponentGenerator.Matcher")]
        public void UsesGlobalNamespace(string path)
        {
            AssertUsesGlobalNamespaces(Path.Combine("gen", "Entitas.Generators", "Component", path + ".cs"));
        }

        /*
         *
         * Non components
         *
         */

        [Fact]
        public Task SomeNamespacedClass() => Verify("SomeNamespacedClass", DefaultOptions);

        [Fact]
        public Task Class() => Verify("SomeClass", DefaultOptions);

        /*
         *
         * Invalid components
         *
         */

        [Fact]
        public Task NonPublicComponent() => VerifyComponent("NonPublicComponent", DefaultOptions);

        [Fact]
        public Task NoContextComponent() => VerifyComponent("NoContextComponent", DefaultOptions);

        /*
         *
         * Valid components
         *
         */

        [Fact]
        public Task NamespacedComponent() => VerifyComponent("SomeNamespacedComponent", DefaultOptions);

        [Fact]
        public Task Component() => VerifyComponent("SomeComponent", DefaultOptions);

        [Fact]
        public Task OneFieldNamespacedComponent() => VerifyComponent("OneFieldNamespacedComponent", EntityExtensionOptions);

        [Fact]
        public Task OneFieldComponent() => VerifyComponent("OneFieldComponent", EntityExtensionOptions);

        [Fact]
        public Task MultipleFieldsNamespacedComponent() => VerifyComponent("MultipleFieldsNamespacedComponent", EntityExtensionOptions);

        [Fact]
        public Task MultipleFieldsComponent() => VerifyComponent("MultipleFieldsComponent", EntityExtensionOptions);

        [Fact]
        public Task ReservedKeywordFieldsNamespacedComponent() => VerifyComponent("ReservedKeywordFieldsNamespacedComponent", EntityExtensionOptions);

        [Fact]
        public Task NoValidFieldsNamespacedComponent() => VerifyComponent("NoValidFieldsNamespacedComponent", EntityExtensionOptions);

        [Fact]
        public Task MultiplePropertiesNamespacedComponent() => VerifyComponent("MultiplePropertiesNamespacedComponent", EntityExtensionOptions);

        [Fact]
        public Task ContextFromDifferentAssemblyNamespacedComponent() => VerifyComponent("ContextFromDifferentAssemblyNamespacedComponent", EntityExtensionOptions);

        [Fact]
        public Task UniqueNamespacedComponent() => VerifyComponent("UniqueNamespacedComponent", ContextExtensionOptions);

        [Fact]
        public Task UniqueOneFieldNamespacedComponent() => VerifyComponent("UniqueOneFieldNamespacedComponent", ContextExtensionOptions);

        [Fact]
        public Task CleanupRemoveNamespacedComponent() => VerifyComponent("CleanupRemoveNamespacedComponent", CleanupSystemsOptions);

        [Fact]
        public Task CleanupRemoveComponent() => VerifyComponent("CleanupRemoveComponent", CleanupSystemsOptions);

        [Fact]
        public Task CleanupDestroyEntityNamespacedComponent() => VerifyComponent("CleanupDestroyEntityNamespacedComponent", CleanupSystemsOptions);

        [Fact]
        public Task CleanupDestroyEntityComponent() => VerifyComponent("CleanupDestroyEntityComponent", CleanupSystemsOptions);

        [Fact]
        public Task CleanupSystems() => VerifyContext("ContextInitialization", CleanupSystemsOptions);

        [Fact]
        public Task NoCleanupSystems() => VerifyContext("EmptyContextInitialization", CleanupSystemsOptions);

        [Fact]
        public Task ComplexTypesComponent() => VerifyComponent("ComplexTypesComponent", EntityExtensionOptions);

        /*
         *
         * Events
         *
         */

        [Fact]
        public Task EventNamespacedComponent() => VerifyComponent("EventNamespacedComponent", EventsOptions);

        [Fact]
        public Task EventComponent() => VerifyComponent("EventComponent", EventsOptions);

        [Fact]
        public Task FlagEventNamespacedComponent() => VerifyComponent("FlagEventNamespacedComponent", EventsOptions);

        [Fact]
        public Task FlagEventComponent() => VerifyComponent("FlagEventComponent", EventsOptions);

        [Fact]
        public Task EventSystems() => VerifyContext("ContextInitialization", EventSystemsExtensionOptions);

        [Fact]
        public Task NoEventSystems() => VerifyContext("EmptyContextInitialization", EventSystemsExtensionOptions);

        /*
         *
         * Entity Index
         *
         */

        [Fact]
        public Task EntityIndexes() => VerifyContext("ContextInitialization", EntityIndexExtensionOptions);

        [Fact]
        public Task NoEntityIndexes() => VerifyContext("EmptyContextInitialization", EntityIndexExtensionOptions);

        /*
         *
         * Invalid usages (but works anyway)
         *
         */

        [Fact]
        public Task DuplicatedContextsNamespacedComponent() => VerifyComponent("DuplicatedContextsNamespacedComponent", EntityExtensionOptions);

        /*
         *
         * ContextInitialization
         *
         */

        [Fact]
        public Task EmptyContextInitialization() => VerifyContext("EmptyContextInitialization", ContextInitializationMethodOptions);

        [Fact]
        public Task ContextInitialization() => VerifyContext("ContextInitialization", ContextInitializationMethodOptions);

        [Fact]
        public Task ContextInitializationFromDifferentAssembly() => VerifyContext("ContextInitializationFromDifferentAssembly", ContextInitializationMethodOptions);
    }
}
