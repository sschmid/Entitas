public static class TestComponentsLookup {

    public const int ClassToGenerate = 0;
    public const int ComponentWithFields = 1;
    public const int ComponentWithFieldsAndProperties = 2;
    public const int ComponentWithProperties = 3;
    public const int CustomPrefixFlag = 4;
    public const int DontGenerateMethods = 5;
    public const int Flag = 6;
    public const int InterfaceToGenerate = 7;
    public const int MyNamespace = 8;
    public const int MyNamespaceFlag = 9;
    public const int NameAge = 10;
    public const int NewCustomNameComponent1 = 11;
    public const int NewCustomNameComponent2 = 12;
    public const int Standard = 13;
    public const int StructToGenerate = 14;
    public const int Test2Context = 15;
    public const int UniqueFlag = 16;
    public const int UniqueMyNamespace = 17;
    public const int UniqueMyNamespaceFlag = 18;
    public const int UniqueStandard = 19;

    public const int TotalComponents = 20;

    public static readonly string[] componentNames = {
        "ClassToGenerate",
        "ComponentWithFields",
        "ComponentWithFieldsAndProperties",
        "ComponentWithProperties",
        "CustomPrefixFlag",
        "DontGenerateMethods",
        "Flag",
        "InterfaceToGenerate",
        "MyNamespace",
        "MyNamespaceFlag",
        "NameAge",
        "NewCustomNameComponent1",
        "NewCustomNameComponent2",
        "Standard",
        "StructToGenerate",
        "Test2Context",
        "UniqueFlag",
        "UniqueMyNamespace",
        "UniqueMyNamespaceFlag",
        "UniqueStandard"
    };

    public static readonly System.Type[] componentTypes = {
        typeof(ClassToGenerateComponent),
        typeof(ComponentWithFields),
        typeof(ComponentWithFieldsAndProperties),
        typeof(ComponentWithProperties),
        typeof(CustomPrefixFlagComponent),
        typeof(DontGenerateMethodsComponent),
        typeof(FlagComponent),
        typeof(InterfaceToGenerateComponent),
        typeof(My.Namespace.MyNamespaceComponent),
        typeof(My.Namespace.MyNamespaceFlagComponent),
        typeof(NameAgeComponent),
        typeof(NewCustomNameComponent1Component),
        typeof(NewCustomNameComponent2Component),
        typeof(StandardComponent),
        typeof(StructToGenerateComponent),
        typeof(Test2ContextComponent),
        typeof(UniqueFlagComponent),
        typeof(My.Namespace.UniqueMyNamespaceComponent),
        typeof(My.Namespace.UniqueMyNamespaceFlagComponent),
        typeof(UniqueStandardComponent)
    };
}
