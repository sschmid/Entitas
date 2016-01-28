public class ComponentInfo {

    public string type { get { return _type; } }
    public ComponentFieldInfo[] fieldInfos { get { return _fieldInfos; } }
    public string[] pools { get { return _pools; } }
    public bool isSingleEntity { get { return _isSingleEntity; } }
    public string singleComponentPrefix { get { return _singleComponentPrefix; } }
    public bool generateMethods { get { return _generateMethods; } }
    public bool generateIndex { get { return _generateIndex; } }

    public bool isSingletonComponent { get { return _fieldInfos.Length == 0; } }

    readonly string _type;
    readonly ComponentFieldInfo[] _fieldInfos;
    readonly string[] _pools;
    readonly bool _isSingleEntity;
    readonly string _singleComponentPrefix;
    readonly bool _generateMethods;
    readonly bool _generateIndex;

    public ComponentInfo(string type, ComponentFieldInfo[] fieldInfos, string[] pools, bool isSingleEntity, string singleComponentPrefix,
        bool generateMethods, bool generateIndex) {
        _type = type;
        _fieldInfos = fieldInfos;
        _pools = pools;
        _isSingleEntity = isSingleEntity;
        _singleComponentPrefix = singleComponentPrefix;
        _generateMethods = generateMethods;
        _generateIndex = generateIndex;
    }
}

public class ComponentFieldInfo {

    public string type { get { return _type; } }
    public string name{ get { return _name; } }

    readonly string _type;
    readonly string _name;

    public ComponentFieldInfo(string type, string name) {
        _type = type;
        _name = name;
    }
}
