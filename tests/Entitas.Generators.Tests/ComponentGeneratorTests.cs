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
    public Task NamespacedClass() => Verify("NamespacedClass");

    [Fact]
    public Task Class() => Verify("SomeClass");

    [Fact]
    public Task NonPartialComponent() => VerifyComponent("NonPartialComponent");

    [Fact]
    public Task NamespacedComponent() => VerifyComponent("NamespacedComponent");

    [Fact]
    public Task Component() => VerifyComponent("SomeComponent");

    [Fact]
    public Task OneContextNamespacedComponent() => VerifyComponent("OneContextNamespacedComponent");
}
