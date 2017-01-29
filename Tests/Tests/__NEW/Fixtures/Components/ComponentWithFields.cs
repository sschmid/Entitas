using Entitas.Api;

[Test]
public class ComponentWithFields : IComponent {

    // Has one public field
    [TestMember("myField")]
    public string publicField;

    // Should be ignored
    public static bool publicStaticField;
    bool _privateField;
    static bool _privateStaticField;
}
