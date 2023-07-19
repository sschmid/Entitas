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
        static readonly string ProjectRoot = TestExtensions.GetProjectRoot();

        static readonly string FixturesPath = Path.Combine(ProjectRoot, "tests", "Entitas.Generators.Tests.Fixtures");

        static Task Verify(string fixture, Dictionary<string, string> options) =>
            TestHelper.Verify(File.ReadAllText(Path.Combine(FixturesPath, $"{fixture}.cs")), new ComponentGenerator(), options);

        static Task VerifyComponent(string fixture, Dictionary<string, string> options) =>
            TestHelper.Verify(File.ReadAllText(Path.Combine(FixturesPath, "Components", $"{fixture}.cs")), new ComponentGenerator(), options);

        static Task VerifyContext(string fixture, Dictionary<string, string> options) =>
            TestHelper.Verify(File.ReadAllText(Path.Combine(FixturesPath, "Contexts", $"{fixture}.txt")), new ComponentGenerator(), options);

        static readonly Dictionary<string, string> DefaultOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentEventSystemsExtensionKey, "false" }
        };

        static readonly Dictionary<string, string> NoComponentIndexNoMatcherOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentComponentIndexKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventSystemsExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentMatcherKey, "false" }
        };

        static readonly Dictionary<string, string> CleanupOnlyOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentComponentIndexKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentContextExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentContextInitializationMethodKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEntityExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEntityIndexExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventsKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventSystemsExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentMatcherKey, "false" }
        };

        static readonly Dictionary<string, string> EventsOnlyOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentComponentIndexKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentContextExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEntityExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventSystemsExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentMatcherKey, "false" }
        };

        static readonly Dictionary<string, string> EventSystemsOnlyOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentComponentIndexKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentContextExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentContextInitializationMethodKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEntityExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventsKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentMatcherKey, "false" }
        };

        static readonly Dictionary<string, string> EntityIndexOnlyOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentComponentIndexKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentContextExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentContextInitializationMethodKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEntityExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventsKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventSystemsExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentMatcherKey, "false" }
        };

        static readonly Dictionary<string, string> ContextInitializationOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentComponentIndexKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentContextExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEntityExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventsKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventSystemsExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentMatcherKey, "false" }
        };

        [Theory]
        [InlineData("ComponentGenerator")]
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
        public Task OneFieldNamespacedComponent() => VerifyComponent("OneFieldNamespacedComponent", NoComponentIndexNoMatcherOptions);

        [Fact]
        public Task OneFieldComponent() => VerifyComponent("OneFieldComponent", NoComponentIndexNoMatcherOptions);

        [Fact]
        public Task MultipleFieldsNamespacedComponent() => VerifyComponent("MultipleFieldsNamespacedComponent", NoComponentIndexNoMatcherOptions);

        [Fact]
        public Task MultipleFieldsComponent() => VerifyComponent("MultipleFieldsComponent", NoComponentIndexNoMatcherOptions);

        [Fact]
        public Task ReservedKeywordFieldsNamespacedComponent() => VerifyComponent("ReservedKeywordFieldsNamespacedComponent", NoComponentIndexNoMatcherOptions);

        [Fact]
        public Task NoValidFieldsNamespacedComponent() => VerifyComponent("NoValidFieldsNamespacedComponent", NoComponentIndexNoMatcherOptions);

        [Fact]
        public Task MultiplePropertiesNamespacedComponent() => VerifyComponent("MultiplePropertiesNamespacedComponent", NoComponentIndexNoMatcherOptions);

        [Fact]
        public Task ContextFromDifferentAssemblyNamespacedComponent() => VerifyComponent("ContextFromDifferentAssemblyNamespacedComponent", NoComponentIndexNoMatcherOptions);

        [Fact]
        public Task UniqueNamespacedComponent() => VerifyComponent("UniqueNamespacedComponent", NoComponentIndexNoMatcherOptions);

        [Fact]
        public Task UniqueOneFieldNamespacedComponent() => VerifyComponent("UniqueOneFieldNamespacedComponent", NoComponentIndexNoMatcherOptions);

        [Fact]
        public Task CleanupRemoveNamespacedComponent() => VerifyComponent("CleanupRemoveNamespacedComponent", CleanupOnlyOptions);

        [Fact]
        public Task CleanupRemoveComponent() => VerifyComponent("CleanupRemoveComponent", CleanupOnlyOptions);

        [Fact]
        public Task CleanupDestroyEntityNamespacedComponent() => VerifyComponent("CleanupDestroyEntityNamespacedComponent", CleanupOnlyOptions);

        [Fact]
        public Task CleanupDestroyEntityComponent() => VerifyComponent("CleanupDestroyEntityComponent", CleanupOnlyOptions);

        /*
         *
         * Events
         *
         */

        [Fact]
        public Task EventNamespacedComponent() => VerifyComponent("EventNamespacedComponent", EventsOnlyOptions);

        [Fact]
        public Task EventComponent() => VerifyComponent("EventComponent", EventsOnlyOptions);

        [Fact]
        public Task FlagEventNamespacedComponent() => VerifyComponent("FlagEventNamespacedComponent", EventsOnlyOptions);

        [Fact]
        public Task FlagEventComponent() => VerifyComponent("FlagEventComponent", EventsOnlyOptions);

        [Fact]
        public Task EventSystems() => VerifyComponent("EventComponent", EventSystemsOnlyOptions);

        /*
         *
         * Entity Index
         *
         */

        [Fact]
        public Task NoEntityIndexNamespacedComponent() => VerifyComponent("SomeNamespacedComponent", EntityIndexOnlyOptions);

        [Fact]
        public Task EntityIndexNamespacedComponent() => VerifyComponent("EntityIndexNamespacedComponent", EntityIndexOnlyOptions);

        [Fact]
        public Task EntityIndexComponent() => VerifyComponent("EntityIndexComponent", EntityIndexOnlyOptions);

        [Fact]
        public Task PrimaryEntityIndexNamespacedComponent() => VerifyComponent("PrimaryEntityIndexNamespacedComponent", EntityIndexOnlyOptions);

        [Fact]
        public Task PrimaryEntityIndexComponent() => VerifyComponent("PrimaryEntityIndexComponent", EntityIndexOnlyOptions);

        /*
         *
         * Invalid usages (but works anyway)
         *
         */

        [Fact]
        public Task DuplicatedContextsNamespacedComponent() => VerifyComponent("DuplicatedContextsNamespacedComponent", NoComponentIndexNoMatcherOptions);

        /*
         *
         * ContextInitialization
         *
         */

        [Fact]
        public Task EmptyContextInitialization() => VerifyContext("EmptyContextInitialization", ContextInitializationOptions);

        [Fact]
        public Task ContextInitialization() => VerifyContext("ContextInitialization", ContextInitializationOptions);

        [Fact]
        public Task ContextInitializationFromDifferentAssembly() => VerifyContext("ContextInitializationFromDifferentAssembly", ContextInitializationOptions);
    }
}
