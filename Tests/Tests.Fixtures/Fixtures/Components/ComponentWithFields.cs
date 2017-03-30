using Entitas;
using Entitas.CodeGenerator.Attributes;

[Context("Test")]
public class ComponentWithFields : IComponent {

    // Has one public field
    [TestMember("myField")]
    public string publicField;

    // Should be ignored
    #pragma warning disable
    public static bool publicStaticField;
    bool _privateField;
    static bool _privateStaticField;
}
