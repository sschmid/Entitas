using Entitas.CodeGeneration.Plugins;
using NSpec;
using Shouldly;

class describe_CodeGeneratorExtentions : nspec
{
    void when_extension()
    {
        context["wraps in namespace"] = () =>
        {
            it["returns same string when namespace is null"] = () => "Test".WrapInNamespace(null).ShouldBe("Test");
            it["returns same string when namespace is empty"] = () => "Test".WrapInNamespace(string.Empty).ShouldBe("Test");

            it["indents by 4 spaces"] = () =>
            {
                var s = "Test".WrapInNamespace("MyNamespace");
                s.ShouldBe(@"namespace MyNamespace
{
    Test
}
");
            };

            it["indents multiple lines"] = () =>
            {
                var s = "Test\nTest\nTest".WrapInNamespace("MyNamespace");
                s.ShouldBe(@"namespace MyNamespace
{
    Test
    Test
    Test
}
");
            };

            it["keeps empty lines"] = () =>
            {
                var s = "Test\n\nTest\n\nTest".WrapInNamespace("MyNamespace");
                s.ShouldBe(@"namespace MyNamespace
{
    Test

    Test

    Test
}
");
            };

            it["combines multiple namespaces"] = () =>
            {
                var s = "Test".WrapInNamespace("MyNamespace", "Module");
                s.ShouldBe(@"namespace MyNamespace.Module
{
    Test
}
");
            };
        };
    }
}
