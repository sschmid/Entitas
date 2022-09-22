using Entitas;
using Entitas.CodeGeneration.Attributes;

[Context("Test")]
public class ComponentWithProperties : IComponent
{
    // Has one public property
    [TestMember("myProperty")]
    public string publicProperty { get; set; }

    // Should be ignored
    public static bool publicStaticProperty { get; set; }
    bool _privateProperty { get; set; }
    static bool _privateStaticProperty { get; set; }

    public string publicPropertyGet => null;

    public string publicPropertySet
    {
        set { }
    }
}
