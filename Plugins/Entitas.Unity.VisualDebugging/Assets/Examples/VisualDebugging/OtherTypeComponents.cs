using System;
using System.Collections.Generic;
using Entitas;
using UnityEngine;

public class CustomObject {
    public string name;

    public CustomObject(string name) {
        this.name = name;
    }
}

public class MonoBehaviourSubClass : MonoBehaviour {
}

[Game]
public class MonoBehaviourSubClassComponent : IComponent {
    public MonoBehaviourSubClass monoBehaviour;
}

[Game]
public class CustomObjectComponent : IComponent {
    public CustomObject customObject;
}

[Game]
public class SystemObjectComponent : IComponent {
    public System.Object systemObject;
}

[Game]
public class DateTimeComponent : IComponent {
    public DateTime date;
}

[Game]
public class AnArrayComponent : IComponent {
    public string[] array;
}

[Game]
public class Array2DComponent : IComponent {
    public string[,] array2d;
}

[Game]
public class Array3DComponent : IComponent {
    public string[,,] array3d;
}

[Game]
public class JaggedArrayComponent : IComponent {
    public string[][] jaggedArray;
}

[Game]
public class ListArrayComponent : IComponent {
    public List<string>[] listArray;
}

[Game]
public class ListComponent : IComponent {
    public List<string> list;
}

[Game]
public class DictionaryComponent : IComponent {
    public Dictionary<string, string> dict;
}

[Game]
public class DictArrayComponent : IComponent {
    public Dictionary<int, string[]> dict;
    public Dictionary<int, string[]>[] dictArray;
}

[Game]
public class HashSetComponent : IComponent {
    public HashSet<string> hashset;
}

[Game]
public class MyCharComponent : IComponent {
    public char myChar;
}

public class UnsupportedObject {
    public string name;

    public UnsupportedObject(string name) {
        this.name = name;
    }
}

[Game]
public class UnsupportedObjectComponent : IComponent {
    public UnsupportedObject unsupportedObject;
}

[Game]
public class PropertyComponent : IComponent {
    public string value { get; set; }
}

[Game]
public class PersonComponent : IComponent {
    public string name;
    public string gender;
}

[Game]
public class DefaultContextComponent : IComponent {
}
