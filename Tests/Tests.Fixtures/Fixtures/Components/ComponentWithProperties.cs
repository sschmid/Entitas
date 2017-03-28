using Entitas;
using Entitas.CodeGenerator.Attributes;

[Context("Test")]
public class ComponentWithProperties : IComponent {

    // Has one public property
    [TestMember("myProperty")]
    public string publicProperty { get; set; }

    // Should be ignored
    public static bool publicStaticProperty { get; set; }
    bool _privateProperty { get; set; }
    static bool _privateStaticProperty { get; set; }

    public string publicPropertyGet { get { return null; } }
    public string publicPropertySet { set { } }
}
