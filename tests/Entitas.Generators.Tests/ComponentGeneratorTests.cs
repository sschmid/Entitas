using System.IO;
using System.Threading.Tasks;
using VerifyXunit;
using Xunit;

namespace Entitas.Generators.Tests;

[UsesVerify]
public class ComponentGeneratorTests
{
    static string FixturesPath => Path.Combine(TestExtensions.GetProjectRoot(),
        "tests", "Entitas.Generators.Tests.Fixtures");

    static Task Verify(string fixture) => TestHelper.Verify(File.ReadAllText(
        Path.Combine(FixturesPath, $"{fixture}.cs")), new ComponentGenerator());

    static Task VerifyComponent(string fixture) => TestHelper.Verify(File.ReadAllText(
        Path.Combine(FixturesPath, "Components", $"{fixture}.cs")), new ComponentGenerator());

    [Fact]
    public Task SomeNamespacedClass() => Verify("SomeNamespacedClass");

    [Fact]
    public Task Class() => Verify("SomeClass");

    [Fact]
    public Task NonPartialComponent() => VerifyComponent("NonPartialComponent");

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
}
