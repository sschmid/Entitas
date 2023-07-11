using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Entitas.Generators.Tests
{
    [UsesVerify]
    public class ComponentGeneratorTests
    {
        static readonly string ProjectRoot = TestExtensions.GetProjectRoot();

        static readonly string FixturesPath = Path.Combine(ProjectRoot,
            "tests", "Entitas.Generators.Tests.Fixtures");

        static Task Verify(string fixture) => TestHelper.Verify(File.ReadAllText(
            Path.Combine(FixturesPath, $"{fixture}.cs")), new ComponentGenerator());

        static Task VerifyComponent(string fixture) => TestHelper.Verify(File.ReadAllText(
            Path.Combine(FixturesPath, "Components", $"{fixture}.cs")), new ComponentGenerator());

        static Task VerifyContext(string fixture) => TestHelper.Verify(File.ReadAllText(
            Path.Combine(FixturesPath, "Contexts", $"{fixture}.txt")), new ComponentGenerator());

        [Fact]
        public void UsesGlobalNamespace()
        {
            TestHelper.AssertUsesGlobalNamespaces(File.ReadAllText(Path.Combine(ProjectRoot,
                "gen/Entitas.Generators/Component/ComponentGenerator.cs")));
        }

        /*
         *
         * Non components
         *
         */

        [Fact]
        public Task SomeNamespacedClass() => Verify("SomeNamespacedClass");

        [Fact]
        public Task Class() => Verify("SomeClass");

        /*
         *
         * Valid components
         *
         */

        [Fact]
        public Task NonPublicComponent() => VerifyComponent("NonPublicComponent");

        [Fact]
        public Task NamespacedComponent() => VerifyComponent("SomeNamespacedComponent");

        [Fact]
        public Task Component() => VerifyComponent("SomeComponent");

        [Fact]
        public Task NoContextNamespacedComponent() => VerifyComponent("NoContextNamespacedComponent");

        [Fact]
        public Task NoContextComponent() => VerifyComponent("NoContextComponent");

        [Fact]
        public Task OneFieldNamespacedComponent() => VerifyComponent("OneFieldNamespacedComponent");

        [Fact]
        public Task OneFieldComponent() => VerifyComponent("OneFieldComponent");

        [Fact]
        public Task MultipleFieldsNamespacedComponent() => VerifyComponent("MultipleFieldsNamespacedComponent");

        [Fact]
        public Task MultipleFieldsComponent() => VerifyComponent("MultipleFieldsComponent");

        [Fact]
        public Task ReservedKeywordFieldsNamespacedComponent() => VerifyComponent("ReservedKeywordFieldsNamespacedComponent");

        [Fact]
        public Task NoValidFieldsNamespacedComponent() => VerifyComponent("NoValidFieldsNamespacedComponent");

        [Fact]
        public Task MultiplePropertiesNamespacedComponent() => VerifyComponent("MultiplePropertiesNamespacedComponent");

        [Fact]
        public Task ContextFromDifferentAssemblyNamespacedComponent() => VerifyComponent("ContextFromDifferentAssemblyNamespacedComponent");

        [Fact]
        public Task UniqueNamespacedComponent() => VerifyComponent("UniqueNamespacedComponent");

        [Fact]
        public Task UniqueOneFieldNamespacedComponent() => VerifyComponent("UniqueOneFieldNamespacedComponent");

        /*
         *
         * Events
         *
         */

        [Fact]
        public Task EventNamespacedComponent() => VerifyComponent("EventNamespacedComponent");

        /*
         *
         * Invalid usages
         *
         */

        [Fact]
        public Task DuplicatedContextsNamespacedComponent() => VerifyComponent("DuplicatedContextsNamespacedComponent");

        /*
         *
         * ContextInitialization
         *
         */

        [Fact]
        public Task EmptyContextInitialization() => VerifyContext("EmptyContextInitialization");

        [Fact]
        public Task ContextInitialization() => VerifyContext("ContextInitialization");

        [Fact]
        public Task ContextInitializationFromDifferentAssembly() => VerifyContext("ContextInitializationFromDifferentAssembly");
    }
}
