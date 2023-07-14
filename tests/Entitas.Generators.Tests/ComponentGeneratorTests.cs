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

        static readonly string FixturesPath = Path.Combine(ProjectRoot,
            "tests", "Entitas.Generators.Tests.Fixtures");

        static Task Verify(string fixture, Dictionary<string, string> options) => TestHelper.Verify(File.ReadAllText(
            Path.Combine(FixturesPath, $"{fixture}.cs")), new ComponentGenerator(), options);

        static Task VerifyComponent(string fixture, Dictionary<string, string> options) => TestHelper.Verify(File.ReadAllText(
            Path.Combine(FixturesPath, "Components", $"{fixture}.cs")), new ComponentGenerator(), options);

        static Task VerifyContext(string fixture, Dictionary<string, string> options) => TestHelper.Verify(File.ReadAllText(
            Path.Combine(FixturesPath, "Contexts", $"{fixture}.txt")), new ComponentGenerator(), options);

        static readonly Dictionary<string, string> DefaultOptions = new Dictionary<string, string>();

        static readonly Dictionary<string, string> NoComponentIndexNoMatcherOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentComponentIndexKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentMatcherKey, "false" }
        };

        static readonly Dictionary<string, string> EventsOnlyOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentComponentIndexKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentMatcherKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEntityExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentContextExtensionKey, "false" }
        };

        static readonly Dictionary<string, string> ContextInitializationOptions = new Dictionary<string, string>
        {
            { EntitasAnalyzerConfigOptions.ComponentComponentIndexKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentMatcherKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEntityExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentContextExtensionKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventsKey, "false" },
            { EntitasAnalyzerConfigOptions.ComponentEventSystemsContextExtensionKey, "false" }
        };

        [Fact]
        public void UsesGlobalNamespace()
        {
            AssertUsesGlobalNamespaces("gen/Entitas.Generators/Component/ComponentGenerator.cs");
            AssertUsesGlobalNamespaces("gen/Entitas.Generators/Component/ComponentGenerator.ComponentIndex.cs");
            AssertUsesGlobalNamespaces("gen/Entitas.Generators/Component/ComponentGenerator.ContextExtension.cs");
            AssertUsesGlobalNamespaces("gen/Entitas.Generators/Component/ComponentGenerator.EntityExtension.cs");
            AssertUsesGlobalNamespaces("gen/Entitas.Generators/Component/ComponentGenerator.Events.cs");
            AssertUsesGlobalNamespaces("gen/Entitas.Generators/Component/ComponentGenerator.Matcher.cs");
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
         * Valid components
         *
         */

        [Fact]
        public Task NonPublicComponent() => VerifyComponent("NonPublicComponent", DefaultOptions);

        [Fact]
        public Task NamespacedComponent() => VerifyComponent("SomeNamespacedComponent", DefaultOptions);

        [Fact]
        public Task Component() => VerifyComponent("SomeComponent", DefaultOptions);

        [Fact]
        public Task NoContextNamespacedComponent() => VerifyComponent("NoContextNamespacedComponent", DefaultOptions);

        [Fact]
        public Task NoContextComponent() => VerifyComponent("NoContextComponent", DefaultOptions);

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

        /*
         *
         * Invalid usages
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
