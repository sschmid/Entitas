using Entitas;
using Entitas.Plugins.Attributes;

[Context("Test1")]
public class ComponentWithFields : IComponent
{
    // Has one public field
    [TestMember("MyField")]
    public string PublicField;

    // Should be ignored
#pragma warning disable
    public static bool PublicStaticField;
    bool _privateField;
    static bool _privateStaticField;
}
