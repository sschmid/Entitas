namespace Entitas.Generators.Tests;

[UsesVerify]
public class ComponentGeneratorTests
{
    static string FixturesPath => Path.Combine(TestExtensions.GetProjectRoot(),
        "tests", "Entitas.Generators.Tests.Fixtures");

    [Fact]
    public Task Test() => TestHelper.Verify(
        GetFixture("NamespacedComponentWithOneField"),
        new ComponentGenerator());

    static string GetFixture(string name) => File.ReadAllText(Path.Combine(FixturesPath, $"{name}.cs"));
}
