using System.Collections.Generic;
using Entitas;

[Game]
public class MyDictArrayComponent : IComponent
{
    public Dictionary<int, string[]> Dict;
    public Dictionary<int, string[]>[] DictArray;
}
