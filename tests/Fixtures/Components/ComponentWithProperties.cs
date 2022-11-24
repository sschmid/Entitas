using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1")]
public class ComponentWithProperties : IComponent
{
    // Has one public property
    [TestMember("MyProperty")]
    public string PublicProperty { get; set; }

    // Should be ignored
    public static bool PublicStaticProperty { get; set; }
    bool _privateProperty { get; set; }
    static bool _privateStaticProperty { get; set; }

    public string PublicPropertyGet => null;

    public string PublicPropertySet
    {
        set { }
    }
}
