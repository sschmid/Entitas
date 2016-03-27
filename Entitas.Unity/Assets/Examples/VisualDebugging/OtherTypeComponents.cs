using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

[VisualDebugging]
public class CustomObject {
    public string name;

    public CustomObject(string name) {
        this.name = name;
    }
}

[VisualDebugging]
public class MonoBehaviourSubClass : MonoBehaviour {
}

[VisualDebugging]
public class MonoBehaviourSubClassComponent : IComponent {
    public MonoBehaviourSubClass monoBehaviour;
}

[VisualDebugging]
public class CustomObjectComponent : IComponent {
    public CustomObject customObject;
}

[VisualDebugging]
public class SystemObjectComponent : IComponent {
    public System.Object systemObject;
}

[VisualDebugging]
public class DateTimeComponent : IComponent {
    public DateTime date;
}

[VisualDebugging]
public class AnArrayComponent : IComponent {
    public string[] array;
}

[VisualDebugging]
public class Array2DComponent : IComponent {
    public string[,] array2d;
}

[VisualDebugging]
public class Array3DComponent : IComponent {
    public string[,,] array3d;
}

[VisualDebugging]
public class JaggedArrayComponent : IComponent {
    public string[][] jaggedArray;
}

[VisualDebugging]
public class ListArrayComponent : IComponent {
    public List<string>[] listArray;
}

[VisualDebugging]
public class ListComponent : IComponent {
    public List<string> list;
}

[VisualDebugging]
public class DictionaryComponent : IComponent {
    public Dictionary<string, string> dict;
}

[VisualDebugging]
public class DictArrayComponent : IComponent {
    public Dictionary<int, string[]> dict;
    public Dictionary<int, string[]>[] dictArray;
}

[VisualDebugging]
public class HashSetComponent : IComponent {
    public HashSet<string> hashset;
}

[VisualDebugging]
public class UnsupportedObject {
    public string name;

    public UnsupportedObject(string name) {
        this.name = name;
    }
}

[VisualDebugging]
public class UnsupportedObjectComponent : IComponent {
    public UnsupportedObject unsupportedObject;
}

[VisualDebugging]
public class PropertyComponent : IComponent {
    public string value { get; set; }
}

